using HanselAndGretel.Data;
using KryptonEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	class JumpOverGap : ActivityState
	{
		public JumpOverGap(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
		}

		#region Override Methods

		public override Activity GetPossibleActivity(bool pContains)
		{
			if (pContains)
				return Activity.JumpOverGap;
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
				//Spieler zu passender Position bewegen
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
				//Spieler zu passender Position bewegen
				pPlayer.MoveAgainstPoint(NearestActionPosition(pPlayer.Position));
			}
		}

		public override void StartAction(Player pPlayer)
		{
			//ToDo Passende Animation entsprechend Richtung starten.
			pPlayer.mModel.SetAnimation("attack", false);
			if (pPlayer.GetType() == typeof(Hansel))
			{
				mStateHansel = State.Running;
			}
			else if (pPlayer.GetType() == typeof(Gretel))
			{
				mStateGretel = State.Running;
			}
		}

		public override void UpdateAction(Player pPlayer)
		{
			if (pPlayer.mModel.AnimationComplete)
			{
				pPlayer.mCurrentActivity = new None();
				pPlayer.Position = DistantActionPosition(pPlayer.Position);
				if (pPlayer.GetType() == typeof(Hansel))
				{
					mStateHansel = State.Idle;
				}
				else if (pPlayer.GetType() == typeof(Gretel))
				{
					mStateGretel = State.Idle;
				}
			}
		}

		#endregion
	}
}
