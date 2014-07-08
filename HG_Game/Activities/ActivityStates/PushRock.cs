using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class PushRock : ActivityState
	{
		protected QuickTimeEvent QTE;
		protected Vector2 mSourceHansel;
		protected Vector2 mSourceGretel;
		protected Vector2 mSourceIObj;
		protected Vector2 mDestinationHansel;
		protected Vector2 mDestinationGretel;
		protected Vector2 mDestinationIObj;

		protected const int RockMoveDistance = 256;
		protected float OldProgress;

		public PushRock(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			:base(pHansel, pGretel, pIObj)
		{
			QTE = new QuickTimeEvent(pHansel.Input, pGretel.Input, true, true);
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
					mSourceIObj = rIObj.SkeletonPosition; //new Vector2(rIObj.CollisionRectList[0].X, rIObj.CollisionRectList[0].Y);

					Vector2 ActionToCollisionRectDirection = new Vector2(rIObj.CollisionRectList[0].X - rIObj.ActionRectList[0].X, rIObj.CollisionRectList[0].Y - rIObj.ActionRectList[0].Y);

					Vector2 DestinationDelta;
					if (ActionToCollisionRectDirection.Y > 0)
						DestinationDelta = new Vector2(0, RockMoveDistance);
					else if (ActionToCollisionRectDirection.Y < 0)
						DestinationDelta = new Vector2(0, -RockMoveDistance);
					else if (ActionToCollisionRectDirection.X > 0)
						DestinationDelta = new Vector2(-RockMoveDistance, 0);
					else
						DestinationDelta = new Vector2(RockMoveDistance, 0);

					mDestinationIObj = rIObj.SkeletonPosition + DestinationDelta;
					mDestinationHansel = mSourceHansel + DestinationDelta;
					mDestinationGretel = mSourceGretel + DestinationDelta;

					++pPlayer.mCurrentState;
					break;
				case 2:
					// QTE Progress von 0.1 auf 0.5 für smoothere Rockbewegung.
					if (OldProgress < QTE.Progress)
						OldProgress += 0.01f;
					if (OldProgress >= QTE.Progress)
						QTE.Update();

					Sequences.UpdateMovementStepping(rIObj, OldProgress, mSourceIObj, mDestinationIObj);
					Sequences.UpdateMovementStepping(pPlayer, OldProgress, mSourceHansel, mDestinationHansel);
					Sequences.UpdateMovementStepping(pOtherPlayer, OldProgress, mSourceGretel, mDestinationGretel);

					if (QTE.State == QuickTimeEvent.QTEState.Successfull && OldProgress >= 1.0f)
					{
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
						rIObj.ActionRectList.Clear();
					}
					break;
			}
		}

		#endregion
	}
}
