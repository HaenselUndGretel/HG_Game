using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	class CaughtInSwamp : ActivityState
	{
		int FreeProgressCounter;
		bool WalkAway;
		public bool HanselTrapped;
		public bool GretelTrapped;
		float MaxFreeDistance;

		public CaughtInSwamp(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			FreeProgressCounter = 0;
			WalkAway = false;
			HanselTrapped = false;
			GretelTrapped = false;
			MaxFreeDistance = 200f;
		}

		#region Override Methods

		public override Activity GetPossibleActivity(bool pContains)
		{
			if (m2ndState && (rHansel.Inventory.Contains(typeof(Branch)) || rGretel.Inventory.Contains(typeof(Branch))) && WithinMaxFreeDistance())
				return Activity.FreeFromSwamp;
			return Activity.None;
		}

		public override void PrepareAction(Player pPlayer)
		{
			FreeProgressCounter = 0;
			WalkAway = false;
			if (HanselTrapped || GretelTrapped)
				m2ndState = true;
			if (pPlayer.GetType() == typeof(Hansel))
			{
				mStateHansel = State.Starting;
			}
			else if (pPlayer.GetType() == typeof(Gretel))
			{
				mStateGretel = State.Starting;
			}
		}

		public override void StartAction(Player pPlayer)
		{
			if (pPlayer.GetType() == typeof(Hansel))
			{
				if (HanselTrapped)
				{
					pPlayer.mModel.SetAnimation("attack");
				}
				else
				{
					//ToDo Start Animation for QuickEvent Stepping.
				}
				mStateHansel = State.Running;
			}
			else if (pPlayer.GetType() == typeof(Gretel))
			{
				if (GretelTrapped)
				{
					pPlayer.mModel.SetAnimation("attack");
				}
				else
				{
					//ToDo Start Animation for QuickEvent Stepping.
				}
				mStateGretel = State.Running;
			}
		}

		public override void UpdateAction(Player pPlayer)
		{
			if (WalkAway && pPlayer.GetType() == typeof(Hansel)) //Von Netz entfernen (nur einmal pro Frame ausführen)
			{
				rHansel.MoveManually(new Vector2(0, 1));
				rGretel.MoveManually(new Vector2(0, 1));
				bool finished = true;
				foreach (Rectangle rect in rIObj.ActionRectList)
				{
					if (rect.Intersects(rHansel.CollisionBox) || rect.Intersects(rGretel.CollisionBox))
						finished = false;
				}
				if (finished)
				{
					rHansel.MoveManually(new Vector2(0, 1));
					rGretel.MoveManually(new Vector2(0, 1));
					rHansel.mCurrentActivity = new None();
					rGretel.mCurrentActivity = new None();
					mStateHansel = State.Idle;
					mStateGretel = State.Idle;
					HanselTrapped = false;
					GretelTrapped = false;
					m2ndState = false;
				}
				return;
			}

			if ((pPlayer.GetType() == typeof(Hansel) && !HanselTrapped) || (pPlayer.GetType() == typeof(Gretel) && !GretelTrapped))
			{
				if (pPlayer.Input.ActionJustPressed)
				{
					++FreeProgressCounter;
					//ToDo Step through QuickEvent Animation
					if (FreeProgressCounter > 10)
					{
						WalkAway = true;
					}
				}
			}
		}

		public bool WithinMaxFreeDistance()
		{
			return ((new Vector2(rHansel.CollisionBox.Center.X, rHansel.CollisionBox.Center.Y) - new Vector2(rGretel.CollisionBox.Center.X, rGretel.CollisionBox.Center.Y)).Length() <= MaxFreeDistance) ? true : false;
		}

		#endregion
	}
}
