using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class KnockOverTree : ActivityState
	{
		protected Vector2 StartPosition;
		protected Vector2 Direction;

		protected SteppingProgress Progress;
		public ActivityInstruction ActI;

		public KnockOverTree(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			StartPosition = Vector2.Zero;
			Direction = Vector2.Zero;
			Progress = new SteppingProgress();
			ActI = new ActivityInstruction();
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (!m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.KnockOverTree) &&
				Conditions.Contains(pPlayer, rIObj) &&
				Conditions.NearestActionPosition1(pPlayer, rIObj) &&
				Conditions.ActivityNotInUseByOtherPlayer(pOtherPlayer, this)
				)
				return Activity.KnockOverTree;
			if (m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.BalanceOverTree) &&
				Conditions.Contains(pPlayer, rIObj) &&
				Conditions.ActivityNotInUseByOtherPlayer(pOtherPlayer, this)
				)
				return Activity.BalanceOverTree;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			if (!m2ndState) //KnockOverTree
			{
				switch (pPlayer.mCurrentState)
				{
					case 0:
						//-----Zu Position bewegen-----
						if (Conditions.PlayerAtActionPosition(pPlayer))
							++pPlayer.mCurrentState;
						Sequences.MovePlayerToActionPosition(pPlayer);
						break;
					case 1:
						//-----Richtung bestimmen-----
						Sequences.StartAnimation(pPlayer, Hardcoded.Anim_KnockOverTree_Up);
						ActivityInstruction.ThumbstickDirection dir = ActivityInstruction.ThumbstickDirection.None;
						Vector2 DestinationDelta = rIObj.ActionPosition2 - rIObj.ActionPosition1;
						DestinationDelta.Normalize();
						if (DestinationDelta.Y > Math.Sin(MathHelper.ToRadians(67.5f))) //Runter
						{
							dir = ActivityInstruction.ThumbstickDirection.Down;
						}
						else if (DestinationDelta.Y > Math.Sin(MathHelper.ToRadians(-22.5f))) //Seitlich
						{
							if (DestinationDelta.X < 0) //Links
							{
								dir = ActivityInstruction.ThumbstickDirection.Left;
							}
							else //Rechts
							{
								dir = ActivityInstruction.ThumbstickDirection.Right;
							}
						}
						else //Hoch
						{
							dir = ActivityInstruction.ThumbstickDirection.Up;
						}

						string animPlayer = Character.GetRightDirectionAnimation(rIObj.ActionPosition2 - rIObj.ActionPosition1, Hardcoded.Anim_KnockOverTree_Up, Hardcoded.Anim_KnockOverTree_Down, Hardcoded.Anim_KnockOverTree_Side);
						string animTree = Character.GetRightDirectionAnimation(rIObj.ActionPosition2 - rIObj.ActionPosition1, Hardcoded.Anim_Tree_KnockOver_Up, Hardcoded.Anim_Tree_KnockOver_Down, Hardcoded.Anim_Tree_KnockOver_Side);

						Character.SetSkeletonFlipState(pPlayer, rIObj.ActionPosition2 - rIObj.ActionPosition1);
						Character.SetSkeletonFlipState(rIObj, rIObj.ActionPosition2 - rIObj.ActionPosition1);
						Sequences.StartAnimation(pPlayer, animPlayer);
						Sequences.StartAnimation(rIObj, animTree);
						
						ActI.SetThumbstickDir(pPlayer, dir);
						ActI.SetThumbstickDir(pOtherPlayer, ActivityInstruction.ThumbstickDirection.None);
						++pPlayer.mCurrentState;
						break;
					case 2:
						//-----Baum umwerfen-----
						if (!Conditions.ActionThumbstickPressed(pPlayer, rIObj.ActionPosition2 - rIObj.ActionPosition1))
						{
							ActI.SetFadingState(pPlayer, true);
							Progress.StepBackward();
						}
						else
						{
							ActI.SetFadingState(pPlayer, false, false);
							Progress.StepForward();
						}
						Sequences.UpdateAnimationStepping(rIObj, Progress.Progress);
						Sequences.UpdateAnimationStepping(pPlayer, Progress.Progress);

						if (Progress.Complete)
						{
							//Baum fällt
							string anim = Character.GetRightDirectionAnimation(rIObj.ActionPosition2 - rIObj.ActionPosition1, Hardcoded.Anim_Tree_Falling_Up, Hardcoded.Anim_Tree_Falling_Down, Hardcoded.Anim_Tree_Falling_Side);
							Sequences.StartAnimation(rIObj, anim);
							//Sequences.StartAnimation(pPlayer, "attack"); kann weg?
							ActI.SetFadingState(pPlayer, false);
							++pPlayer.mCurrentState;
						}
						break;
					case 3:
						//-----Baum umgefallen?-----
						if (Conditions.AnimationComplete(rIObj))
						{
							Sequences.SetPlayerToIdle(pPlayer);
							m2ndState = true;
						}
						break;
				}
				ActI.Update();
			}
			else //BalanceOverTree
			{
				switch (pPlayer.mCurrentState)
				{
					case 0:
						//-----Zu Position bewegen-----
						if (!Conditions.ActivityNotInUseByOtherPlayer(pOtherPlayer, this))
						{
							Sequences.SetPlayerToIdle(pPlayer);
							return;
						}
						if (Conditions.PlayerAtNearestActionPosition(pPlayer))
							++pPlayer.mCurrentState;
						Sequences.MovePlayerToNearestActionPosition(pPlayer);
						break;
					case 1:
						//-----Richtung bestimmen-----
						IsAvailable = false;
						StartPosition = pPlayer.SkeletonPosition;
						Direction = rIObj.DistantActionPosition(pPlayer.SkeletonPosition) - StartPosition;
						Sequences.AnimateAccordingToDirection(pPlayer, Direction, Hardcoded.Anim_Balance_Enter_Up, Hardcoded.Anim_Balance_Enter_Down, Hardcoded.Anim_Balance_Enter_Side);
						++pPlayer.mCurrentState;
						break;
					case 2:
						//-----Auf Baum steigen-----
						Direction.Normalize();
						Sequences.SynchMovementToAnimation(pPlayer, pPlayer, StartPosition, StartPosition + (Direction * Hardcoded.KnockOverTree_EnterBalanceDistance));
						if (Conditions.AnimationComplete(pPlayer))
							++pPlayer.mCurrentState;
						break;
					case 3:
						//-----Auf Baum balancieren-----
						//Update Movement
						Vector2 MovementInput = pPlayer.Input.Movement;
						if (MovementInput == Vector2.Zero)
						{
							Sequences.StartAnimation(pPlayer, Hardcoded.Anim_Balance_Idle);
							break; //Performance quit
						}

						//Sideways?
						Vector2 DirectionTest = rIObj.ActionPosition2 - rIObj.ActionPosition1;
						DirectionTest.Normalize();
						bool Sideways = false;
						if (DirectionTest.Y <= Math.Sin(MathHelper.ToRadians(45f)) && DirectionTest.Y >= -Math.Sin(MathHelper.ToRadians(45f)))
							Sideways = true;

						//Runter fallen?
						if ((MovementInput.X == 0 && MovementInput.Y != 0 && Sideways) || (MovementInput.X != 0 && MovementInput.Y == 0 && !Sideways))
						{
							GameScene.End = true;
							break;
						}

						//BalancingMovement ausführen
						Sequences.AnimateAccordingToDirection(pPlayer, DirectionTest, Hardcoded.Anim_Balance_Up, Hardcoded.Anim_Balance_Down, Hardcoded.Anim_Balance_Side);
						pPlayer.MoveAgainstPoint(rIObj.NearestActionPosition(pPlayer.SkeletonPosition + MovementInput * 1000f), Hardcoded.KnockOverTree_BalanceSpeedFactor, null, true, false, false);

						//Leave Tree?
						Vector2 TargetActionPosition = rIObj.NearestActionPosition(pPlayer.SkeletonPosition + MovementInput * 1000f);
						Vector2 MovementDirection = TargetActionPosition - pPlayer.SkeletonPosition;
						MovementDirection.Normalize();
						//Wenn Entfernung vom Player zum TargetActionPoint <= EnterBalanceEntfernung
						if ((TargetActionPosition - pPlayer.SkeletonPosition).Length() <= (MovementDirection * Hardcoded.KnockOverTree_EnterBalanceDistance).Length())
						{
							++pPlayer.mCurrentState;
							StartPosition = pPlayer.SkeletonPosition;
							Direction = MovementDirection;
							Direction.Normalize();
							Sequences.AnimateAccordingToDirection(pPlayer, MovementDirection, Hardcoded.Anim_Balance_Leave_Up, Hardcoded.Anim_Balance_Leave_Down, Hardcoded.Anim_Balance_Leave_Side);
						}
						break;
					case 4:
						//-----Von Baum steigen-----
						Sequences.SynchMovementToAnimation(pPlayer, pPlayer, StartPosition, StartPosition + (Direction * Hardcoded.KnockOverTree_EnterBalanceDistance));
						while (pPlayer.CollisionBox.Intersects(pOtherPlayer.CollisionBox))
							pOtherPlayer.MoveManually(Direction);
						if (Conditions.AnimationComplete(pPlayer))
						{
							Sequences.SetPlayerToIdle(pPlayer);
							IsAvailable = true;
							//pPlayer.mCurrentState = 0;
						}
						break;
				}
			}
		}

		public override void Draw(SpriteBatch pSpriteBatch, Player pPlayer, Player pOtherPlayer)
		{
			Sequences.DrawActI(ActI, pSpriteBatch, pPlayer, pOtherPlayer);
		}

		#endregion
	}
}
