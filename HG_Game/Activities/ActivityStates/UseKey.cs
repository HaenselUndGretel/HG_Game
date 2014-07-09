using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class UseKey : ActivityState
	{
		public QuickTimeEvent QTE;
		protected float OldProgress;

		public UseKey(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			QTE = new QuickTimeEvent(pHansel.Input, pGretel.Input, true, true);
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (!m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.UseKey) &&
				Conditions.Contains(pPlayer, rIObj) &&
				Conditions.ItemInOwnHand(pPlayer, typeof(Key)) &&
				Conditions.ActivityNotInUseByOtherPlayer(pOtherPlayer, this)
				)
				return Activity.UseKey;
			if (m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.PushDoor) &&
				Conditions.Contains(pPlayer, rIObj)
				)
				return Activity.PushDoor;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			if (!m2ndState) //UseKey
			{
				switch (pPlayer.mCurrentState)
				{
					case 0:
						if (Conditions.PlayerAtActionPosition(pPlayer))
							++pPlayer.mCurrentState;
						Sequences.MovePlayerToActionPosition(pPlayer);
						break;
					case 1:
						Sequences.StartAnimation(pPlayer, "attack");
						++pPlayer.mCurrentState;
						break;
					case 2:
						if (Conditions.AnimationComplete(pPlayer))
						{
							Sequences.SetPlayerToIdle(pPlayer);
							m2ndState = true;
						}
						break;
				}
			}
			else //PushDoor
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

						Vector2 ActionToCollisionRectDirection = new Vector2(rIObj.CollisionRectList[0].X - rIObj.ActionRectList[0].X, rIObj.CollisionRectList[0].Y - rIObj.ActionRectList[0].Y);

						int AnimationDirection;
						if (ActionToCollisionRectDirection.Y > 0)
							AnimationDirection = 0;
						else if (ActionToCollisionRectDirection.Y < 0)
							AnimationDirection = 1;
						else if (ActionToCollisionRectDirection.X > 0)
							AnimationDirection = 2;
						else
							AnimationDirection = 3;
						//Passende Animation entsprechend AnimationDirection starten
						Sequences.StartAnimation(pPlayer, "attack");
						Sequences.StartAnimation(pOtherPlayer, "attack");
						++pPlayer.mCurrentState;
						++pOtherPlayer.mCurrentState;
						break;
					case 2:
						if (OldProgress < QTE.Progress)
							OldProgress += 0.01f;
						if (OldProgress >= QTE.Progress)
							QTE.Update();

						if (pPlayer.GetType() == typeof(Hansel))
						{
							Sequences.UpdateAnimationStepping(rIObj, OldProgress);
							Sequences.UpdateAnimationStepping(pPlayer, OldProgress);
						}
						else
						{
							Sequences.UpdateAnimationStepping(pPlayer, OldProgress);
						}

						if (QTE.State == QuickTimeEvent.QTEState.Successfull && OldProgress >= 1.0f)
						{
							Sequences.SetPlayerToIdle(pPlayer);
							Sequences.SetPlayerToIdle(pOtherPlayer);
							rIObj.CollisionRectList.Clear();
							rIObj.ActionRectList.Clear();
						}
						break;
				}
			}
		}

		#endregion
	}
}
