using HanselAndGretel.Data;
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
		protected Vector2 mStartOffsetGretel = new Vector2(55, -20);
		protected Vector2 mOffsetGretel = new Vector2(-20, -255);

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
					if (!m2ndState) //LegUp
						Progress.Reset(); //LegUp kann mehrfach ausgeführt werden
					if (!Conditions.ActionHold(pPlayer))
					{
						Sequences.SetPlayerToIdle(pPlayer);
						break;
					}
					if (Conditions.PlayersAtActionPositions(pPlayer, pOtherPlayer, mStartOffsetGretel))
						++pPlayer.mCurrentState;
					Sequences.MovePlayerToRightActionPosition(pPlayer, mStartOffsetGretel);
					break;
				case 1:
					Sequences.StartAnimation(pPlayer, "attack");
					ActI.ThumbstickDirHansel = ActivityInstruction.ThumbstickDirection.Up;
					ActI.ThumbstickDirGretel = ActivityInstruction.ThumbstickDirection.None;
					++pPlayer.mCurrentState;
					break;
				case 2: //Hoch heben
					if (pPlayer.GetType() == typeof(Hansel))
					{
						Sequences.UpdateActIProgressBoth(Progress, ActI, pPlayer, pOtherPlayer, new Vector2(0, -1));
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
								++pOtherPlayer.mCurrentState;
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
					if (pPlayer.GetType() == typeof(Gretel))
					{
						if (!m2ndState) //LegUp
						{
							Sequences.StartAnimation(pPlayer, "attack"); //hoch ziehen
						}
						else //LegUpGrab
						{
							Sequences.StartAnimation(pPlayer, "attack"); //Item greifen
							rIObj.ActionRectList.Clear(); //LegUp kann mehrfach ausgeführt werden?
						}
						++pPlayer.mCurrentState;
						++pOtherPlayer.mCurrentState;
					}
					break;
				case 4:
					if (pPlayer.GetType() == typeof(Hansel) && Conditions.AnimationComplete(pOtherPlayer))
					{
						ActI.SetFadingState(pOtherPlayer, false);
						//Gretel runter lassen
						Sequences.StartAnimation(pPlayer, "attack");
						Sequences.StartAnimation(pOtherPlayer, "attack");
						++pPlayer.mCurrentState;
						++pOtherPlayer.mCurrentState;
					}
					break;
				case 5:
					if (Conditions.AnimationComplete(pPlayer))
					{
						if (!m2ndState && pPlayer.GetType() == typeof(Gretel))
							Sequences.Move(pPlayer, mOffsetGretel); //Bei LegUp Gretel hoch setzen
						Sequences.SetPlayerToIdle(pPlayer);
					}
					break;
			}
			ActI.Update();
		}

		public override void Draw(SpriteBatch pSpriteBatch, Player pPlayer, Player pOtherPlayer)
		{
			if (pPlayer.GetType() == typeof(Hansel))
				ActI.Draw(pSpriteBatch, (Hansel)pPlayer, (Gretel)pOtherPlayer);
			else
				ActI.Draw(pSpriteBatch, (Hansel)pOtherPlayer, (Gretel)pPlayer);
		}

		#endregion
	}
}
