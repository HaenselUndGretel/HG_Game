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
			if (pPlayer.GetType() == typeof(Hansel))
			{
				if (!rHansel.Input.ActionIsPressed)
				{
					rHansel.mCurrentActivity = new None();
					mStateHansel = State.Idle;
					return;
				}

				//Wenn der Spieler an der passenden Position ist Action starten.
				if (rHansel.Position == rIObj.ActionPosition1 && rGretel.Position == rIObj.ActionPosition1 + mGretelActionStartOffset)
				{
					mStateHansel = State.Starting;
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

				//Wenn der Spieler an der passenden Position ist Action starten.
				if (rGretel.Position == rIObj.ActionPosition1 + mGretelActionStartOffset && rHansel.Position == rIObj.ActionPosition1)
				{
					mStateGretel = State.Starting;
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
			else if (pPlayer.GetType() == typeof(Gretel) && rHansel.mModel.AnimationComplete)
			{
				rGretel.Position += mGretelMovedByAction;
				rGretel.mCurrentActivity = new None();
				mStateGretel = State.Idle;
			}
		}

		#endregion
	}
}
