using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class UseWell : ActivityState
	{
		protected const float BucketSpeed = 0.2f; //Wieviel der Gesamtstrecke mit einer Controllerumdrehung zurück gelegt wird
		protected float BucketState;

		public UseWell(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			:base(pHansel, pGretel, pIObj)
		{
			BucketState = 0;
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.NotHandicapped(pPlayer, Activity.UseWell)
				)
				return Activity.UseWell;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0:
					//-----Zu Positionen holden-----
					if (!Conditions.ActionHold(pPlayer))
						Sequences.SetPlayerToIdle(pPlayer);
					//Wenn Spieler an passenden Positionen sind
					if ((!m2ndState && Conditions.PlayersAtWellPositions(pPlayer, pOtherPlayer, false)) ||
						(m2ndState && Conditions.PlayersAtWellPositions(pPlayer, pOtherPlayer, true))
						)
						++pPlayer.mCurrentState;
					//Spieler zu passenden Positionen bewegen
					if (m2ndState) //Gretel im Brunnen zum Korb bewegen
						Sequences.MovePlayerToWellActionPosition(pPlayer, true);
					else
						Sequences.MovePlayerToWellActionPosition(pPlayer, false);
					break;
				case 1:
					//Nur gemeinsam starten
					if (pOtherPlayer.mCurrentState > 0)
						++pPlayer.mCurrentState;
					break;
				case 2:
					//-----Animationen starten (Hansel an Kurbel bereit machen, Gretel in Korb steigen)-----
					m2ndState = true;
					if (pPlayer.GetType() == typeof(Hansel))
						pPlayer.SetAnimation("attack");
					else
						pPlayer.SetAnimation("attack", false);
					++pPlayer.mCurrentState;
					break;
				case 3:
					//-----Gretel bereit / im Korb?-----
					if (pOtherPlayer.mCurrentState < 3)
						break;
					bool finished = false;
					if (pPlayer.GetType() == typeof(Gretel))
						finished = Conditions.AnimationComplete(pPlayer);
					else
						finished = Conditions.AnimationComplete(pOtherPlayer);
					if (finished)
						++pPlayer.mCurrentState;
					break;
				case 4:
					//-----Brunnen hochziehen / herablassen-----
					if (pOtherPlayer.mCurrentState != 4)
						break;
					if (pPlayer.GetType() == typeof(Hansel))
					{
						BucketState += pPlayer.Input.LeftStickRotation * BucketSpeed;
						if (pPlayer.Input.LeftStickRotation > 0f)
							pPlayer.SetAnimation("attack"); //Brunnen runter lassen Animation
						else if (pPlayer.Input.LeftStickRotation < 0f)
							pPlayer.SetAnimation("attack"); //Brunnen hoch ziehen Animation
						else
							pPlayer.SetAnimation("attack"); //Brunnen idle Animation
					}
					if (BucketState <= 0f)
					{
						BucketState = 0f;
						pPlayer.SetAnimation(); //Hansel von Kurbel weg nehmen
						pPlayer.mCurrentState = 10;
						pOtherPlayer.mCurrentState = 8;
					}
					if (BucketState >= 1f)
					{
						BucketState = 1f;
						pOtherPlayer.mCurrentState = 8;
					}
					Sequences.UpdateAnimationStepping(rIObj, BucketState);
					Sequences.UpdateMovementStepping(pOtherPlayer, BucketState, new Vector2(rIObj.ActionPosition2.X - rIObj.ActionPosition1.X, 0), rIObj.ActionPosition2);
					break;
				case 8:
					//-----Oben/Unten aussteigen-----
					pPlayer.SetAnimation("attack", false); // Aus Eimer aussteigen
					++pPlayer.mCurrentState;
					break;
				case 9:
					if (Conditions.AnimationComplete(pPlayer))
					{
						pPlayer.SetAnimation();
						++pPlayer.mCurrentState;
					}
					break;
				case 10:
					//-----States setzen-----
					if (Conditions.AnimationComplete(pPlayer))
					{
						Sequences.SetPlayerToIdle(pPlayer);
						if (pPlayer.GetType() == typeof(Hansel))
						{
							BucketState = 0f;
							m2ndState = false;
						}
					}
					break;


			}
		}

		#endregion
	}
}
