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
	public class PushDoor : ActivityState
	{
		protected SteppingProgress Progress;
		public ActivityInstruction ActI;

		public PushDoor(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			Progress = new SteppingProgress();
			ActI = new ActivityInstruction();
			ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Down);
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.Contains(pPlayer, rIObj))
			{
				if (Conditions.NotHandicapped(pPlayer, Activity.PushDoor) &&
					(pPlayer.Lantern || pOtherPlayer.Lantern)
					)
					return Activity.PushDoor;
			}
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0:
					//-----Zu Position holden-----
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
					Sequences.StartAnimation(pPlayer, Hardcoded.Anim_PushDoor);
					Sequences.StartAnimation(pOtherPlayer, Hardcoded.Anim_PushDoor);
					Sequences.StartAnimation(rIObj, Hardcoded.Anim_Door_Open);
					++pPlayer.mCurrentState;
					pOtherPlayer.mCurrentState = pPlayer.mCurrentState;
					break;
				case 2:
					//-----Tür bewegen-----
					if (pPlayer.GetType() == typeof(Hansel))
					{
						Sequences.UpdateActIProgressBoth(Progress, ActI, pPlayer, pOtherPlayer, new Vector2(0, 1), false);
						Sequences.UpdateAnimationStepping(rIObj, Progress.Progress);
						Sequences.UpdateAnimationStepping(pPlayer, Progress.Progress);
						Sequences.UpdateAnimationStepping(pOtherPlayer, Progress.Progress);
					}

					if (Progress.Complete)
					{
						Sequences.SetPlayerToIdle(pPlayer);
						Sequences.SetPlayerToIdle(pOtherPlayer);
						ActI.SetFadingState(pPlayer, false);
						ActI.SetFadingState(pOtherPlayer, false);
						rIObj.CollisionRectList.Clear();
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
