using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class UseAmulet : ActivityState
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
				Conditions.ActivityNotInUseByOtherPlayer(pOtherPlayer, this) &&
				!pPlayer.Lantern &&
				m2ndState
				)
				return Activity.UseAmulet;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0:
					//-----Animation starten-----
					Sequences.StartAnimation(pPlayer, "attack");
					++pPlayer.mCurrentState;
					break;
				case 1:
					//-----Amulett hoch halten-----
					Sequences.UpdateActIProgress(Progress, ActI, pPlayer, new Vector2(0, -1), false);
					Sequences.UpdateAnimationStepping(pPlayer, Progress.Progress);
					//Abbrechbar
					if (Progress.Progress <= 0f && !Conditions.ActionHold(pPlayer) && !Conditions.ActionHold(pOtherPlayer))
						Sequences.SetPlayerToIdle(pPlayer);
					if (Progress.Complete)
					{
						Sequences.SetPlayerToIdle(pPlayer);
						ActI.SetFadingState(pPlayer, false);
						//Delete Witch
					}
					break;
			}
			ActI.Update();
		}

		#endregion

	}
}
