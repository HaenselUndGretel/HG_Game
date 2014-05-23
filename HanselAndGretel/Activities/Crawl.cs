using HanselAndGretel.Data;
using KryptonEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	class Crawl : ActivityState
	{
		#region Properties

		protected Hansel rHansel;
		protected Gretel rGretel;
		protected InteractiveObject rIObj;

		#endregion

		public Crawl(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
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
				return Activity.Crawl;
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
