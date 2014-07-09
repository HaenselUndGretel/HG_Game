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
		protected float OldProgress;
		protected bool pLeaveUp = false;
		protected Vector2 StartOffsetActionRectangle2 = new Vector2(0, -200);
		public ActivityState CaughtPlayerSwitchItem;

		public CaughtInCobweb(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			QTE = new QuickTimeEvent(pHansel.Input, pGretel.Input, false, false, true);
			CaughtPlayerSwitchItem = ActivityHandler.None;
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (!m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.CaughtInCobweb)
				)
			{
				m2ndState = true;
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
					if (Conditions.PlayerAtCobwebActionPosition(pPlayer, StartOffsetActionRectangle2))
						++pPlayer.mCurrentState;
					foreach (Rectangle rect in rIObj.ActionRectList)
					{
						if (rect.Contains(pPlayer.CollisionRectList[0]))
						{
							if (rect.Contains(new Point((int)rIObj.ActionPosition2.X, (int)rIObj.ActionPosition2.Y)))
								Sequences.MovePlayerToCobwebActionPosition(pPlayer, Vector2.Zero);
							else
								Sequences.MovePlayerToCobwebActionPosition(pPlayer, StartOffsetActionRectangle2);
						}
					}
					break;
				case 1:
					Sequences.StartAnimation(pPlayer, "attack");
					//Leave Up or Down?
					float ActionToCollisionRectY = 0f;
					foreach (Rectangle rect in rIObj.ActionRectList)
					{
						if (pPlayer.CollisionBox.Intersects(rect))
							ActionToCollisionRectY = rIObj.CollisionRectList[0].Y - rect.Y;
					}

					if (ActionToCollisionRectY > 0f)
						pLeaveUp = true;
					else
						pLeaveUp = false;

					if (pPlayer.GetType() == typeof(Hansel))
						QTE.OnlyOnePlayerIsHansel = true;
					else
						QTE.OnlyOnePlayerIsHansel = false;
					QTE.StartQTE();
					++pPlayer.mCurrentState;
					break;
				case 2:
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
				case 3:
					if (!Conditions.CobwebIntersects(pPlayer, rIObj) && !Conditions.CobwebIntersects(pOtherPlayer, rIObj))
					{
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
						m2ndState = false;
					}
					Sequences.MoveUpDown(pPlayer, pLeaveUp);
					Sequences.MoveUpDown(pOtherPlayer, pLeaveUp);
					break;
				case 10: //CaughtInCobeweb
					Sequences.SetToPosition(pPlayer, rIObj.ActionPosition1);
					Sequences.StartAnimation(pPlayer, "attack", true);
					OldProgress = 0f;
					++pPlayer.mCurrentState;
					break;
				case 11:
					//Inventar im Netz öffnen / schließen
					if (pPlayer.Input.SwitchItemJustPressed)
					{
						if (CaughtPlayerSwitchItem.GetType() == typeof(SwitchItem))
						{ //Schließen
							CaughtPlayerSwitchItem = ActivityHandler.None;
						}
						else
						{ //Öffnen
							if (pPlayer.GetType() == typeof(Hansel))
								CaughtPlayerSwitchItem = new SwitchItem((Hansel)pPlayer, (Gretel)pOtherPlayer, true);
							else
								CaughtPlayerSwitchItem = new SwitchItem((Hansel)pOtherPlayer, (Gretel)pPlayer, true);
						}
					}
					if (pPlayer.Input.BackJustPressed) //Schließen
						CaughtPlayerSwitchItem = ActivityHandler.None;
					CaughtPlayerSwitchItem.Update(pPlayer, pOtherPlayer);
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
