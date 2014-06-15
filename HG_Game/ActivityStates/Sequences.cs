﻿using HanselAndGretel.Data;
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

		public static void SetToPosition(Player pPlayer, Vector2 pPosition)
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
		public static void StartAnimation(Player pPlayer, bool pLoop)
		{

		}

		public static void StartAnimation(InteractiveObject pIObj, bool pLoop)
		{

		}

		//Animation Stepping
		public static void UpdateAnimationStepping(Player pPlayer, float pProgress)
		{

		}

		public static void UpdateAnimationStepping(InteractiveObject pIObj, float pProgress)
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
		public static void SynchMovementToAnimation(AnimationState pAnimationState, Player pPlayer, Vector2 pDestination)
		{

		}

		//QuickTimeEvent

		//Set States

		//Gretel States setzen (Brunnen)

		//Set Arrow States

		//Rock & Arrow Menu Navigation

		//Overlay ein-/ausblenden

		//Stuff (BrunnenKorb) bewegen

		//EndeGelände

	}
}
