using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	class LegUpGrab : ActivityState
	{
		protected Vector2 mGretelActionStartOffset;
		State GrabState;

		public LegUpGrab(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			mGretelActionStartOffset = new Vector2(100, 10);
			GrabState = State.Idle;
		}

		#region Override Methods

		public override Activity GetPossibleActivity(bool pContains)
		{
			if (pContains)
				return Activity.LegUpGrab;
			return Activity.None;
		}

		public override void PrepareAction(Player pPlayer)
		{
			//Wenn beide Spieler an der passenden Position sind Action starten.
			if (rHansel.Position == rIObj.ActionPosition1 && rHansel.Input.ActionIsPressed && rGretel.Position == rIObj.ActionPosition1 + mGretelActionStartOffset && rGretel.Input.ActionIsPressed)
			{
				mStateGretel = State.Starting;
				mStateHansel = State.Starting;
				return;
			}

			if (pPlayer.GetType() == typeof(Hansel))
			{
				if (!rHansel.Input.ActionIsPressed)
				{
					rHansel.mCurrentActivity = new None();
					mStateHansel = State.Idle;
					return;
				}
				//Spieler bewegen
				rHansel.MoveAgainstPoint(rIObj.ActionPosition1);
			}
			else if (pPlayer.GetType() == typeof(Gretel))
			{
				if (!rGretel.Input.ActionIsPressed)
				{
					rGretel.mCurrentActivity = new None();
					mStateGretel = State.Idle;
					return;
				}
				//Spieler bewegen
				rGretel.MoveAgainstPoint(rIObj.ActionPosition1 + mGretelActionStartOffset);
			}
			else
			{
				throw new Exception("Nicht existenter Spielername!");
			}
		}

		public override void StartAction(Player pPlayer)
		{
			rHansel.mModel.SetAnimation("attack", false);
			rGretel.mModel.SetAnimation("attack", false);
			GrabState = State.Preparing;
			mStateHansel = State.Running;
			mStateGretel = State.Running;
		}

		public override void UpdateAction(Player pPlayer)
		{
			if (pPlayer.GetType() == typeof(Hansel) && rHansel.mModel.AnimationComplete)
			{
				if (GrabState == State.Running)
				{
					rHansel.mCurrentActivity = new None();
					mStateHansel = State.Idle;
				}
			}
			else if (pPlayer.GetType() == typeof(Gretel) && rGretel.mModel.AnimationComplete)
			{
				switch (GrabState)
				{
					case State.Preparing:
						GrabState = State.Starting;
						return;
					case State.Starting:
						if (rGretel.Input.ActionJustPressed)
						{
							rGretel.TryToGrabItem();
							rGretel.mModel.SetAnimation("attack", false);
							rHansel.mModel.SetAnimation("attack", false);
							GrabState = State.Running;
							IsAvailable = false;
						}
						return;
					case State.Running:
						rGretel.mCurrentActivity = new None();
						mStateGretel = State.Idle;
						return;
				}
				
			}
		}

		#endregion
	}
}
