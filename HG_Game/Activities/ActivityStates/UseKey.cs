using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class UseKey : ActivityState
	{

		public UseKey(InteractiveObject pIObj)
			: base(pIObj)
		{
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (!m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.UseKey) &&
				Conditions.Contains(pPlayer, rIObj) &&
				Conditions.ItemInOwnHand(pPlayer, typeof(Key)) &&
				Conditions.ActivityNotInUseByOtherPlayer(pOtherPlayer, this)
				)
				return Activity.UseKey;
			if (m2ndState &&
				Conditions.NotHandicapped(pPlayer, Activity.PushDoor) &&
				Conditions.Contains(pPlayer, rIObj)
				)
				return Activity.PushDoor;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			if (!m2ndState) //UseKey
			{
				switch (pPlayer.mCurrentState)
				{
					case 0:
						Sequences.StartAnimation(pPlayer, "attack");
						++pPlayer.mCurrentState;
						break;
					case 1:
						if (Conditions.AnimationComplete(pPlayer.mModel))
							Sequences.SetPlayerToIdle(pPlayer);
						break;
				}
			}
			else //PushDoor
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
