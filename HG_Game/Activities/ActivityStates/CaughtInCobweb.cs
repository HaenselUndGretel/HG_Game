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
		protected QuickTimeEvent QTE;

		public CaughtInCobweb(InteractiveObject pIObj)
			: base(pIObj)
		{

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
					Sequences.StartAnimation(pPlayer.mModel, "attack");
					QTE.StartQTE();
					++pPlayer.mCurrentState;
					break;
				case 1:
					if (QTE.State == QuickTimeEvent.QTEState.Failed)
					{

						break;
					}
					if (QTE.State == QuickTimeEvent.QTEState.Successfull)
					{
						++pPlayer.mCurrentState;
						break;
					}
					QTE.Update();
					Sequences.UpdateAnimationStepping(pPlayer.mModel, QTE.Progress);
					break;
				case 2:
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
					Sequences.StartAnimation(pPlayer.mModel, "attack", true);
					break;
			}
		}

		#endregion
	}
}
