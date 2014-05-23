using HanselAndGretel.Data;
using KryptonEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	class CaughtInSwamp : ActivityState
	{
		public CaughtInSwamp(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
		}

		#region Override Methods

		public override Activity GetPossibleActivity(bool pContains)
		{
			if (m2ndState && (rHansel.Inventory.Contains(typeof(Branch)) || rGretel.Inventory.Contains(typeof(Branch))))
				return Activity.FreeFromSwamp;
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
