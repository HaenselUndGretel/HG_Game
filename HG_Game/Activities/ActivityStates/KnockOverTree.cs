using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class KnockOverTree : ActivityState
	{
		// von 100 auf 74 wegen anderen Hitboxen
		protected const float EnterBalanceDistance = 74.0f;
		protected const float BalanceSpeedFactor = 0.6f;
		protected Vector2 StartPosition;
		protected Vector2 Direction;

		public KnockOverTree(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			StartPosition = Vector2.Zero;
			Direction = Vector2.Zero;
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
						if (Conditions.PlayerAtActionPosition(pPlayer))
							++pPlayer.mCurrentState;
						Sequences.MovePlayerToActionPosition(pPlayer);
						break;
					case 1:
						Sequences.StartAnimation(pPlayer, "attack");
						++pPlayer.mCurrentState;
						break;
					case 2:
						if (Conditions.AnimationComplete(pPlayer))
						{
							Sequences.SetPlayerToIdle(pPlayer);
							m2ndState = true;
						}
						break;
				}
			}
			else //BalanceOverTree
			{
				switch (pPlayer.mCurrentState)
				{
					case 0:
						if (Conditions.PlayerAtNearestActionPosition(pPlayer))
							++pPlayer.mCurrentState;
						Sequences.MovePlayerToNearestActionPosition(pPlayer);
						break;
					case 1:
						IsAvailable = false;
						Sequences.StartAnimation(pPlayer, "attack");
						StartPosition = pPlayer.SkeletonPosition;
						Direction = rIObj.DistantActionPosition(pPlayer.SkeletonPosition) - StartPosition;
						Direction.Normalize();
						++pPlayer.mCurrentState;
						break;
					case 2:
						Sequences.SynchMovementToAnimation(pPlayer, pPlayer, StartPosition, StartPosition + (Direction * EnterBalanceDistance));
						if (Conditions.AnimationComplete(pPlayer))
							++pPlayer.mCurrentState;
						break;
					case 3:
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
							Sequences.End();
						}
						//WalkAway?
						Vector2 TargetActionPosition = rIObj.NearestActionPosition(pPlayer.SkeletonPosition + MovementInput * 1000f);
						Vector2 MovementDirection = TargetActionPosition - pPlayer.SkeletonPosition;
						MovementDirection.Normalize();
						//Wenn Entfernung vom Player zum TargetActionPoint <= EnterBalanceEntfernung
						if ((TargetActionPosition - pPlayer.SkeletonPosition).Length() <= (MovementDirection * EnterBalanceDistance).Length())
						{
							++pPlayer.mCurrentState;
							// Kann man auch sein lassen.
							//Sequences.SetPlayerToPosition(pPlayer, TargetActionPosition - pPlayer.SkeletonPosition);
							Sequences.StartAnimation(pPlayer, "attack"); //ToDo Raus fade Animation starten. In passende Richtung!
							StartPosition = pPlayer.SkeletonPosition;
						}

						//BalancingMovement ausführen
						pPlayer.MoveAgainstPoint(rIObj.NearestActionPosition(pPlayer.SkeletonPosition + MovementInput * 1000f), BalanceSpeedFactor);
						break;
					case 4:
						Sequences.SynchMovementToAnimation(pPlayer, pPlayer, StartPosition, StartPosition + (Direction * EnterBalanceDistance));
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

		#endregion
	}
}
