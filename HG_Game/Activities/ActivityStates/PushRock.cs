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
		protected Vector2 AllDestination;
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
					mSourceIObj = new Vector2(rIObj.CollisionRectList[0].X, rIObj.CollisionRectList[0].Y);

					Vector2 direction = new Vector2(rIObj.CollisionRectList[0].X - rIObj.ActionRectList[0].X, rIObj.CollisionRectList[0].Y - rIObj.ActionRectList[0].Y);

					if (direction.Y > 0)
						AllDestination = new Vector2(0, RockMoveDistance);
					else if (direction.Y < 0)
						AllDestination = new Vector2(0, -RockMoveDistance);
					else if (direction.X > 0)
						AllDestination = new Vector2(-RockMoveDistance, 0);
					else
						AllDestination = new Vector2(RockMoveDistance, 0);

					++pPlayer.mCurrentState;
					break;
				case 2:
					// QTE Progress von 0.1 auf 0.5 für smoothere Rockbewegung.
					OldProgress = QTE.Progress;
					QTE.Update();

					if(OldProgress >= 1.0f)
					{
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
						rIObj.ActionRectList.Clear();
					}

					if (OldProgress != QTE.Progress)
					{
						Sequences.UpdateMovementStepping(rIObj, 0.1f, mSourceIObj, AllDestination);
						Sequences.UpdateMovementStepping(pPlayer, 0.1f, mSourceHansel, AllDestination);
						Sequences.UpdateMovementStepping(pOtherPlayer, 0.1f, mSourceGretel, AllDestination);
					}
					break;
			}
		}

		#endregion
	}
}
