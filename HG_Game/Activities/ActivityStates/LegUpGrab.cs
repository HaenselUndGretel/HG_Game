using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class LegUpGrab : ActivityState
	{
		public QuickTimeEvent QTE;
		protected Vector2 mStartOffsetGretel = new Vector2(55, -20);

		public LegUpGrab(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			QTE = new QuickTimeEvent(pHansel.Input, pGretel.Input, false, true, true, true);
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.NotHandicapped(pPlayer, Activity.LegUpGrab) &&
				Conditions.Contains(pPlayer, rIObj)
				)
				return Activity.LegUpGrab;
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
						++pPlayer.mCurrentState;
					}
					break;
				case 3:
					if (pPlayer.GetType() == typeof(Gretel) && Conditions.ActionPressed(pPlayer))
					{
						Sequences.StartAnimation(pPlayer, "attack");
						++pPlayer.mCurrentState;
						++pOtherPlayer.mCurrentState;
					}
					break;
				case 4:
					if (Conditions.AnimationComplete(pPlayer) && Conditions.AnimationComplete(pOtherPlayer))
					{
						Sequences.StartAnimation(pPlayer, "attack");
						Sequences.StartAnimation(pOtherPlayer, "attack");
						++pPlayer.mCurrentState;
						++pOtherPlayer.mCurrentState;
					}
					break;
				case 5:
					if (Conditions.AnimationComplete(pPlayer))
						Sequences.SetPlayerToIdle(pPlayer);
					break;
			}
		}

		#endregion
	}
}
