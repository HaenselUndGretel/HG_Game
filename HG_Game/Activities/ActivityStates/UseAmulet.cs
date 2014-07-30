using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class UseAmulet : ActivityState
	{
		protected SteppingProgress Progress;
		public ActivityInstruction ActI;

		public UseAmulet(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			Progress = new SteppingProgress();
			ActI = new ActivityInstruction();
			ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Up);
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.NotHandicapped(pPlayer, Activity.UseAmulet) &&
				Conditions.Contains(pPlayer, rIObj)
				)
				return Activity.UseAmulet;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0:
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
					//Amulett hoch halten
					Sequences.StartAnimation(pPlayer, "attack");
					Sequences.StartAnimation(pOtherPlayer, "attack");
					++pPlayer.mCurrentState;
					++pOtherPlayer.mCurrentState;
					break;
				case 2:
					if (pPlayer.GetType() == typeof(Hansel))
					{
						Sequences.UpdateActIProgressBoth(Progress, ActI, pPlayer, pOtherPlayer, new Vector2(0, -1), false);
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

		#endregion
	}
}
