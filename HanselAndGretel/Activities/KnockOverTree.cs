using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	class KnockOverTree : ActivityState
	{
		/// <summary>
		/// Muss kleiner sein als der halbe Abstand der ActionPositions und größer als der Durchmesser der angrenzenden CollisionBox
		/// </summary>
		float EnterBalanceDistance;
		float BalanceSpeedFactor;
		bool WalkAway;

		public KnockOverTree(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			EnterBalanceDistance = 80f;
			BalanceSpeedFactor = 0.6f;
			WalkAway = false;
		}

		#region Override Methods

		public override Activity GetPossibleActivity(bool pContains)
		{
			if (pContains && (mStateGretel == State.Idle && mStateHansel == State.Idle))
			{
				if (m2ndState)
					return Activity.BalanceOverTree;
				return Activity.KnockOverTree;
			}
			return Activity.None;
		}

		public override void PrepareAction(Player pPlayer)
		{
			if (pPlayer.GetType() == typeof(Hansel))
			{
				if (!m2ndState && NearestActionPosition(pPlayer.Position) != rIObj.ActionPosition1)
				{
					pPlayer.mCurrentActivity = new None();
					mStateHansel = State.Idle;
					return;
				}
				//Wenn Spieler an der passenden Position ist Action starten
				if (pPlayer.Position == NearestActionPosition(pPlayer.Position))
				{
					if (!m2ndState)
					{
						//ToDo Start Tree falling
						pPlayer.mModel.SetAnimation("attack", false);
					}
					mStateHansel = State.Starting;
					return;
				}
				pPlayer.MoveAgainstPoint(NearestActionPosition(pPlayer.Position));
			}
			else if (pPlayer.GetType() == typeof(Gretel))
			{
				if (!m2ndState && NearestActionPosition(pPlayer.Position) != rIObj.ActionPosition1)
				{
					pPlayer.mCurrentActivity = new None();
					mStateGretel = State.Idle;
					return;
				}
				//Wenn Spieler an der passenden Position ist Action starten
				if (pPlayer.Position == NearestActionPosition(pPlayer.Position))
				{
					if (!m2ndState)
					{
						//ToDo Start Tree falling
						pPlayer.mModel.SetAnimation("attack", false);
					}
					mStateGretel = State.Starting;
					return;
				}
				pPlayer.MoveAgainstPoint(NearestActionPosition(pPlayer.Position));
			}
		}

		public override void StartAction(Player pPlayer)
		{
			IsAvailable = false;
			if (m2ndState)
			{
				if (pPlayer.mModel.AnimationComplete)
				{
					WalkAway = false;
					Vector2 Direction = DistantActionPosition(pPlayer.Position) - NearestActionPosition(pPlayer.Position);
					Direction.Normalize();
					pPlayer.Position += Direction * EnterBalanceDistance;
					if (pPlayer.GetType() == typeof(Hansel))
					{
						mStateHansel = State.Running;
					}
					else if (pPlayer.GetType() == typeof(Gretel))
					{
						mStateGretel = State.Running;
					}
				}
				return;
			}
			if (pPlayer.mModel.AnimationComplete)
			{
				m2ndState = true;
				WalkAway = true;
				if (pPlayer.GetType() == typeof(Hansel) && rHansel.mModel.AnimationComplete)
				{
					mStateHansel = State.Running;
				}
				else if (pPlayer.GetType() == typeof(Gretel) && rGretel.mModel.AnimationComplete)
				{
					mStateGretel = State.Running;
				}
			}
		}

		public override void UpdateAction(Player pPlayer)
		{
			if (!WalkAway)
			{ //Wenn nicht WalkAway: Update Movement
				Vector2 MovementInput = pPlayer.Input.Movement;
				if (MovementInput == Vector2.Zero) //Performance quit
					return;
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
					throw new Exception("Du N00B bist runter gefallen! OMG, is ja doof.");
				}
				//WalkAway?
				Vector2 TargetActionPosition = NearestActionPosition(pPlayer.Position + MovementInput * 1000f);
				Vector2 MovementDirection = TargetActionPosition - pPlayer.Position;
				MovementDirection.Normalize();
				//Wenn Entfernung vom Player zum TargetActionPoint <= EnterBalanceEntfernung
				if ((TargetActionPosition - pPlayer.Position).Length() <= (MovementDirection * EnterBalanceDistance).Length())
				{
					WalkAway = true;
					pPlayer.Position = TargetActionPosition - (MovementDirection * EnterBalanceDistance);
					pPlayer.mModel.SetAnimation("attack", false); //ToDo Raus fade Animation starten. In passende Richtung!
				}

				//BalancingMovement ausführen
				pPlayer.MoveAgainstPoint(NearestActionPosition(pPlayer.Position + MovementInput * 1000f), BalanceSpeedFactor);
				return;
			}

			//WalkAway: Raus faden umsetzen
			if (pPlayer.mModel.AnimationComplete)
			{
				pPlayer.Position = NearestActionPosition(pPlayer.Position);
				WalkAway = false;
				if (pPlayer.GetType() == typeof(Hansel) && rHansel.mModel.AnimationComplete)
				{
					rHansel.mCurrentActivity = new None();
					mStateHansel = State.Idle;
				}
				else if (pPlayer.GetType() == typeof(Gretel) && rGretel.mModel.AnimationComplete)
				{
					rGretel.mCurrentActivity = new None();
					mStateGretel = State.Idle;
				}
				IsAvailable = true;
			}
		}

		#endregion
	}
}
