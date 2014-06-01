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

		protected Vector2 mGretelActionStartOffset;
		protected Vector2 mGretelMovedByAction;

		#endregion

		public LegUp(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			mGretelActionStartOffset = new Vector2(80, -20);
			mGretelMovedByAction = new Vector2(5, -200);
		}

		#region Override Methods

		public override Activity GetPossibleActivity(bool pContains)
		{
			if (pContains)
				return Activity.LegUp;
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
		}

		public override void StartAction(Player pPlayer)
		{
			rHansel.mModel.SetAnimation("attack", false);
			rGretel.mModel.SetAnimation("attack", false);
			mStateHansel = State.Running;
			mStateGretel = State.Running;
		}

		public override void UpdateAction(Player pPlayer)
		{
			if (pPlayer.GetType() == typeof(Hansel) && rHansel.mModel.AnimationComplete)
			{
				rHansel.mCurrentActivity = new None();
				mStateHansel = State.Idle;
			}
			else if (pPlayer.GetType() == typeof(Gretel) && rGretel.mModel.AnimationComplete)
			{
				rGretel.Position += mGretelMovedByAction;
				rGretel.mCurrentActivity = new None();
				mStateGretel = State.Idle;
			}
		}

		#endregion
	}
}
