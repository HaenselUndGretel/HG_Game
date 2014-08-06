using HanselAndGretel.Data;
using KryptonEngine.Entities;
using KryptonEngine.FModAudio;
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
					//-----Zu Position bewegen-----
					if (Conditions.PlayerAtNearestActionPosition(pPlayer))
						++pPlayer.mCurrentState;
					Sequences.MovePlayerToNearestActionPosition(pPlayer);
					break;
				case 1:
					//-----Animation starten-----
					pPlayer.IsVisible = false; //Weg bewegen
					++pPlayer.mCurrentState;
					break;
				case 2:
					//-----Weg bewegt?-----
					if (Conditions.AnimationComplete(pPlayer))
					{
						FmodMediaPlayer.Instance.AddSong("gretel_crawl", 0.8f);
						Destination = rIObj.DistantActionPosition(pPlayer.SkeletonPosition);
						pPlayer.IsVisible = false;
						++pPlayer.mCurrentState;
					}
					break;
				case 3:
					//-----Durch Felsspalt bewegen-----
					if (pPlayer.SkeletonPosition == Destination)
					{
						pPlayer.IsVisible = true; //Wieder "auftauchen"
						pPlayer.IsVisible = true;
						++pPlayer.mCurrentState;
						break;
					}
					Sequences.MoveToPosition(pPlayer, Destination, 1f, true);
					break;
				case 4:
					//-----Wieder komplett da?-----
					if (Conditions.AnimationComplete(pPlayer))
						Sequences.SetPlayerToIdle(pPlayer);
					break;
			}
		}

		#endregion
	}
}
