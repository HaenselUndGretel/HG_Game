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
		public QuickTimeEvent QTE;
		protected float OldProgress;

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
				m2ndState = true;
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
					if (OldProgress < QTE.Progress)
						OldProgress += 0.01f;
					if (OldProgress >= QTE.Progress)
						QTE.Update();

					Sequences.UpdateAnimationStepping(pPlayer, OldProgress);

					if (QTE.State == QuickTimeEvent.QTEState.Failed)
						Sequences.SetPlayerToIdle(pPlayer);
					if (QTE.State == QuickTimeEvent.QTEState.Successfull && OldProgress >= 1.0f)
					{
						++pPlayer.mCurrentState;
						++pOtherPlayer.mCurrentState;
					}
					break;
				case 2:
					if (!Conditions.Contains(pPlayer, rIObj) && !Conditions.Contains(pOtherPlayer, rIObj))
					{
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
						m2ndState = false;
						break;
					}
					Vector2 Source = new Vector2(rIObj.ActionRectList[0].Center.X, rIObj.ActionRectList[0].Center.Y);
					Sequences.MoveAway(pPlayer, Source);
					Sequences.MoveAway(pOtherPlayer, Source);
					break;
				case 10: //CaughtInSwamp
					Sequences.StartAnimation(pPlayer, "attack", true);
					OldProgress = 0f;
					++pPlayer.mCurrentState;
					break;
				case 11:
					//if (Conditions.AnimationComplete(pPlayer))
						//Sequences.End();
					break;
				case 12:
					//Spieler führt nicht mehr zu Spielende während er raus bewegt wird
					break;
			}
		}

		#endregion
	}
}
