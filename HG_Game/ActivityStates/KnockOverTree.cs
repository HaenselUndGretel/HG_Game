﻿using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class KnockOverTree : ActivityState
	{

		public KnockOverTree(InteractiveObject pIObj)
			: base(pIObj)
		{
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (!m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.KnockOverTree) &&
				Conditions.Contains(pPlayer, rIObj) &&
				Conditions.NearestActionPosition1(pPlayer, rIObj) &&
				Conditions.ActivityNotInUseByOtherPlayer(pOtherPlayer, this)
				)
				return Activity.KnockOverTree;
			if (m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.BalanceOverTree) &&
				Conditions.Contains(pPlayer, rIObj) &&
				Conditions.ActivityNotInUseByOtherPlayer(pOtherPlayer, this)
				)
				return Activity.BalanceOverTree;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			base.Update(pPlayer, pOtherPlayer);
		}

		#endregion
	}
}