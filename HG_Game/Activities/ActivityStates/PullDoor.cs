using HanselAndGretel.Data;
using KryptonEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class PullDoor : ActivityState
	{

		public PullDoor(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.NotHandicapped(pPlayer, Activity.PullDoor) &&
				Conditions.Contains(pPlayer, rIObj)
				)
				return Activity.PullDoor;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			base.Update(pPlayer, pOtherPlayer);
		}

		#endregion
	}
}
