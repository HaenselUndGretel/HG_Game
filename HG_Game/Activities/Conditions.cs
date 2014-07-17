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
		public static bool Contains(Player pPlayer, InteractiveObject pIObj)
		{
			foreach (Rectangle rect in pIObj.ActionRectList)
			{
				if (rect.Contains(new Point((int)pPlayer.SkeletonPosition.X, (int)pPlayer.SkeletonPosition.Y))) //Contains
				{
					return true;
				}
			}
			return false;
		}

		public static bool CobwebSwampIntersects(Player pPlayer, InteractiveObject pIObj)
		{
			foreach (Rectangle rect in pIObj.ActionRectList)
			{
				if (rect.Intersects(pPlayer.CollisionBox)) //Contains
				{
					return true;
				}
			}
			foreach (Rectangle rect in pIObj.CollisionRectList)
			{
				if (rect.Intersects(pPlayer.CollisionBox)) //Contains
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

		//Weitere
		public static bool NearestActionPosition1(Player pPlayer, InteractiveObject IObj)
		{
			return (IObj.NearestActionPosition(pPlayer.SkeletonPosition) == IObj.ActionPosition1) ? true : false;
		}

		public static bool NearestActionPosition2(Player pPlayer, InteractiveObject IObj)
		{
			return (IObj.NearestActionPosition(pPlayer.SkeletonPosition) == IObj.ActionPosition2) ? true : false;
		}

		public static bool AtRightSideOfWell(Player pPlayer, InteractiveObject pIObj)
		{

			if (pPlayer.GetType() == typeof(Hansel))
			{
				return ((pIObj.ActionPosition1 - pPlayer.SkeletonPosition).Length() < (WellActionPosition2Up(pIObj) - pPlayer.SkeletonPosition).Length()) ? true : false;
			}
			else
			{
				if (pIObj.ActivityState.m2ndState) //Gretel im Brunnenschacht
					return ((pIObj.ActionPosition1 - pPlayer.SkeletonPosition).Length() <= (pIObj.ActionPosition2 - pPlayer.SkeletonPosition).Length()) ? false : true;
				return ((pIObj.ActionPosition1 - pPlayer.SkeletonPosition).Length() <= (WellActionPosition2Up(pIObj) - pPlayer.SkeletonPosition).Length()) ? false : true; //Oben
			}

		}

		public static Vector2 WellActionPosition2Up(InteractiveObject pIObj)
		{
			Vector2 Pos = pIObj.ActionPosition1;
			Pos.X = pIObj.ActionPosition2.X;
			return Pos;
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

		public static bool PlayerAtActionPosition(Player pPlayer, bool pAP2 = false)
		{
			Vector2 APPosition = pPlayer.mCurrentActivity.rIObj.ActionPosition1;
			if (pAP2)
				APPosition = pPlayer.mCurrentActivity.rIObj.ActionPosition2;
			if (pPlayer.SkeletonPosition == APPosition)
				return true;
			return false;
		}

		public static bool PlayerAtCobwebActionPosition(Player pPlayer, Vector2 pOffsetOtherPosition)
		{
			if (pPlayer.SkeletonPosition == pPlayer.mCurrentActivity.rIObj.ActionPosition2 || (pPlayer.SkeletonPosition == pPlayer.mCurrentActivity.rIObj.ActionPosition2 + pOffsetOtherPosition))
				return true;
			return false;
		}

		public static bool PlayerAtNearestActionPosition(Player pPlayer)
		{
			if (pPlayer.SkeletonPosition == pPlayer.mCurrentActivity.rIObj.NearestActionPosition(pPlayer.SkeletonPosition))
				return true;
			return false;
		}

		public static bool PlayersAtActionPositions(Player pPlayer, Player pOtherPlayer, Nullable<Vector2> pOffsetGretel = null)
		{
			if (pPlayer.mCurrentActivity.rIObj == null || pOtherPlayer.mCurrentActivity.rIObj == null) return false;
			//Hansel an AP1 & Gretel an AP2
			if (pPlayer.mCurrentActivity != pOtherPlayer.mCurrentActivity)
				return false;
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
			if (TmpHansel.SkeletonPosition != TmpHansel.mCurrentActivity.rIObj.ActionPosition1)
				return false;
			if (pOffsetGretel == null && TmpGretel.SkeletonPosition != TmpGretel.mCurrentActivity.rIObj.ActionPosition2)
				return false;
			//Wenn pOffsetGretel nicht null ist muss Gretel an AP1 + pOffsetGretel sitzen.
			if (pOffsetGretel != null && TmpGretel.SkeletonPosition != TmpGretel.mCurrentActivity.rIObj.ActionPosition1 + pOffsetGretel)
				return false;
			return true;
		}

		public static bool PlayersAtWellPositions(Player pPlayer, Player pOtherPlayer, bool pGretelDown)
		{
			//Hansel an AP1 & Gretel->AP1 + Offset / AP2
			if (pPlayer.mCurrentActivity != pOtherPlayer.mCurrentActivity)
				return false;
			Player TmpHansel;
			Player TmpGretel;
			if (pPlayer.GetType() == typeof(Hansel))
			{
				TmpHansel = pPlayer;
				TmpGretel = pOtherPlayer;
			}
			else
			{
				TmpHansel = pOtherPlayer;
				TmpGretel = pPlayer;
			}
			if (TmpHansel.SkeletonPosition != TmpHansel.mCurrentActivity.rIObj.ActionPosition1)
				return false;
			if (pGretelDown && TmpGretel.SkeletonPosition != TmpGretel.mCurrentActivity.rIObj.ActionPosition2)
				return false;
			if (!pGretelDown && TmpGretel.SkeletonPosition != Conditions.WellActionPosition2Up(TmpGretel.mCurrentActivity.rIObj))
				return false;
			return true;
		}


		public static bool AnimationComplete(SpineObject pSpine)
		{
			return pSpine.AnimationComplete;
		}

		public static bool ActionThumbstickPressed(Player pPlayer, Vector2 ThumbstickDirection, float pTolerance = 20f)
		{
			Vector2 Thumbstick = pPlayer.Input.Movement;
			ThumbstickDirection.Normalize();
			Thumbstick.Normalize();
			if (pPlayer.Input.ActionIsPressed && Thumbstick != Vector2.Zero && MathHelper.ToDegrees((float)Math.Acos(Vector2.Dot(Thumbstick, ThumbstickDirection))) < pTolerance)
				return true;
			return false;
		}

		public static bool ActionThumbstickPressedBothPlayer(Player pPlayer, Player pOtherPlayer, Vector2 ThumbstickDirection, float pTolerance = 20f)
		{
			Vector2 Thumbstick = pPlayer.Input.Movement;
			ThumbstickDirection.Normalize();
			Thumbstick.Normalize();
			int p = 0;
			if (pPlayer.Input.ActionIsPressed && Thumbstick != Vector2.Zero && MathHelper.ToDegrees((float)Math.Acos(Vector2.Dot(Thumbstick, ThumbstickDirection))) < pTolerance)
				++p;

			Thumbstick = pOtherPlayer.Input.Movement;
			Thumbstick.Normalize();
			if (pOtherPlayer.Input.ActionIsPressed && Thumbstick != Vector2.Zero && MathHelper.ToDegrees((float)Math.Acos(Vector2.Dot(Thumbstick, ThumbstickDirection))) < pTolerance)
				++p;
			if (p == 2)
				return true;
			return false;
		}

		#endregion
	}
}
