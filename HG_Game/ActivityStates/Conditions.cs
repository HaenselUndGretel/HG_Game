using HanselAndGretel.Data;
using KryptonEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public static class Conditions
	{

		public static bool NotHandicapped(Player pPlayer, Player pOtherPlayer, ActivityState pActivityState)
		{
			return true;
		}

		public static bool Intersects(Player pPlayer, Player pOtherPlayer, ActivityState pActivityState)
		{
			return true;
		}

		public static bool Contains(Player pPlayer, Player pOtherPlayer, ActivityState pActivityState)
		{
			return true;
		}
	}
}
