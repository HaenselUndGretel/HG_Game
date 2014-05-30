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
		/// Muss kleiner sein als der halbe Abstand der ActionPositions
		/// </summary>
		float EnterBalanceDistance;
		float BalanceSpeedFactor;
		bool WalkAway;

		public KnockOverTree(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			EnterBalanceDistance = 50f;
			BalanceSpeedFactor = 0.6f;
			WalkAway = false;
		}

		#region Override Methods

		public override Activity GetPossibleActivity(bool pContains)
		{
			if (pContains)
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
				//Wenn Spieler an der passenden Position ist Action starten
				if (pPlayer.Position == NearestActionPosition(pPlayer.Position))
				{
					mStateHansel = State.Starting;
					return;
				}
				//Spieler idled
				if (!pPlayer.Input.ActionIsPressed)
				{
					pPlayer.mCurrentActivity = new None();
					mStateHansel = State.Idle;
					return;
				}
				//Spieler zu passender Position bewegen
				if (pPlayer.Input.ActionIsPressed)
					pPlayer.MoveAgainstPoint(NearestActionPosition(pPlayer.Position));
			}
			else if (pPlayer.GetType() == typeof(Gretel))
			{
				//Wenn Spieler an der passenden Position ist Action starten
				if (pPlayer.Position == NearestActionPosition(pPlayer.Position))
				{
					mStateGretel = State.Starting;
					return;
				}
				//Spieler idled
				if (!pPlayer.Input.ActionIsPressed)
				{
					pPlayer.mCurrentActivity = new None();
					mStateGretel = State.Idle;
					return;
				}
				//Spieler zu passender Position bewegen
				if (pPlayer.Input.ActionIsPressed)
					pPlayer.MoveAgainstPoint(NearestActionPosition(pPlayer.Position));
			}
		}

		public override void StartAction(Player pPlayer)
		{
			if (m2ndState && pPlayer.mModel.AnimationComplete)
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
				return;
			}
			pPlayer.mModel.SetAnimation("attack", false); //Start KnockOverTree Animation
			m2ndState = true;
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
				if (DirectionTest.Y < Math.Sin(45) && DirectionTest.Y > -Math.Sin(45))
					Sideways = true;

				//Runter fallen?
				bool Fail = false;
				if (MovementInput.Y > Math.Sin(67.5) || MovementInput.Y < -Math.Sin(67.5)) //Hoch_Runter
				{
					if (Sideways)
						Fail = true;
				}
				else if (!Sideways) //Links_Rechts
				{
					Fail = true;
				}
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
			}
		}

		#endregion
	}
}
