using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class CaughtInSwamp : ActivityState
	{

		public CaughtInSwamp(InteractiveObject pIObj)
			: base(pIObj)
		{
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (!m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.CaughtInSwamp) &&
				Conditions.Contains(pPlayer, rIObj)
				)
			{
				//ToDo: Trap Player!
				return Activity.None;
			}
			if (m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.FreeFromSwamp) &&
				Conditions.ItemInInventory(pPlayer, pOtherPlayer, typeof(Branch)) &&
				Conditions.PlayerNearEnough(pPlayer, pOtherPlayer, 200f)
				)
				return Activity.FreeFromSwamp;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			base.Update(pPlayer, pOtherPlayer);
		}

		#endregion
	}
}
