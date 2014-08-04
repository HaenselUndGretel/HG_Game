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
	class PushDoor : ActivityState
	{
		protected SteppingProgress Progress;
		public ActivityInstruction ActI;

		public PushDoor(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			Progress = new SteppingProgress();
			ActI = new ActivityInstruction();
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.Contains(pPlayer, rIObj))
			{
				if (!m2ndState && Conditions.NotHandicapped(pPlayer, Activity.PushDoor))
					return Activity.PushDoor;
				if (Conditions.NotHandicapped(pPlayer, Activity.PullDoor))
					return Activity.PullDoor;
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
					//-----Richtung bestimmen-----
					Vector2 ActionToCollisionRectDirection = new Vector2(rIObj.CollisionRectList[0].X - rIObj.ActionRectList[0].X, rIObj.CollisionRectList[0].Y - rIObj.ActionRectList[0].Y);

					if (ActionToCollisionRectDirection.Y > 0)
						ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Down);
					else if (ActionToCollisionRectDirection.Y < 0)
						ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Up);
					else if (ActionToCollisionRectDirection.X > 0)
						ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Right);
					else
						ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Left);

					//Passende Animation entsprechend AnimationDirection & Push-/PullDoor starten
					if (!m2ndState) //PushDoor
					{
						Sequences.AnimateAccordingToDirection(pPlayer, ActionToCollisionRectDirection, Hardcoded.Anim_PushDoor_Up, Hardcoded.Anim_PushDoor_Down, Hardcoded.Anim_PushDoor_Side);
						Sequences.AnimateAccordingToDirection(pOtherPlayer, ActionToCollisionRectDirection, Hardcoded.Anim_PushDoor_Up, Hardcoded.Anim_PushDoor_Down, Hardcoded.Anim_PushDoor_Side);
						//Sequences.AnimateAccordingToDirection(rIObj, ActionToCollisionRectDirection, Hardcoded.Anim_Door_Open_Up, Hardcoded.Anim_Door_Open_Down, Hardcoded.Anim_Door_Open_Side);
					}
					else //PullDoor
					{
						Sequences.AnimateAccordingToDirection(pPlayer, ActionToCollisionRectDirection, Hardcoded.Anim_PullDoor_Up, Hardcoded.Anim_PullDoor_Down, Hardcoded.Anim_PullDoor_Side);
						Sequences.AnimateAccordingToDirection(pOtherPlayer, ActionToCollisionRectDirection, Hardcoded.Anim_PullDoor_Up, Hardcoded.Anim_PullDoor_Down, Hardcoded.Anim_PullDoor_Side);
						//Sequences.AnimateAccordingToDirection(rIObj, ActionToCollisionRectDirection, Hardcoded.Anim_Door_Close_Up, Hardcoded.Anim_Door_Close_Down, Hardcoded.Anim_Door_Close_Side);
					}
					++pPlayer.mCurrentState;
					pOtherPlayer.mCurrentState = pPlayer.mCurrentState;
					break;
				case 2:
					//-----Tür bewegen-----
					if (pPlayer.GetType() == typeof(Hansel))
					{
						Sequences.UpdateActIProgressBoth(Progress, ActI, pPlayer, pOtherPlayer, new Vector2(rIObj.CollisionRectList[0].X - rIObj.ActionRectList[0].X, rIObj.CollisionRectList[0].Y - rIObj.ActionRectList[0].Y), false);
						//Sequences.UpdateAnimationStepping(rIObj, Progress.Progress);
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
