using HanselAndGretel.Data;
using KryptonEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	class CaughtInCobweb : ActivityState
	{
		public CaughtInCobweb(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
		}

		#region Override Methods

		public override Activity GetPossibleActivity(bool pContains)
		{
			if (m2ndState && (rHansel.Inventory.Contains(typeof(Knife)) || rGretel.Inventory.Contains(typeof(Knife))))
				return Activity.FreeFromCobweb;
			return Activity.None;
		}

		public override void UpdateAction(Player pPlayer)
		{
			if (pPlayer.GetType() == typeof(Hansel))
			{

			}
			else if (pPlayer.GetType() == typeof(Gretel))
			{

			}
		}

		#endregion
	}
}
