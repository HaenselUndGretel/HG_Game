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
	public class ChargeAmulet : ActivityState
	{
		protected SteppingProgress Progress;
		public ActivityInstruction ActI;

		public ChargeAmulet(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			Progress = new SteppingProgress();
			ActI = new ActivityInstruction();
			ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Up);
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.NotHandicapped(pPlayer, Activity.ChargeAmulet) &&
				Conditions.Contains(pPlayer, rIObj)
				)
				return Activity.ChargeAmulet;
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
					if (Conditions.PlayersAtActionPositions(pPlayer, pOtherPlayer))
						++pPlayer.mCurrentState;
					Sequences.MovePlayerToRightActionPosition(pPlayer);
					break;
				case 1:
					//-----Animation starten-----
					Sequences.StartAnimation(pPlayer, "attack");
					Sequences.StartAnimation(pOtherPlayer, "attack");
					++pPlayer.mCurrentState;
					++pOtherPlayer.mCurrentState;
					break;
				case 2:
					//-----Amulett hoch halten-----
					if (pPlayer.GetType() == typeof(Hansel))
					{
						Sequences.UpdateActIProgressBoth(Progress, ActI, pPlayer, pOtherPlayer, new Vector2(0, -1));
						//Sequences.UpdateAnimationStepping(rIObj, Progress.Progress);
						Sequences.UpdateAnimationStepping(pPlayer, Progress.Progress);
					}
					else
					{
						Sequences.UpdateAnimationStepping(pPlayer, Progress.Progress);
					}

					if (Progress.Complete)
					{
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
						ActI.SetFadingState(pPlayer, false);
						ActI.SetFadingState(pOtherPlayer, false);
						rIObj.ActionRectList.Clear();
						m2ndState = true;
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
