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

		}

		public static void MovePlayerToRightActionPosition(Player pPlayer, Nullable<Vector2> pOffsetGretel = null)
		{
			//Hansel->AP1 Gretel->AP2
			//Wenn pOffsetGretel nicht null ist muss Gretel an AP1 + pOffsetGretel gesetzt werden.
		}

		public static void SetPlayerToPosition(Player pPlayer, Vector2 pPosition)
		{

		}

		//Move für Cobweb
		public static void MoveUpDown(Player pPlayer, bool pUp, float pSpeedFactor = 1f)
		{

		}

		//Move für Swamp
		public static void MoveAway(Player pPlayer, Vector2 pSource, float pSpeedFactor = 1f)
		{

		}

		//Start Animation
		public static void StartAnimation(SpineObject pSpine, string pAnimation, bool pLoop = false)
		{

		}

		//Animation Stepping
		public static void UpdateAnimationStepping(SpineObject pSpine, float pProgress)
		{

		}

		//Movement Stepping
		public static void UpdateMovementStepping(Player pPlayer, float pProgress, Vector2 pDestination)
		{

		}

		public static void UpdateMovementStepping(InteractiveObject pIObj, float pProgress, Vector2 pDestination)
		{

		}

		//Pause & Play Animation
		public static void PausePlayAnimation(AnimationState pAnimationState, bool pPlay)
		{

		}

		//Player während Animation bewegen
		public static void SynchMovementToAnimation(SpineObject pSpineToSynchTo, Player pPlayer, Vector2 pDestination, Vector2 pSource)
		{

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

		}

	}
}
