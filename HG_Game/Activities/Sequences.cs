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

		public static void MovePlayerToActionPosition(Player pPlayer, bool pAP2 = false)
		{
			Vector2 APPosition = pPlayer.mCurrentActivity.rIObj.ActionPosition1;
			if (pAP2)
				APPosition = pPlayer.mCurrentActivity.rIObj.ActionPosition2;
			pPlayer.MoveAgainstPoint(APPosition);
		}

		public static void MovePlayerToCobwebActionPosition(Player pPlayer, Vector2 pOffsetOtherPosition)
		{
			pPlayer.MoveAgainstPoint(pPlayer.mCurrentActivity.rIObj.ActionPosition2 + pOffsetOtherPosition);
		}

		public static void MovePlayerToNearestActionPosition(Player pPlayer)
		{
			pPlayer.MoveAgainstPoint(pPlayer.mCurrentActivity.rIObj.NearestActionPosition(pPlayer.SkeletonPosition));
		}

		public static void MovePlayerToRightActionPosition(Player pPlayer, Nullable<Vector2> pOffsetGretel = null, float pSpeedFactor = 1f)
		{
			if (pPlayer.mCurrentActivity.rIObj == null) return;
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

		public static void Move(InteractiveObject pPlayerIObj, Vector2 pPosition)
		{
			pPlayerIObj.MoveInteractiveObject(pPosition);
		}

		public static void MovePlayer(Player pPlayer, Vector2 pPosition)
		{
			pPlayer.Move(pPosition - pPlayer.SkeletonPosition, new List<Rectangle>());
		} 

		public static void SetToPosition(InteractiveObject pPlayerIObj, Vector2 pPosition)
		{
			pPlayerIObj.MoveInteractiveObject(pPosition - pPlayerIObj.SkeletonPosition);
		}

		//Start Animation
		public static void StartAnimation(SpineObject pSpine, string pAnimation, bool pLoop = false)
		{
			pSpine.SetAnimation(pAnimation, pLoop, true);
		}

		//Animation Stepping
		public static void UpdateAnimationStepping(SpineObject pSpine, float pProgress)
		{
			if (pProgress > 0.9f)
				pProgress = 0.9f;
			if (pSpine.AnimationState.ToString() == "<none>") return;
			//if (pSpine.AnimationState.ToString() == "<none>") throw new Exception("SpineObjekt hat keinen AnimationState. TmpFix = return.");
			pSpine.AnimationState.GetCurrent(0).Time = pSpine.AnimationState.GetCurrent(0).EndTime * pProgress;
		}

		//Movement Stepping
		public static void UpdateMovementStepping(InteractiveObject pPlayerIObj, float pProgress, Vector2 pSource, Vector2 pDestination)
		{
			if (pPlayerIObj.GetType() == typeof(Player))
				MovePlayer((Player)pPlayerIObj, pSource + (pDestination - pSource) * pProgress);
			else
				SetToPosition(pPlayerIObj, pSource + (pDestination - pSource) * pProgress);
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
			SetToPosition(pPlayer, pSource + (pDestination - pSource) * Progress);
		}

		//ActivityInstruction & Progress Update
		public static void UpdateActIProgressBoth(SteppingProgress pProgress, ActivityInstruction pActI, Player pPlayer, Player pOtherPlayer, Vector2 pThumbstickDirection, bool AllowStepBack = true)
		{
			if (!Conditions.ActionThumbstickPressedBothPlayer(pPlayer, pOtherPlayer, pThumbstickDirection))
			{
				if (!Conditions.ActionThumbstickPressed(pPlayer, pThumbstickDirection))
					pActI.SetFadingState(pPlayer, true);
				else
					pActI.SetFadingState(pPlayer, false, false);
				if (!Conditions.ActionThumbstickPressed(pOtherPlayer, pThumbstickDirection))
					pActI.SetFadingState(pOtherPlayer, true);
				else
					pActI.SetFadingState(pOtherPlayer, false, false);
				if (AllowStepBack)
					pProgress.StepBackward();
			}
			else
			{
				pActI.SetFadingState(pPlayer, false, false);
				pActI.SetFadingState(pOtherPlayer, false, false);
				pProgress.StepForward();
			}
		}

		public static void UpdateActIProgressBothLegUp(SteppingProgress pProgress, ActivityInstruction pActI, Player pPlayer, Player pOtherPlayer, Vector2 pThumbstickDirection)
		{
			if (!Conditions.ActionThumbstickPressed(pPlayer, pThumbstickDirection) || !Conditions.ActionHold(pOtherPlayer))
			{
				if (!Conditions.ActionThumbstickPressed(pPlayer, pThumbstickDirection))
					pActI.SetFadingState(pPlayer, true);
				else
					pActI.SetFadingState(pPlayer, false, false);
				if (!Conditions.ActionHold(pOtherPlayer))
					pActI.SetFadingState(pOtherPlayer, true);
				else
					pActI.SetFadingState(pOtherPlayer, false, false);
				pProgress.StepBackward();
			}
			else
			{
				pActI.SetFadingState(pPlayer, false, false);
				pActI.SetFadingState(pOtherPlayer, false, false);
				pProgress.StepForward();
			}
		}

		//Set States
		public static void SetPlayerToIdle(Player pPlayer)
		{
			pPlayer.mCurrentActivity = ActivityHandler.None;
			pPlayer.mCurrentState = 0;
		}

		//Brunnen-Overlay ein-/ausblenden
		public static void SetWellOverlay()
		{

		}

		//EndeGelände
		public static void End()
		{
			//So ist das nicht passiert...
			throw new NotImplementedException();
		}

	}
}
