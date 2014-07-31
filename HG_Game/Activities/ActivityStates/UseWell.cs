using HanselAndGretel.Data;
using KryptonEngine.Entities;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class UseWell : ActivityState
	{
		protected SteppingProgress Progress;
		public ActivityInstruction ActI;

		public UseWell(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			:base(pHansel, pGretel, pIObj)
		{
			Progress = new SteppingProgress();
			ActI = new ActivityInstruction();
			ActI.SetThumbstickDir(pHansel, ActivityInstruction.ThumbstickDirection.Rotate);
			ActI.SetThumbstickDir(pGretel, ActivityInstruction.ThumbstickDirection.None);
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.NotHandicapped(pPlayer, Activity.UseWell) &&
				Conditions.AtRightSideOfWell(pPlayer, rIObj)
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
					{
						Sequences.SetPlayerToIdle(pPlayer);
						break;
					}
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
						pPlayer.SetAnimation(Hardcoded.Anim_Well_GrabWind_Hansel); //An Kurbel
					else
						pPlayer.SetAnimation(Hardcoded.Anim_Well_Enter_Gretel, false); //In Korb steigen
					++pPlayer.mCurrentState;
					break;
				case 3:
					//-----Gretel bereit / im Korb?-----
					if (pOtherPlayer.mCurrentState < 3)
						break;
					bool finished = false;
					if (pPlayer.GetType() == typeof(Gretel)) //Gretel im Korb
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
						if (pPlayer.Input.ActionIsPressed && pPlayer.Input.LeftStickRotation != 0f && pOtherPlayer.Input.ActionIsPressed) //Brunnen wird bewegt?
						{
							if (pPlayer.Input.LeftStickRotation > 0f)
							{
								pPlayer.SetAnimation(Hardcoded.Anim_Well_WindDown_Hansel); //Brunnen runter lassen Animation
								pOtherPlayer.SetAnimation(Hardcoded.Anim_Well_Hang_Gretel);
							}
							else if (pPlayer.Input.LeftStickRotation < 0f)
							{
								pPlayer.SetAnimation(Hardcoded.Anim_Well_WindUp_Hansel); //Brunnen hoch ziehen Animation
								pOtherPlayer.SetAnimation(Hardcoded.Anim_Well_Hang_Gretel);
							}
							ActI.SetFadingState(pPlayer, false, false);
							ActI.SetFadingState(pOtherPlayer, false, false);
							Progress.StepFromRotation(pPlayer.Input.LeftStickRotation, Hardcoded.UseWell_ProgressPerRotation, Hardcoded.UseWell_UpRotationFrictionFactor);
						}
						else
						{
							pPlayer.SetAnimation(Hardcoded.Anim_Well_WindIdle_Hansel); //Brunnen idle Animation
							pOtherPlayer.SetAnimation(Hardcoded.Anim_Well_Idle_Gretel);
							if (pPlayer.Input.ActionIsPressed && pPlayer.Input.LeftStickRotation != 0f) //Hansel versucht Brunnen zu bewegen?
								ActI.SetFadingState(pPlayer, false, false);
							else
								ActI.SetFadingState(pPlayer, true, false);
							if (pOtherPlayer.Input.ActionIsPressed)
								ActI.SetFadingState(pOtherPlayer, false, false);
							else
								ActI.SetFadingState(pOtherPlayer, true);
						}

						if (Progress.Progress <= 0f && pPlayer.Input.LeftStickRotation < 0f)
						{
							ActI.SetFadingState(pPlayer, false);
							ActI.SetFadingState(pOtherPlayer, false);
							Progress.Reset();
							pPlayer.SetAnimation(); //Hansel von Kurbel weg nehmen
							pPlayer.mCurrentState = 10;
							pOtherPlayer.mCurrentState = 8;
						}
						if (Progress.Progress >= 1f && pPlayer.Input.LeftStickRotation > 0f)
						{
							ActI.SetFadingState(pPlayer, false);
							ActI.SetFadingState(pOtherPlayer, false);
							Progress.Reset(true);
							pOtherPlayer.mCurrentState = 8;
						}
						Sequences.UpdateAnimationStepping(rIObj, Progress.Progress);
						Sequences.UpdateMovementStepping(pOtherPlayer, Progress.Progress, Conditions.WellActionPosition2Up(rIObj), rIObj.ActionPosition2);
					}
					break;
				case 8:
					//-----Oben/Unten aussteigen-----
					pPlayer.SetAnimation(Hardcoded.Anim_Well_Leave_Gretel, false); // Aus Eimer aussteigen
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
							Progress.Reset();
							m2ndState = false;
						}
					}
					break;
			}
			ActI.Update();
		}

		public void UpdateOverlay(ref List<InteractiveObject> pRenderList)
		{
			if (Progress.Progress > Hardcoded.UseWell_ShowOverlayProgressBarrier)
			{
				Sequences.SetWellOverlay(rIObj.SkeletonPosition, false, ref pRenderList); //Hide Overlay
				return;
			}
			Sequences.SetWellOverlay(rIObj.SkeletonPosition, true, ref pRenderList); //Display Overlay
		}

		public override void Draw(SpriteBatch pSpriteBatch, Player pPlayer, Player pOtherPlayer)
		{
			Sequences.DrawActI(ActI, pSpriteBatch, pPlayer, pOtherPlayer);
		}

		#endregion
	}
}
