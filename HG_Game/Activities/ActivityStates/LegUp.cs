using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class LegUp : ActivityState
	{
		protected SteppingProgress Progress;
		public ActivityInstruction ActI;

		public LegUp(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			Progress = new SteppingProgress();
			ActI = new ActivityInstruction();
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (
				Conditions.NotHandicapped(pPlayer, Activity.LegUpGrab) &&
				Conditions.Contains(pPlayer, rIObj)
				)
			{
				if (!m2ndState)
					return Activity.LegUp;
				return Activity.LegUpGrab;
			}
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0:
					//-----Zu Position holden-----
					if (!m2ndState) //LegUp
						Progress.Reset(); //LegUp kann mehrfach ausgeführt werden
					if (!Conditions.ActionHold(pPlayer))
					{
						Sequences.SetPlayerToIdle(pPlayer);
						break;
					}
					if (Conditions.PlayersAtActionPositions(pPlayer, pOtherPlayer, Hardcoded.LegUp_StartOffsetGretel))
						++pPlayer.mCurrentState;
					Sequences.MovePlayerToRightActionPosition(pPlayer, Hardcoded.LegUp_StartOffsetGretel);
					break;
				case 1:
					Sequences.StartAnimation(pPlayer, Character.GetRightDirectionAnimation(rIObj.ActionPosition2 - rIObj.ActionPosition1, Hardcoded.Anim_LegUp_Raise_Up, "gibts nicht", Hardcoded.Anim_LegUp_Raise_Side));
					ActI.ThumbstickDirHansel = ActivityInstruction.ThumbstickDirection.Up;
					ActI.ThumbstickDirGretel = ActivityInstruction.ThumbstickDirection.None;
					++pPlayer.mCurrentState;
					break;
				case 2:
					//-----Hoch heben-----
					if (pPlayer.GetType() == typeof(Hansel))
					{
						Sequences.UpdateActIProgressBothLegUp(Progress, ActI, pPlayer, pOtherPlayer, new Vector2(0, -1));
						if (Progress.Progress <= 0f && !Conditions.ActionHold(pPlayer) && !Conditions.ActionHold(pOtherPlayer))
						{ //Abbrechbar
							Sequences.SetPlayerToIdle(pPlayer);
							Sequences.SetPlayerToIdle(pOtherPlayer);
						}
						Sequences.UpdateAnimationStepping(pPlayer, Progress.Progress);
						Sequences.UpdateAnimationStepping(pOtherPlayer, Progress.Progress);

						if (Progress.Complete)
						{ //GretelGrab am höchsten Punkt
							ActI.ThumbstickDirGretel = ActivityInstruction.ThumbstickDirection.Up;
							if (Conditions.ActionThumbstickPressed(pOtherPlayer, new Vector2(0, -1)))
							{ //Start Grab
								ActI.SetFadingState(pPlayer, false);
								ActI.SetFadingState(pOtherPlayer, false, false);
								++pPlayer.mCurrentState;
								pOtherPlayer.mCurrentState = pPlayer.mCurrentState;
							}
							else //Show ActI f Gretel
							{
								ActI.SetFadingState(pOtherPlayer, true);
							}
						}
						else
						{ //Nicht am höchsten Punkt: ActI.Gretel nicht Thumbstick anzeigen
							ActI.ThumbstickDirGretel = ActivityInstruction.ThumbstickDirection.None;
						}
					}
					break;
				case 3:
					//-----Gretel nächste Animation starten-----
					if (pPlayer.GetType() == typeof(Gretel))
					{
						if (!m2ndState) //LegUp
						{
							Sequences.StartAnimation(pPlayer, Character.GetRightDirectionAnimation(rIObj.ActionPosition2 - rIObj.ActionPosition1, Hardcoded.Anim_LegUp_Lift_Up, "gibts nicht", Hardcoded.Anim_LegUp_Lift_Side)); //hoch ziehen
							Sequences.StartAnimation(pOtherPlayer, Character.GetRightDirectionAnimation(rIObj.ActionPosition2 - rIObj.ActionPosition1, Hardcoded.Anim_LegUp_Lift_Up, "gibts nicht", Hardcoded.Anim_LegUp_Lift_Side));
						}
						else //LegUpGrab
						{
							//Hier wurde die Animation LegUpGrab gestartet
							rIObj.ActionRectList.Clear(); //LegUpGrab kann nicht mehrfach ausgeführt werden
						}
						++pPlayer.mCurrentState;
						pOtherPlayer.mCurrentState = pPlayer.mCurrentState;
					}
					break;
				case 4:
					//-----Gretel fertig?-----
					if (pPlayer.GetType() == typeof(Hansel) && Conditions.AnimationComplete(pOtherPlayer))
					{
						ActI.SetFadingState(pOtherPlayer, false);
						if (m2ndState) //LegUpGrab
						{
							//Gretel runter lassen
							Sequences.StartAnimation(pPlayer, Hardcoded.Anim_LegUp_Lower);
							Sequences.StartAnimation(pOtherPlayer, Hardcoded.Anim_LegUp_Lower);
						}
						++pPlayer.mCurrentState;
						pOtherPlayer.mCurrentState = pPlayer.mCurrentState;
					}
					break;
				case 5:
					//-----Beide fertig?-----
					if (Conditions.AnimationComplete(pPlayer))
					{
						if (!m2ndState && pPlayer.GetType() == typeof(Gretel))
						{
							Vector2 Offset;
							switch (GameReferenzes.SceneID)
							{
								case 0:
									Offset = Hardcoded.LegUp_OffsetGretel_0;
									break;
								case 4:
									Offset = Hardcoded.LegUp_OffsetGretel_4;
									break;
								case 7:
									Offset = Hardcoded.LegUp_OffsetGretel_7;
									break;
								case 10:
									Offset = Hardcoded.LegUp_OffsetGretel_10;
									break;
								case 13:
									Offset = Hardcoded.LegUp_OffsetGretel_13;
									break;
								default:
									throw new Exception("Für diese Scene gibt es kein LegUp");
							}
							Sequences.Move(pPlayer, Offset); //Bei LegUp Gretel hoch setzen
						}
						Sequences.SetPlayerToIdle(pPlayer);
					}
					break;
			}
			ActI.Update();
		}

		public override void Draw(SpriteBatch pSpriteBatch, Player pPlayer, Player pOtherPlayer)
		{
			Sequences.DrawActI(ActI, pSpriteBatch, pPlayer, pOtherPlayer);
		}

		#endregion
	}
}
