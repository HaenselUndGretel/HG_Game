using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class CaughtInSwamp : ActivityState
	{
		protected QuickTimeEvent QTE;

		public CaughtInSwamp(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			QTE = new QuickTimeEvent(pHansel.Input, pGretel.Input, false);
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (!m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.CaughtInSwamp) &&
				Conditions.Contains(pPlayer, rIObj)
				)
			{
				pPlayer.mCurrentActivity = this;
				pPlayer.mCurrentState = 10;
				return Activity.None;
			}
			if (m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.FreeFromSwamp) &&
				Conditions.ItemInInventory(pPlayer, pOtherPlayer, typeof(Branch)) &&
				Conditions.PlayerNearEnough(pPlayer, pOtherPlayer, 200f)
				)
				return Activity.FreeFromSwamp;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0: //FreeFromSwamp
					Sequences.StartAnimation(pPlayer, "attack");
					QTE.StartQTE();
					++pPlayer.mCurrentState;
					break;
				case 1:
					if (QTE.State == QuickTimeEvent.QTEState.Failed)
					{
						Sequences.SetPlayerToIdle(pPlayer);
						break;
					}
					if (QTE.State == QuickTimeEvent.QTEState.Successfull)
					{
						++pPlayer.mCurrentState;
						break;
					}
					QTE.Update();
					Sequences.UpdateAnimationStepping(pPlayer, QTE.Progress);
					break;
				case 2:
					if (!Conditions.Contains(pPlayer, rIObj) && !Conditions.Contains(pOtherPlayer, rIObj))
					{
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
						break;
					}
					Vector2 Source = new Vector2(rIObj.ActionRectList[0].Center.X, rIObj.ActionRectList[0].Center.Y);
					Sequences.MoveAway(pPlayer, Source);
					Sequences.MoveAway(pOtherPlayer, Source);
					break;
				case 10: //CaughtInSwamp
					Sequences.StartAnimation(pPlayer, "attack", true);
					++pPlayer.mCurrentState;
					break;
			}
		}

		#endregion
	}
}
