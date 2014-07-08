using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class CaughtInCobweb : ActivityState
	{
		public QuickTimeEvent QTE;

		public CaughtInCobweb(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			QTE = new QuickTimeEvent(pHansel.Input, pGretel.Input, false, false, true);
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (!m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.CaughtInCobweb)
				)
			{
				pPlayer.mCurrentActivity = this;
				pPlayer.mCurrentState = 10;
				return Activity.None;
			}
			if (m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.FreeFromCobweb) &&
				Conditions.ItemInOwnHand(pPlayer, typeof(Knife))
				)
				return Activity.FreeFromCobweb;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0: //FreeFromCobweb
					if (Conditions.PlayerAtActionPosition(pPlayer, true))
						++pPlayer.mCurrentState;
					Sequences.MovePlayerToActionPosition(pPlayer, true);
					break;
				case 1:
					Sequences.StartAnimation(pPlayer, "attack");
					if (pPlayer.GetType() == typeof(Hansel))
						QTE.OnlyOnePlayerIsHansel = true;
					else
						QTE.OnlyOnePlayerIsHansel = false;
					QTE.StartQTE();
					++pPlayer.mCurrentState;
					break;
				case 2:
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
				case 3:
					if (!Conditions.Contains(pPlayer, rIObj) && !Conditions.Contains(pOtherPlayer, rIObj))
					{
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
						break;
					}
					Sequences.MoveUpDown(pPlayer, false);
					Sequences.MoveUpDown(pOtherPlayer, false);
					break;
				case 10: //CaughtInCobeweb
					Sequences.StartAnimation(pPlayer, "attack", true);
					++pPlayer.mCurrentState;
					break;
			}
		}

		#endregion
	}
}
