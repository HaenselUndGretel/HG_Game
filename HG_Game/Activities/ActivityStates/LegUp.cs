using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class LegUp : ActivityState
	{
		protected QuickTimeEvent QTE;
		protected Vector2 mStartOffsetGretel = new Vector2(55, -20);
		protected Vector2 mOffsetGretel = new Vector2(-20, -200);

		public LegUp(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			QTE = new QuickTimeEvent(pHansel.Input, pGretel.Input, false, true, true, true);
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.NotHandicapped(pPlayer, Activity.LegUp) &&
				Conditions.Contains(pPlayer, rIObj)
				)
				return Activity.LegUp;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0:
					if (!Conditions.ActionHold(pPlayer))
						Sequences.SetPlayerToIdle(pPlayer);
					if (Conditions.PlayersAtActionPositions(pPlayer, pOtherPlayer, mStartOffsetGretel))
						++pPlayer.mCurrentState;
					Sequences.MovePlayerToRightActionPosition(pPlayer, mStartOffsetGretel);
					break;
				case 1:
					Sequences.StartAnimation(pPlayer, "attack");
					QTE.StartQTE();
					++pPlayer.mCurrentState;
					break;
				case 2:
					QTE.Update();
					Sequences.UpdateAnimationStepping(pPlayer, QTE.Progress);
					Sequences.UpdateAnimationStepping(pOtherPlayer, QTE.Progress);
					if (QTE.State == QuickTimeEvent.QTEState.Failed)
					{
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
					}
					else if (QTE.State == QuickTimeEvent.QTEState.Successfull)
					{
						if (pPlayer.GetType() == typeof(Gretel))
							Sequences.SetPlayerToPosition(pPlayer, pPlayer.SkeletonPosition + mOffsetGretel);
						Sequences.SetPlayerToIdle(pPlayer);
					}
					break;
			}
		}

		#endregion
	}
}
