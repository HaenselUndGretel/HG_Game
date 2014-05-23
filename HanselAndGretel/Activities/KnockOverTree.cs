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
		public KnockOverTree(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
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
