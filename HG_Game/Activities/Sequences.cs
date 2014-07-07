using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public static class Sequences
	{
		//Move & Set to Position
		public static void MoveToPosition(Player pPlayer, Vector2 pPosition, float pSpeedFactor = 1f)
		{
			pPlayer.MoveAgainstPoint(pPosition, pSpeedFactor);
		}

		public static void MovePlayerToRightActionPosition(Player pPlayer, Nullable<Vector2> pOffsetGretel = null, float pSpeedFactor = 1f)
		{
			//Hansel->AP1 Gretel->AP2
			Vector2 Destination = pPlayer.mCurrentActivity.rIObj.ActionPosition1;
			if (pPlayer.GetType() == typeof(Gretel))
			{
				//Wenn pOffsetGretel nicht null ist muss Gretel an AP1 + pOffsetGretel gesetzt werden.
				if (pOffsetGretel != null)
					Destination = pPlayer.mCurrentActivity.rIObj.ActionPosition1 + (Vector2)pOffsetGretel;
				else
					Destination = pPlayer.mCurrentActivity.rIObj.ActionPosition2;
			}
			pPlayer.MoveAgainstPoint(Destination, pSpeedFactor);
		}

		public static void MovePlayerToWellActionPosition(Player pPlayer, bool pGretelDown, float pSpeedFactor = 1f)
		{
			//Hansel->AP1 Gretel->AP1 + Offset / AP2
			Vector2 Destination = pPlayer.mCurrentActivity.rIObj.ActionPosition1;
			if (pPlayer.GetType() == typeof(Gretel))
			{
				//Wenn pOffsetGretel oben ist muss Gretel an AP1 + Offset gesetzt werden.
				if (!pGretelDown)
					Destination = pPlayer.mCurrentActivity.rIObj.ActionPosition1 + new Vector2(pPlayer.mCurrentActivity.rIObj.ActionPosition2.X - pPlayer.mCurrentActivity.rIObj.ActionPosition1.X, 0);
				else //sonst an AP2
					Destination = pPlayer.mCurrentActivity.rIObj.ActionPosition2;
			}
			pPlayer.MoveAgainstPoint(Destination, pSpeedFactor);
		}

		public static void SetPlayerToPosition(Player pPlayer, Vector2 pPosition)
		{
			pPlayer.PositionIO = pPosition;
		}

		//Move für Cobweb
		public static void MoveUpDown(Player pPlayer, bool pUp, float pSpeedFactor = 1f)
		{
			Vector2 Direction = new Vector2(0, 1);
			if (pUp)
				Direction = new Vector2(0, -1);
			pPlayer.MoveManually(Direction, pSpeedFactor);
		}

		//Move für Swamp
		public static void MoveAway(Player pPlayer, Vector2 pSource, float pSpeedFactor = 1f)
		{
			Vector2 Direction = pPlayer.PositionIO - pSource;
			Direction.Normalize();
			pPlayer.MoveManually(Direction, pSpeedFactor);
		}

		//Start Animation
		public static void StartAnimation(SpineObject pSpine, string pAnimation, bool pLoop = false)
		{
			pSpine.SetAnimation(pAnimation, pLoop, true);
		}

		//Animation Stepping
		public static void UpdateAnimationStepping(SpineObject pSpine, float pProgress)
		{
			pSpine.AnimationState.GetCurrent(0).TimeScale = pSpine.AnimationState.GetCurrent(0).EndTime * pProgress;
		}

		//Movement Stepping
		public static void UpdateMovementStepping(Player pPlayer, float pProgress, Vector2 pSource, Vector2 pDestination)
		{
			pPlayer.PositionIO = (pDestination - pSource) * pProgress;
		}

		public static void UpdateMovementStepping(InteractiveObject pIObj, float pProgress, Vector2 pSource, Vector2 pDestination)
		{
			pIObj.PositionIO = (pDestination - pSource) * pProgress;
		}

		//Pause & Play Animation
		public static void PausePlayAnimation(AnimationState pAnimationState, bool pPlay)
		{
			if (pAnimationState.TimeScale == 1f)
				pAnimationState.TimeScale = 0f;
			else if (pAnimationState.TimeScale == 0f)
				pAnimationState.TimeScale = 1f;
		}

		//Player während Animation bewegen
		public static void SynchMovementToAnimation(SpineObject pSpineToSynchTo, Player pPlayer, Vector2 pSource, Vector2 pDestination)
		{
			float Progress = pSpineToSynchTo.AnimationState.GetCurrent(0).Time / pSpineToSynchTo.AnimationState.GetCurrent(0).EndTime;
			pPlayer.PositionIO = (pDestination - pSource) * Progress;
		}

		//QuickTimeEvent

		//Set States
		public static void SetPlayerToIdle(Player pPlayer)
		{
			pPlayer.mCurrentActivity = ActivityHandler.None;
			pPlayer.mCurrentState = 0;
		}

		//Gretel States setzen (Brunnen)

		//Set Arrow States

		//Rock & Arrow Menu Navigation

		//Overlay ein-/ausblenden

		//Stuff (BrunnenKorb) bewegen

		//EndeGelände
		public static void End()
		{
			//So ist das nicht passiert...
			throw new NotImplementedException();
		}

	}
}
