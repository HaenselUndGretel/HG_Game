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
						Sequences.StartAnimation(pPlayer, Hardcoded.Anim_KnockOverTree);
						ActivityInstruction.ThumbstickDirection dir = ActivityInstruction.ThumbstickDirection.None;
						Vector2 DestinationDelta = rIObj.ActionPosition2 - rIObj.ActionPosition1;
						if (DestinationDelta.Y > 0)
							dir = ActivityInstruction.ThumbstickDirection.Down;
						else if (DestinationDelta.Y < 0)
							dir = ActivityInstruction.ThumbstickDirection.Up;
						else if (DestinationDelta.X < 0)
							dir = ActivityInstruction.ThumbstickDirection.Left;
						else
							dir = ActivityInstruction.ThumbstickDirection.Right;

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
							//Sequences.StartAnimation(pPlayer, "attack"); kann weg?
							//Sequences.StartAnimation(rIObj, Hardcoded.Anim_Tree_Falling);
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
						Sequences.StartAnimation(pPlayer, Hardcoded.Anim_Balance_EnterDown);
						StartPosition = pPlayer.SkeletonPosition;
						Direction = rIObj.DistantActionPosition(pPlayer.SkeletonPosition) - StartPosition;
						Direction.Normalize();
						++pPlayer.mCurrentState;
						break;
					case 2:
						//-----Auf Baum steigen-----
						Sequences.SynchMovementToAnimation(pPlayer, pPlayer, StartPosition, StartPosition + (Direction * Hardcoded.KnockOverTree_EnterBalanceDistance));
						if (Conditions.AnimationComplete(pPlayer))
							++pPlayer.mCurrentState;
						break;
					case 3:
						//-----Auf Baum balancieren-----
						//Update Movement
						Vector2 MovementInput = pPlayer.Input.Movement;
						if (MovementInput == Vector2.Zero) //Performance quit
							break;
						//Sideways?
						Vector2 DirectionTest = rIObj.ActionPosition2 - rIObj.ActionPosition1;
						DirectionTest.Normalize();
						bool Sideways = false;
						if (DirectionTest.Y <= Math.Sin(45) && DirectionTest.Y >= -Math.Sin(45))
							Sideways = true;

						//Runter fallen?
						bool Fail = false;
						if ((MovementInput.X == 0 && MovementInput.Y != 0 && Sideways) || (MovementInput.X != 0 && MovementInput.Y == 0 && !Sideways))
							Fail = true;
						//Fallen
						if (Fail)
						{
							GameScene.End = true;
						}
						//WalkAway?
						Vector2 TargetActionPosition = rIObj.NearestActionPosition(pPlayer.SkeletonPosition + MovementInput * 1000f);
						Vector2 MovementDirection = TargetActionPosition - pPlayer.SkeletonPosition;
						MovementDirection.Normalize();
						//Wenn Entfernung vom Player zum TargetActionPoint <= EnterBalanceEntfernung
						if ((TargetActionPosition - pPlayer.SkeletonPosition).Length() <= (MovementDirection * Hardcoded.KnockOverTree_EnterBalanceDistance).Length())
						{
							++pPlayer.mCurrentState;
							Sequences.StartAnimation(pPlayer, Hardcoded.Anim_Balance_LeaveDown); //ToDo Raus fade Animation starten. In passende Richtung!
							StartPosition = pPlayer.SkeletonPosition;
							Direction = MovementDirection;
						}

						//BalancingMovement ausführen
						pPlayer.MoveAgainstPoint(rIObj.NearestActionPosition(pPlayer.SkeletonPosition + MovementInput * 1000f), Hardcoded.KnockOverTree_BalanceSpeedFactor);
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
