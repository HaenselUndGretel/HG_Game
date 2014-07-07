using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class JumpOverGap : ActivityState
	{
		protected Vector2 mDestination;
		protected Vector2 mSource;

		public JumpOverGap(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			mDestination = Vector2.Zero;
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.NotHandicapped(pPlayer, Activity.JumpOverGap)
				&& Conditions.Contains(pPlayer, rIObj))
				return Activity.JumpOverGap;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0:
					Sequences.StartAnimation(pPlayer, "attack");
					mDestination = rIObj.DistantActionPosition(pPlayer.Position);
					mSource = pPlayer.Position;
					++pPlayer.mCurrentState;
					break;
				case 1:
					Sequences.SynchMovementToAnimation(rIObj, pPlayer, mSource, mDestination);
					if (Conditions.AnimationComplete(pPlayer))
						Sequences.SetPlayerToIdle(pPlayer);
					break;
			}
		}

		#endregion
	}
}
