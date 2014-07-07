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
					++pPlayer.mCurrentState;
					break;
				case 2:
					QTE.Update();
					if (pPlayer.GetType() == typeof(Hansel))
					{
						Sequences.UpdateMovementStepping(rIObj, QTE.Progress, mSourceIObj, mDestinationIObj);
						Sequences.UpdateMovementStepping(pPlayer, QTE.Progress, mSourceHansel, mDestinationHansel);
					}
					else
					{
						Sequences.UpdateMovementStepping(pPlayer, QTE.Progress, mSourceGretel, mDestinationGretel);
					}
					if (QTE.State == QuickTimeEvent.QTEState.Successfull)
					{
						Sequences.SetPlayerToIdle(pPlayer);
					}
					else if (QTE.State == QuickTimeEvent.QTEState.Failed)
					{
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
					}
					break;
			}
		}

		#endregion
	}
}
