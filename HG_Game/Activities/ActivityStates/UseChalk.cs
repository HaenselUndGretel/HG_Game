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
		protected ChalkMenu RockMenu;
		protected ChalkMenu ArrowMenu;
		public List<string> rRockData;

		protected HudFading FadingRockMenu;
		protected HudFading FadingArrowMenu;

		protected Vector2 OffsetRockMenu = new Vector2(-100, -150);
		protected Vector2 OffsetArrowMenu = new Vector2(-130, -250);

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
				Conditions.Contains(pPlayer, rIObj)
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
					FadingRockMenu.ShowHudGretel = true;
					FadingArrowMenu.ShowHudGretel = false;
					RockMenu.SetRockMenu(pPlayer.SkeletonPosition + OffsetRockMenu, rRockData);
					++pPlayer.mCurrentState;
					break;
				case 2:
					//RockMenu schließen
					if (pPlayer.Input.BackJustPressed)
					{
						Sequences.SetPlayerToIdle(pPlayer);
						FadingRockMenu.ShowHudGretel = false;
						FadingArrowMenu.ShowHudGretel = false;
					}
					//ArrowMenu öffnen
					if (Conditions.EnoughChalk(pPlayer) && pPlayer.Input.ActionJustPressed && rRockData.Count < 3)
						++pPlayer.mCurrentState;
					break;
				case 3:
					FadingArrowMenu.ShowHudGretel = true;
					ArrowMenu.SetArrowMenu(pPlayer.SkeletonPosition + OffsetArrowMenu);
					++pPlayer.mCurrentState;
					break;
				case 4:
					//ArrowMenu schließen
					if (pPlayer.Input.BackJustPressed)
						pPlayer.mCurrentState = 1;
					int SelectedArrow = ArrowMenu.Update(pPlayer);
					if (SelectedArrow != -1)
					{
						RockMenu.AddButtonToRockMenu(SelectedArrow);
						rRockData.Add(RockMenu.GetRockMenuDataString(SelectedArrow));
						--((Gretel)pPlayer).Chalk;
						++pPlayer.mCurrentState;
						FadingRockMenu.ShowHudGretel = false;
						FadingArrowMenu.ShowHudGretel = false;
					}
					break;
				case 5:
					if (FadingRockMenu.VisibilityGretel > 0f)
						break;
					pPlayer.SetAnimation("attack", false); //Pfeil an Fels malen
					++pPlayer.mCurrentState;
					break;
				case 6:
					if (pPlayer.AnimationComplete)
						Sequences.SetPlayerToIdle(pPlayer);
					break;
			}
		}

		public void DrawMenues(SpriteBatch pSpriteBatch)
		{
			FadingRockMenu.Update();
			FadingArrowMenu.Update();
			RockMenu.Draw(pSpriteBatch, FadingRockMenu.VisibilityGretel);
			ArrowMenu.Draw(pSpriteBatch, FadingArrowMenu.VisibilityGretel);
		}

		#endregion
	}
}
