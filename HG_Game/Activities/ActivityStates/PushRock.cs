﻿using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class PushRock : ActivityState
	{
		protected Vector2 mSourceHansel;
		protected Vector2 mSourceGretel;
		protected Vector2 mSourceIObj;
		protected Vector2 mDestinationHansel;
		protected Vector2 mDestinationGretel;
		protected Vector2 mDestinationIObj;

		protected SteppingProgress Progress;
		public ActivityInstruction ActI;

		public PushRock(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			:base(pHansel, pGretel, pIObj)
		{
			Progress = new SteppingProgress(Hardcoded.PushRock_SteppingDuration);
			ActI = new ActivityInstruction();
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.NotHandicapped(pPlayer, Activity.PushRock) &&
				Conditions.Contains(pPlayer, rIObj)
				)
				return Activity.PushRock;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0:
					//-----Zu Positionen holden-----
					if (!Conditions.ActionHold(pPlayer))
					{
						Sequences.SetPlayerToIdle(pPlayer);
						break;
					}
					if (Conditions.PlayersAtActionPositions(pPlayer, pOtherPlayer))
						++pPlayer.mCurrentState;
					Sequences.MovePlayerToRightActionPosition(pPlayer);
					break;
				case 1:
					//-----Richtung bestimmen-----
					if (m2ndState)
					{
						++pPlayer.mCurrentState;
						break;
					}
					m2ndState = true;
					if (pPlayer.GetType() == typeof(Hansel))
					{
						mSourceHansel = pPlayer.SkeletonPosition;
						mSourceGretel = pOtherPlayer.SkeletonPosition;
					}
					else
					{
						mSourceHansel = pOtherPlayer.SkeletonPosition;
						mSourceGretel = pPlayer.SkeletonPosition;
					}
					mSourceIObj = rIObj.SkeletonPosition; //new Vector2(rIObj.CollisionRectList[0].X, rIObj.CollisionRectList[0].Y);

					Vector2 ActionToCollisionRectDirection = new Vector2(rIObj.CollisionRectList[0].X - rIObj.ActionRectList[0].X, rIObj.CollisionRectList[0].Y - rIObj.ActionRectList[0].Y);

					Vector2 DestinationDelta;
					if (ActionToCollisionRectDirection.Y > 0)
					{
						DestinationDelta = new Vector2(0, Hardcoded.PushRock_RockMoveDistance);
						ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Down);
					}
					else if (ActionToCollisionRectDirection.Y < 0)
					{
						DestinationDelta = new Vector2(0, -Hardcoded.PushRock_RockMoveDistance);
						ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Up);
					}
					else if (ActionToCollisionRectDirection.X > 0)
					{
						DestinationDelta = new Vector2(Hardcoded.PushRock_RockMoveDistance, 0);
						ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Right);
					}
					else
					{
						DestinationDelta = new Vector2(-Hardcoded.PushRock_RockMoveDistance, 0);
						ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Left);
					}

					mDestinationIObj = rIObj.SkeletonPosition + DestinationDelta;
					mDestinationHansel = mSourceHansel + DestinationDelta;
					mDestinationGretel = mSourceGretel + DestinationDelta;

					//Passende Animation entsprechend AnimationDirection starten
					Sequences.AnimateAccordingToDirection(pPlayer, ActionToCollisionRectDirection, Hardcoded.Anim_PushRock_Up, Hardcoded.Anim_PushRock_Down, Hardcoded.Anim_PushRock_Side);
					Sequences.AnimateAccordingToDirection(pOtherPlayer, ActionToCollisionRectDirection, Hardcoded.Anim_PushRock_Up, Hardcoded.Anim_PushRock_Down, Hardcoded.Anim_PushRock_Side);
					++pPlayer.mCurrentState;
					pOtherPlayer.mCurrentState = pPlayer.mCurrentState;
					break;
				case 2:
					//-----Fels bewegen-----
					Sequences.UpdateActIProgressBoth(Progress, ActI, pPlayer, pOtherPlayer, mDestinationIObj - mSourceIObj, false);
					if (Progress.Progress >= 0f && !Conditions.ActionHold(pPlayer) && !Conditions.ActionHold(pOtherPlayer))
					{ //Abbrechbar
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
					}
					if (pPlayer.GetType() == typeof(Hansel))
					{
						Sequences.UpdateMovementStepping(rIObj, Progress.Progress, mSourceIObj, mDestinationIObj);
						Sequences.UpdateMovementStepping(pPlayer, Progress.Progress, mSourceHansel, mDestinationHansel);
						Sequences.UpdateMovementStepping(pOtherPlayer, Progress.Progress, mSourceGretel, mDestinationGretel);
						Sequences.UpdateAnimationStepping(pPlayer, Progress.Progress);
						Sequences.UpdateAnimationStepping(pOtherPlayer, Progress.Progress);
					}
					if (Progress.Complete)
					{
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
						ActI.SetFadingState(pPlayer, false);
						ActI.SetFadingState(pOtherPlayer, false);
						rIObj.ActionRectList.Clear();
					}
					break;
			}
			ActI.Update();
		}

		public override void Draw(SpriteBatch pSpriteBatch, Player pPlayer, Player pOtherPlayer)
		{
			Sequences.DrawActI(ActI, pSpriteBatch, pPlayer, pOtherPlayer);
		}

		#endregion
	}
}
