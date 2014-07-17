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
	class SlipThroughRock : ActivityState
	{
		protected Vector2 Destination;

		public SlipThroughRock(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.NotHandicapped(pPlayer, Activity.SlipThroughRock) &&
				Conditions.Contains(pPlayer, rIObj) &&
				Conditions.ActivityNotInUseByOtherPlayer(pOtherPlayer, this)
				)
				return Activity.SlipThroughRock;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0:
					if (Conditions.PlayerAtNearestActionPosition(pPlayer))
						++pPlayer.mCurrentState;
					Sequences.MovePlayerToNearestActionPosition(pPlayer);
					break;
				case 1:
					Sequences.StartAnimation(pPlayer, "attack"); //Weg bewegen
					++pPlayer.mCurrentState;
					break;
				case 2:
					if (Conditions.AnimationComplete(pPlayer))
					{
						Destination = rIObj.DistantActionPosition(pPlayer.SkeletonPosition);
						pPlayer.IsVisible = false;
						++pPlayer.mCurrentState;
					}
					break;
				case 3:
					if (pPlayer.SkeletonPosition == Destination)
					{
						Sequences.StartAnimation(pPlayer, "attack");
						pPlayer.IsVisible = true;
						++pPlayer.mCurrentState;
						break;
					}
					Sequences.MoveToPosition(pPlayer, Destination);
					break;
				case 4:
					if (Conditions.AnimationComplete(pPlayer))
						Sequences.SetPlayerToIdle(pPlayer);
					break;
			}
		}

		#endregion
	}
}
