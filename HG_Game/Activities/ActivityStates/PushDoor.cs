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
			if (Conditions.NotHandicapped(pPlayer, Activity.PushDoor) &&
				Conditions.Contains(pPlayer, rIObj)
				)
			{
				if (!m2ndState)
					return Activity.PushDoor;
				return Activity.PullDoor;
			}
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
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
					Vector2 ActionToCollisionRectDirection = new Vector2(rIObj.CollisionRectList[0].X - rIObj.ActionRectList[0].X, rIObj.CollisionRectList[0].Y - rIObj.ActionRectList[0].Y);

					int AnimationDirection;
					if (ActionToCollisionRectDirection.Y > 0)
					{
						AnimationDirection = 0;
						ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Down);
					}
					else if (ActionToCollisionRectDirection.Y < 0)
					{
						AnimationDirection = 1;
						ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Up);
					}
					else if (ActionToCollisionRectDirection.X > 0)
					{
						ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Right);
						AnimationDirection = 2;
					}
					else
					{
						AnimationDirection = 3;
						ActI.SetThumbstickDirBoth(ActivityInstruction.ThumbstickDirection.Left);
					}
					//Passende Animation entsprechend AnimationDirection & Push-/PullDoor starten
					Sequences.StartAnimation(pPlayer, "attack");
					Sequences.StartAnimation(pOtherPlayer, "attack");
					++pPlayer.mCurrentState;
					++pOtherPlayer.mCurrentState;
					break;
				case 2:
					if (pPlayer.GetType() == typeof(Hansel))
					{
						Sequences.UpdateActIProgressBoth(Progress, ActI, pPlayer, pOtherPlayer, new Vector2(rIObj.CollisionRectList[0].X - rIObj.ActionRectList[0].X, rIObj.CollisionRectList[0].Y - rIObj.ActionRectList[0].Y), false);
						Sequences.UpdateAnimationStepping(rIObj, Progress.Progress);
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
						rIObj.CollisionRectList.Clear();
						rIObj.ActionRectList.Clear();
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
