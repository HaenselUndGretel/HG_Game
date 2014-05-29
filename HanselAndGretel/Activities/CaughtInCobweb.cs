using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	class CaughtInCobweb : ActivityState
	{
		int FreeProgressCounter;
		Player TrappedPlayer;
		bool WalkAway;

		public CaughtInCobweb(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			FreeProgressCounter = 0;
			WalkAway = false;
		}

		#region Override Methods

		public override Activity GetPossibleActivity(bool pContains)
		{
			if (m2ndState && (rHansel.Inventory.Contains(typeof(Knife)) || rGretel.Inventory.Contains(typeof(Knife))))
				return Activity.FreeFromCobweb;
			return Activity.None;
		}

		public override void PrepareAction(Player pPlayer)
		{
			if (!m2ndState) //Spieler trappen
			{
				pPlayer.Position = rIObj.ActionPosition1;
				TrappedPlayer = pPlayer;
				FreeProgressCounter = 0;
				WalkAway = false;
				m2ndState = true;
				if (pPlayer.GetType() == typeof(Hansel))
				{
					mStateHansel = State.Starting;
				}
				else if (pPlayer.GetType() == typeof(Gretel))
				{
					mStateGretel = State.Starting;
				}
				return;
			}

			//Wenn der Spieler an der passenden Position ist und X drückt.
			if (pPlayer.Position == rIObj.ActionPosition2 && pPlayer.Input.ActionIsPressed)
			{
				if (pPlayer.GetType() == typeof(Hansel))
				{
					mStateHansel = State.Starting;
				}
				else if (pPlayer.GetType() == typeof(Gretel))
				{
					mStateGretel = State.Starting;
				}
				return;
			}

			if (pPlayer.GetType() == typeof(Hansel))
			{
				if (!rHansel.Input.ActionIsPressed)
				{
					rHansel.mCurrentActivity = new None();
					mStateHansel = State.Idle;
					return;
				}
				//Spieler bewegen
				rHansel.MoveAgainstPoint(rIObj.ActionPosition2);
			}
			else if (pPlayer.GetType() == typeof(Gretel))
			{
				if (!rGretel.Input.ActionIsPressed)
				{
					rGretel.mCurrentActivity = new None();
					mStateGretel = State.Idle;
					return;
				}
				//Spieler bewegen
				rGretel.MoveAgainstPoint(rIObj.ActionPosition2);
			}
			else
			{
				throw new Exception("Nicht existenter Spielername!");
			}
		}

		public override void StartAction(Player pPlayer)
		{
			if (pPlayer.GetType() == typeof(Hansel))
			{
				if (pPlayer == TrappedPlayer)
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
				if (pPlayer == TrappedPlayer)
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
					m2ndState = false;
				}
				return;
			}
			if (pPlayer != TrappedPlayer)
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

		#endregion
	}
}
