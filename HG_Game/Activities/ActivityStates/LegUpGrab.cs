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
		protected float OldProgress;
		protected Vector2 mStartOffsetGretel = new Vector2(55, -20);

		public LegUpGrab(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			QTE = new QuickTimeEvent(pHansel.Input, pGretel.Input, false, true, true, true);
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (!m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.LegUpGrab) &&
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
					if (OldProgress < QTE.Progress)
						OldProgress += 0.01f;
					if (OldProgress >= QTE.Progress)
						QTE.Update();

					Sequences.UpdateAnimationStepping(pPlayer, OldProgress);
					Sequences.UpdateAnimationStepping(pOtherPlayer, OldProgress);

					if (QTE.State == QuickTimeEvent.QTEState.Failed)
					{
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
					}
					if (QTE.State == QuickTimeEvent.QTEState.Successfull && OldProgress >= 1.0f)
					{
						++pPlayer.mCurrentState;
						++pOtherPlayer.mCurrentState;
					}
					break;
				case 3:
					QTE.SetToGretelGrab();
					++pPlayer.mCurrentState;
					break;
				case 4:
					QTE.Update();
					if (pPlayer.GetType() == typeof(Gretel) && (QTE.State == QuickTimeEvent.QTEState.Finished || QTE.State == QuickTimeEvent.QTEState.Successfull))
					{
						Sequences.StartAnimation(pPlayer, "attack"); //Item greifen
						m2ndState = true;
						++pPlayer.mCurrentState;
						++pOtherPlayer.mCurrentState;
						QTE.SetToGretelGrabbed();
					}
					break;
				case 5:
					if (Conditions.AnimationComplete(pPlayer) && Conditions.AnimationComplete(pOtherPlayer))
					{
						//Gretel runter lassen
						Sequences.StartAnimation(pPlayer, "attack");
						Sequences.StartAnimation(pOtherPlayer, "attack");
						++pPlayer.mCurrentState;
						++pOtherPlayer.mCurrentState;
					}
					break;
				case 6:
					if (Conditions.AnimationComplete(pPlayer))
						Sequences.SetPlayerToIdle(pPlayer);
					break;
			}
		}

		#endregion
	}
}
