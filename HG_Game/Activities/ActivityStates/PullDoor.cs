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
		public QuickTimeEvent QTE;
		protected Vector2 mSourceHansel;
		protected Vector2 mSourceGretel;

		protected float OldProgress;

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

					Vector2 ActionToCollisionRectDirection = new Vector2(rIObj.CollisionRectList[0].X - rIObj.ActionRectList[0].X, rIObj.CollisionRectList[0].Y - rIObj.ActionRectList[0].Y);

					int AnimationDirection;
					if (ActionToCollisionRectDirection.Y > 0)
						AnimationDirection = 0;
					else if (ActionToCollisionRectDirection.Y < 0)
						AnimationDirection = 1;
					else if (ActionToCollisionRectDirection.X > 0)
						AnimationDirection = 2;
					else
						AnimationDirection = 3;

					//Passende Animation entsprechend AnimationDirection starten
					Sequences.StartAnimation(pPlayer, "attack");
					Sequences.StartAnimation(pOtherPlayer, "attack");
					++pPlayer.mCurrentState;
					++pOtherPlayer.mCurrentState;
					break;
				case 2:
					if (OldProgress < QTE.Progress)
						OldProgress += 0.01f;
					if (OldProgress >= QTE.Progress)
						QTE.Update();

					Sequences.UpdateAnimationStepping(rIObj, OldProgress);
					Sequences.UpdateAnimationStepping(pPlayer, OldProgress);
					Sequences.UpdateAnimationStepping(pOtherPlayer, OldProgress);

					if (QTE.State == QuickTimeEvent.QTEState.Successfull && OldProgress >= 1.0f)
					{
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
						rIObj.CollisionRectList.Clear();
						rIObj.ActionRectList.Clear();
					}
					break;
			}
		}

		#endregion
	}
}
