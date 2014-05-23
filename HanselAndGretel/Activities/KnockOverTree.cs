using HanselAndGretel.Data;
using KryptonEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	class KnockOverTree : ActivityState
	{
				#region Properties

		protected Hansel rHansel;
		protected Gretel rGretel;

		#endregion

		public KnockOverTree(Hansel pHansel, Gretel pGretel)
			:base()
		{
			rHansel = pHansel;
			rGretel = pGretel;
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

		public override void PrepareAction(string pPlayer)
		{
			if (pPlayer == "Hansel")
			{
				if (rHansel.Input.ActionJustPressed)
					mStateHansel = State.Starting;
			}
			else if (pPlayer == "Gretel")
			{
				if (rGretel.Input.ActionJustPressed)
					mStateGretel = State.Starting;
			}
			else
			{
				throw new Exception("Nicht existenter Spielername!");
			}
		}

		public override void StartAction(string pPlayer)
		{
			if (pPlayer == "Hansel")
			{
				mStateHansel = State.Running;
			}
			else if (pPlayer == "Gretel")
			{
				mStateGretel = State.Running;
			}
			else
			{
				throw new Exception("Nicht existenter Spielername!");
			}
		}

		public override void UpdateAction(string pPlayer)
		{
			if (pPlayer == "Hansel")
			{
				
			}
			else if (pPlayer == "Gretel")
			{
				
			}
			else
			{
				throw new Exception("Nicht existenter Spielername!");
			}
		}

		#endregion
	}
}
