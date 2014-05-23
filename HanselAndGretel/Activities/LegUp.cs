using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	class LegUp : ActivityState
	{
		#region Properties

		protected Hansel rHansel;
		protected Gretel rGretel;
		protected InteractiveObject rIObj;
		protected Vector2 mGretelActionStartOffset;
		protected Vector2 mGretelMovedByAction;

		#endregion

		public LegUp(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			:base()
		{
			rHansel = pHansel;
			rGretel = pGretel;
			rIObj = pIObj;
		}

		#region Override Methods

		public override Activity GetPossibleActivity(bool pContains)
		{
			if (pContains)
				return Activity.LegUp;
			return Activity.None;
		}

		public override void PrepareAction(string pPlayer)
		{
			if (pPlayer == "Hansel")
			{
				if (!rHansel.Input.ActionIsPressed)
				{
					rHansel.mCurrentActivity = new None();
					mStateHansel = State.Idle;
					return;
				}

				//Wenn der Spieler an der passenden Position ist Action starten.
				if (rHansel.Position == rIObj.ActionPosition1)
				{
					mStateHansel = State.Starting;
				}

				Vector2 Movement = rIObj.ActionPosition1 - rHansel.Position; //Vector SpielerPosition -> ActionPosition
				Movement.Normalize(); //Bewegungsrichtung

				Vector2 NewDistance = rIObj.ActionPosition1 - (rHansel.Position + Movement); //Vector Neue SpielerPosition -> ActionPosition
				NewDistance.Normalize(); //Neue Bewegunsrichtung

				float TmpSpeedFactor = 1f;

				if (NewDistance == Movement * -1) //Neue Beweungsrichtung zeigt gegen Bewegungsrichtung
				{
					TmpSpeedFactor = (rIObj.ActionPosition1 - rHansel.Position).Length() / (Movement.Length() * rHansel.Speed); //Nicht übers Ziel hinaus bewegen
				}

				rHansel.MoveManually(Movement, TmpSpeedFactor); //Spieler bewegen
			}
			else if (pPlayer == "Gretel")
			{
				if (!rGretel.Input.ActionIsPressed)
				{
					rGretel.mCurrentActivity = new None();
					mStateHansel = State.Idle;
					return;
				}

				//Wenn der Spieler an der passenden Position ist Action starten.
				if (rGretel.Position == rIObj.ActionPosition1 + mGretelActionStartOffset)
				{
					mStateHansel = State.Starting;
				}

				Vector2 Movement = rIObj.ActionPosition1 + mGretelActionStartOffset - rGretel.Position; //Vector SpielerPosition -> ActionPosition
				Movement.Normalize(); //Bewegungsrichtung

				Vector2 NewDistance = rIObj.ActionPosition1 + mGretelActionStartOffset - (rGretel.Position + Movement); //Vector Neue SpielerPosition -> ActionPosition
				NewDistance.Normalize(); //Neue Bewegunsrichtung

				float TmpSpeedFactor = 1f;

				if (NewDistance == Movement * -1) //Neue Beweungsrichtung zeigt gegen Bewegungsrichtung
				{
					TmpSpeedFactor = (rIObj.ActionPosition1 + mGretelActionStartOffset - rGretel.Position).Length() / (Movement.Length() * rGretel.Speed); //Nicht übers Ziel hinaus bewegen
				}

				rGretel.MoveManually(Movement, TmpSpeedFactor); //Spieler bewegen
			}
			else
			{
				throw new Exception("Nicht existenter Spielername!");
			}
		}

		public override void StartAction(string pPlayer)
		{
			if ((pPlayer == "Hansel" && mStateGretel == State.Starting) || (pPlayer == "Gretel" && mStateHansel == State.Starting))
			{
				rHansel.mModel.SetAnimation("attack", false);
				rGretel.mModel.SetAnimation("attack", false);
				mStateHansel = State.Running;
				mStateGretel = State.Running;
			}
		}

		public override void UpdateAction(string pPlayer)
		{
			if (pPlayer == "Hansel" && rHansel.mModel.AnimationComplete)
			{
				rHansel.mCurrentActivity = new None();
				mStateHansel = State.Idle;
			}
			else if (pPlayer == "Gretel" && rHansel.mModel.AnimationComplete)
			{
				rGretel.Position += mGretelMovedByAction;
				rGretel.mCurrentActivity = new None();
				mStateGretel = State.Idle;
			}
		}

		#endregion
	}
}
