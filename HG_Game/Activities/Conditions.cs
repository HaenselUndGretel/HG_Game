using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public static class Conditions
	{
		#region Start
		public static bool NotHandicapped(Player pPlayer, Activity pActivity)
		{
			return true;
		}

		//ActionRectangles
		public static bool Intersects(Player pPlayer, InteractiveObject pIObj)
		{
			return true;
		}

		public static bool Contains(Player pPlayer, InteractiveObject pIObj)
		{
			return true;
		}

		//ActionButton
		public static bool ActionHold(Player pPlayer)
		{
			return true;
		}

		public static bool ActionPressed(Player pPlayer)
		{
			return true;
		}

		//Item dabei
		public static bool ItemInOwnHand(Player pPlayer, Type pItemType)
		{
			return true;
		}

		public static bool ItemInOwnInventory(Player pPlayer, Type pItemType)
		{
			return true;
		}

		public static bool ItemInInventory(Player pPlayer, Player pOtherPlayer, Type pItemType)
		{
			return true;
		}

		//Kreide dabei
		public static bool EnoughChalk(Player pPlayer)
		{
			if (pPlayer.GetType() == typeof(Gretel) && ((Gretel)pPlayer).Chalk < 0)
				return true;
			return false;
		}

		//Weitere
		public static bool NearestActionPosition1(Player pPlayer, InteractiveObject IObj)
		{
			return true;
		}

		public static bool NearestActionPosition2(Player pPlayer, InteractiveObject IObj)
		{
			return true;
		}

		public static bool PlayerNearEnough(Player pPlayer, Player pOtherPlayer, float Distance)
		{
			return true;
		}

		public static bool ActivityNotInUseByOtherPlayer(Player pOtherPlayer, ActivityState pActivityState)
		{
			return true;
		}
		#endregion

		#region Update

		public static bool PlayersAtActionPositions(Player pPlayer, Player pOtherPlayer, Nullable<Vector2> pOffsetGretel = null)
		{
			//Hansel an AP1 & Gretel an AP2
			//Wenn pOffsetGretel nicht null ist muss Gretel an AP1 + pOffsetGretel sitzen.
			return true;
		}

		public static bool AnimationComplete(SpineObject pSpine)
		{
			return pSpine.AnimationComplete;
		}

		#endregion
	}
}
