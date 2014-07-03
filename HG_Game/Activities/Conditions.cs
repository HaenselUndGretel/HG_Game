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
			return !pPlayer.mHandicaps.Contains(pActivity);
		}

		//ActionRectangles
		public static bool Intersects(Player pPlayer, InteractiveObject pIObj)
		{
			foreach (Rectangle rect in pIObj.ActionRectList)
			{
				if (rect.Intersects(pPlayer.CollisionBox)) //Intersects
				{
					return true;
				}
			}
			return false;
		}

		public static bool Contains(Player pPlayer, InteractiveObject pIObj)
		{
			foreach (Rectangle rect in pIObj.ActionRectList)
			{
				if (rect.Contains(pPlayer.CollisionBox)) //Contains
				{
					return true;
				}
			}
			return false;
		}

		//ActionButton
		public static bool ActionHold(Player pPlayer)
		{
			return pPlayer.Input.ActionIsPressed;
		}

		public static bool ActionPressed(Player pPlayer)
		{
			return pPlayer.Input.ActionJustPressed;
		}

		//Item dabei
		public static bool ItemInOwnHand(Player pPlayer, Type pItemType)
		{
			return pPlayer.Inventory.Contains(pItemType);
		}

		public static bool ItemInOwnInventory(Player pPlayer, Type pItemType)
		{
			return pPlayer.Inventory.Contains(pItemType, false);
		}

		public static bool ItemInInventory(Player pPlayer, Player pOtherPlayer, Type pItemType)
		{
			return (pPlayer.Inventory.Contains(pItemType, false) || pOtherPlayer.Inventory.Contains(pItemType, false)) ? true : false;
		}

		//Kreide dabei
		public static bool EnoughChalk(Player pPlayer)
		{
			return (pPlayer.GetType() == typeof(Gretel) && ((Gretel)pPlayer).Chalk < 0) ? true : false;
		}

		//Weitere
		public static bool NearestActionPosition1(Player pPlayer, InteractiveObject IObj)
		{
			return (IObj.NearestActionPosition(pPlayer.Position) == IObj.ActionPosition1) ? true : false;
		}

		public static bool NearestActionPosition2(Player pPlayer, InteractiveObject IObj)
		{
			return (IObj.NearestActionPosition(pPlayer.Position) == IObj.ActionPosition2) ? true : false;
		}

		public static bool PlayerNearEnough(Player pPlayer, Player pOtherPlayer, float Distance)
		{
			Vector2 Center1 = new Vector2(pPlayer.CollisionBox.Center.X, pPlayer.CollisionBox.Center.Y);
			Vector2 Center2 = new Vector2(pOtherPlayer.CollisionBox.Center.X, pOtherPlayer.CollisionBox.Center.Y);
			return ((Center1 - Center2).Length() <= Distance) ? true : false;
		}

		public static bool ActivityNotInUseByOtherPlayer(Player pOtherPlayer, ActivityState pActivityState)
		{
			return (pOtherPlayer.mCurrentActivity == pActivityState) ? false : true;
		}

		#endregion

		#region Update

		public static bool PlayersAtActionPositions(Player pPlayer, Player pOtherPlayer, Nullable<Vector2> pOffsetGretel = null)
		{
			//Hansel an AP1 & Gretel an AP2
			if (pPlayer.mCurrentActivity != pOtherPlayer.mCurrentActivity)
				throw new Exception("Spieler sind nicht an gleicher Activity beteiligt. Soll hier false returned werden?");
			Player TmpHansel = new Player("");
			Player TmpGretel = new Player("");
			if (pPlayer.GetType() == typeof(Hansel))
				TmpHansel = pPlayer;
			else
				TmpGretel = pPlayer;
			if (pOtherPlayer.GetType() == typeof(Hansel))
				TmpHansel = pOtherPlayer;
			else
				TmpGretel = pOtherPlayer;
			if (TmpHansel.Position != TmpHansel.mCurrentActivity.rIObj.ActionPosition1)
				return false;
			if (pOffsetGretel == null && TmpGretel.Position != TmpGretel.mCurrentActivity.rIObj.ActionPosition2)
				return false;
			//Wenn pOffsetGretel nicht null ist muss Gretel an AP1 + pOffsetGretel sitzen.
			if (pOffsetGretel != null && TmpGretel.Position != TmpGretel.mCurrentActivity.rIObj.ActionPosition1 + pOffsetGretel)
				return false;
			return true;
		}

		public static bool PlayersAtWellPositions(Player pPlayer, Player pOtherPlayer, bool pGretelDown)
		{
			//Hansel an AP1 & Gretel->AP1 + Offset / AP2
			if (pPlayer.mCurrentActivity != pOtherPlayer.mCurrentActivity)
				throw new Exception("Spieler sind nicht an gleicher Activity beteiligt. Soll hier false returned werden?");
			Player TmpHansel = new Player("");
			Player TmpGretel = new Player("");
			if (pPlayer.GetType() == typeof(Hansel))
				TmpHansel = pPlayer;
			else
				TmpGretel = pPlayer;
			if (pOtherPlayer.GetType() == typeof(Hansel))
				TmpHansel = pOtherPlayer;
			else
				TmpGretel = pOtherPlayer;
			if (TmpHansel.Position != TmpHansel.mCurrentActivity.rIObj.ActionPosition1)
				return false;
			if (pGretelDown && TmpGretel.Position != TmpGretel.mCurrentActivity.rIObj.ActionPosition2)
				return false;
			if (!pGretelDown && TmpGretel.Position != TmpGretel.mCurrentActivity.rIObj.ActionPosition1 + new Vector2(TmpGretel.mCurrentActivity.rIObj.ActionPosition2.X - TmpGretel.mCurrentActivity.rIObj.ActionPosition1.X, 0))
				return false;
			return true;
		}


		public static bool AnimationComplete(SpineObject pSpine)
		{
			return pSpine.AnimationComplete;
		}

		#endregion
	}
}
