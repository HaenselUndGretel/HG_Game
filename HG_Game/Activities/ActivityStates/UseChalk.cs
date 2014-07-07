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
	class UseChalk : ActivityState
	{
		public enum ChalkState
		{
 			Idle,
			RockMenu,
			ArrowMenu
		}

		public ChalkState MenuState;
		protected ChalkMenu RockMenu;
		protected ChalkMenu ArrowMenu;
		public List<string> rRockData;

		protected HudFading FadingRockMenu;
		protected HudFading FadingArrowMenu;

		protected Vector2 OffsetArrowMenu;
		protected Vector2 OffsetRockMenu;

		public UseChalk(Hansel pHansel, Gretel pGretel, InteractiveObject pIObj)
			: base(pHansel, pGretel, pIObj)
		{
			RockMenu = new ChalkMenu();
			ArrowMenu = new ChalkMenu();
			FadingRockMenu = new HudFading();
			FadingArrowMenu = new HudFading();
		}

		#region Override Methods

		public override Activity GetPossibleActivity(Player pPlayer, Player pOtherPlayer)
		{
			if (Conditions.NotHandicapped(pPlayer, Activity.UseChalk) &&
				Conditions.Contains(pPlayer, rIObj) &&
				Conditions.EnoughChalk(pPlayer)
				)
				return Activity.UseChalk;
			return Activity.None;
		}

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0:
					if (Conditions.PlayerAtActionPosition(pPlayer))
						++pPlayer.mCurrentState;
					Sequences.MovePlayerToActionPosition(pPlayer);
					break;
				case 1:
					MenuState = ChalkState.RockMenu;
					FadingRockMenu.ShowHudGretel = true;
					FadingArrowMenu.ShowHudGretel = false;
					RockMenu.SetRockMenu(Vector2.Zero, 80f, rRockData);
					if (pPlayer.Input.BackJustPressed)
						Sequences.SetPlayerToIdle(pPlayer);
					if (pPlayer.Input.ActionJustPressed && rRockData.Count < 3)
						++pPlayer.mCurrentState;
					break;
				case 2:
					MenuState = ChalkState.ArrowMenu;
					FadingArrowMenu.ShowHudGretel = true;
					ArrowMenu.SetArrowMenu(Vector2.Zero, 80f);
					if (pPlayer.Input.BackJustPressed)
						--pPlayer.mCurrentState;
					int SelectedArrow = ArrowMenu.Update(pPlayer);
					if (SelectedArrow != 0)
					{
						RockMenu.AddButtonToRockMenu(SelectedArrow);
						rRockData.Add(RockMenu.GetRockMenuDataString(SelectedArrow));
						MenuState = ChalkState.Idle;
						FadingRockMenu.ShowHudGretel = false;
						FadingArrowMenu.ShowHudGretel = false;
						pPlayer.SetAnimation("attack", false); //Pfeil an Fels malen
						++pPlayer.mCurrentState;
					}
					break;
				case 3:
					if (pPlayer.AnimationComplete)
						Sequences.SetPlayerToIdle(pPlayer);
					break;
			}
		}

		public void DrawMenues(SpriteBatch pSpriteBatch)
		{
			RockMenu.Draw(pSpriteBatch, FadingRockMenu.VisibilityGretel);
			ArrowMenu.Draw(pSpriteBatch, FadingArrowMenu.VisibilityGretel);
		}

		#endregion
	}
}
