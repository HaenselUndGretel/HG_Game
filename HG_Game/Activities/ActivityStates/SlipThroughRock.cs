using HanselAndGretel.Data;
using KryptonEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class SlipThroughRock : ActivityState
	{
		public SlipThroughRock(InteractiveObject pIObj)
			: base(pIObj)
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
					Sequences.StartAnimation(pPlayer.mModel, "Attack"); //Weg bewegen
					++pPlayer.mCurrentState;
					break;
				case 1:
					if (Conditions.AnimationComplete(pPlayer.mModel))
					{
						Sequences.SetPlayerToPosition(pPlayer, rIObj.DistantActionPosition(pPlayer.Position));
						Sequences.StartAnimation(pPlayer.mModel, "attack");
						++pPlayer.mCurrentState;
					}
					break;
				case 2:
					if (Conditions.AnimationComplete(pPlayer.mModel))
						Sequences.SetPlayerToIdle(pPlayer);
					break;
			}
		}

		#endregion
	}
}
