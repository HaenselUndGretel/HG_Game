﻿using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class PullDoor : ActivityState
	{
		protected QuickTimeEvent QTE;
		protected Vector2 mSourceHansel;
		protected Vector2 mSourceGretel;
		protected Vector2 mDestinationHansel;
		protected Vector2 mDestinationGretel;

		protected float OldProgress;
		protected Vector2 AnimationDirection;

		public PullDoor(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			QTE = new QuickTimeEvent(pHansel.Input, pGretel.Input, true, true);
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.NotHandicapped(pPlayer, Activity.PullDoor) &&
				Conditions.Contains(pPlayer, rIObj)
				)
				return Activity.PullDoor;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0:
					if (!Conditions.ActionHold(pPlayer))
						Sequences.SetPlayerToIdle(pPlayer);
					if (Conditions.PlayersAtActionPositions(pPlayer, pOtherPlayer))
						++pPlayer.mCurrentState;
					Sequences.MovePlayerToRightActionPosition(pPlayer);
					break;
				case 1:
					QTE.StartQTE();
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

					Vector2 direction = new Vector2(rIObj.CollisionRectList[0].X - rIObj.ActionRectList[0].X, rIObj.CollisionRectList[0].Y - rIObj.ActionRectList[0].Y);

					if (direction.Y > 0)
						AnimationDirection = new Vector2(0, 1);
					else if (direction.Y < 0)
						AnimationDirection = new Vector2(0, -1);
					else if (direction.X > 0)
						AnimationDirection = new Vector2(-1, 0);
					else
						AnimationDirection = new Vector2(1, 0);

					++pPlayer.mCurrentState;
					break;
				case 2:
					OldProgress = QTE.Progress;
					QTE.Update();

					if(OldProgress >= 1.0f)
					{
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
						rIObj.CollisionRectList.Clear();
						rIObj.ActionRectList.Clear();
					}

					if (OldProgress != QTE.Progress)
					{
						Sequences.UpdateAnimationStepping(rIObj, QTE.Progress);
						Sequences.UpdateAnimationStepping(pPlayer, QTE.Progress);
						Sequences.UpdateAnimationStepping(pOtherPlayer, QTE.Progress);
					}
					//QTE.Update();
					//if (pPlayer.GetType() == typeof(Hansel))
					//{
					//	Sequences.UpdateAnimationStepping(rIObj, QTE.Progress);
					//	Sequences.UpdateMovementStepping(pPlayer, QTE.Progress, mSourceHansel, mDestinationHansel);
					//}
					//else
					//{
					//	Sequences.UpdateMovementStepping(pPlayer, QTE.Progress, mSourceGretel, mDestinationGretel);
					//}
					//if (QTE.State == QuickTimeEvent.QTEState.Successfull)
					//{
					//	Sequences.SetPlayerToIdle(pPlayer);
					//}
					//else if (QTE.State == QuickTimeEvent.QTEState.Failed)
					//{
					//	Sequences.SetPlayerToIdle(pPlayer);
					//	Sequences.SetPlayerToIdle(pOtherPlayer);
					//}
					break;
			}
		}

		#endregion
	}
}
