﻿using HanselAndGretel.Data;
using KryptonEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class UseChalk : ActivityState
	{
		public UseChalk(InteractiveObject pIObj)
			: base(pIObj)
		{
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.NotHandicapped(pPlayer, Activity.UseChalk) &&
				Conditions.Contains(pPlayer, rIObj) &&
				Conditions.EnoughChalk(pPlayer)
				)
				return Activity.UseChalk;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			base.Update(pPlayer, pOtherPlayer);
		}

		#endregion
	}
}