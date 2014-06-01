using HanselAndGretel.Data;
using KryptonEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	class PushRock : ActivityState
	{
		protected int ProgressCounterHansel;
		protected int ProgressCounterGretel;
		protected const int MaxProgress = 10;

		public PushRock(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			:base(pHansel, pGretel, pIObj)
		{
			ProgressCounterHansel = 0;
			ProgressCounterGretel = 0;
		}

		#region Override Methods

		public override Activity GetPossibleActivity(bool pContains)
		{
			if (pContains)
				return Activity.PushRock;
			return Activity.None;
		}

		public override void PrepareAction(Player pPlayer)
		{
			//Wenn beide Spieler an der passenden Position sind Action starten.
			if (rHansel.Position == rIObj.ActionPosition1 && rHansel.Input.ActionIsPressed && rGretel.Position == rIObj.ActionPosition2 && rGretel.Input.ActionIsPressed)
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
				rGretel.MoveAgainstPoint(rIObj.ActionPosition2);
			}
		}

		public override void StartAction(Player pPlayer)
		{
			if (pPlayer.GetType() == typeof(Hansel))
			{
				ProgressCounterHansel = 0;
				//ToDo Start Animation for QuickEvent Stepping.
				mStateHansel = State.Running;
			}
			else if (pPlayer.GetType() == typeof(Gretel))
			{
				ProgressCounterGretel = 0;
				//ToDo Start Animation for QuickEvent Stepping.
				mStateGretel = State.Running;
			}
		}


		public override void UpdateAction(Player pPlayer)
		{
			if (ProgressCounterHansel >= MaxProgress && ProgressCounterGretel >= MaxProgress)
			{
				rHansel.mCurrentActivity = new None();
				rGretel.mCurrentActivity = new None();
				mStateHansel = State.Idle;
				mStateGretel = State.Idle;
				m2ndState = true;
				IsAvailable = false;
				return;
			}

			if (pPlayer.GetType() == typeof(Hansel))
			{
				if (pPlayer.Input.ActionJustPressed && ProgressCounterHansel <= ProgressCounterGretel)
				{
					//ToDo Step through QuickEvent Animation
					++ProgressCounterHansel;
				}
			}
			else if (pPlayer.GetType() == typeof(Gretel))
			{
				if (pPlayer.Input.ActionJustPressed && ProgressCounterGretel <= ProgressCounterHansel)
				{
					//ToDo Step through QuickEvent Animation
					++ProgressCounterGretel;
				}
			}
		}

		#endregion
	}
}
