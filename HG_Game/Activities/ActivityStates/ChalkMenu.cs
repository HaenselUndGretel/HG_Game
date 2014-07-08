using HanselAndGretel.Data;
using KryptonEngine.Controls;
using KryptonEngine.Entities;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class ChalkMenu
	{
		#region Properties

		protected Vector2 Position;
		protected Texture2D mBackground;
		protected List<ImageButton> mButtons;
		protected int SelectedIndex;
		protected bool RockMenu;
		protected const float ButtonSpacing = 60f;

		#endregion

		#region Getter & Setter

		#endregion

		#region Constructor

		public ChalkMenu()
		{
			Initialize();
		}

		#endregion

		#region Override Methods

		public void Initialize()
		{
			RockMenu = false;
			SelectedIndex = 0;
		}

		public int Update(Player pPlayer)
		{
			//Vor Update-Aufruf auf Arrow oder Rock Menu setzen
			mButtons[SelectedIndex].IsSelected = false;

			if (pPlayer.Input.ButtonJustPressed(Buttons.DPadRight))
				++SelectedIndex;
			else if (pPlayer.Input.ButtonJustPressed(Buttons.DPadLeft))
				--SelectedIndex;

			if (SelectedIndex < 0)
				SelectedIndex = mButtons.Count - 1;
			if (SelectedIndex > mButtons.Count - 1)
				SelectedIndex = 0;

			mButtons[SelectedIndex].IsSelected = true;

			if (pPlayer.Input.ActionJustPressed)
				return SelectedIndex;
			return -1;
		}

		#endregion

		#region Methods

		public void SetArrowMenu(Vector2 pMenuPosition)
		{
			Position = pMenuPosition;
			mBackground = TextureManager.Instance.GetElementByString("ArrowMenuBackground");
			List<String> Names = new List<String>() { "arrow_left", "arrow_up", "arrow_right", "arrow_down" }; //Reihenfolge muss zu Reihenfolge in AddButtonToRockMenu und GetRockMenuDataString passen
			mButtons = new List<ImageButton>();

			for (int i = 0; i < 4; i++)
				mButtons.Add(new ImageButton(Names[i], Vector2.Zero, new Action(() => { })));
			mButtons[SelectedIndex].IsSelected = true;
			SetButtonPositions(pMenuPosition, mButtons.Count);
		}

		public void SetRockMenu(Vector2 pMenuPosition, List<string> pData)
		{
			Position = pMenuPosition;
			mBackground = TextureManager.Instance.GetElementByString("RockMenuBackground");

			mButtons = new List<ImageButton>();
			foreach (string str in pData)
			{
				mButtons.Add(new ImageButton(str, Vector2.Zero, new Action(() => { })));
			}
			SetButtonPositions(pMenuPosition, 3);
		}

		public void AddButtonToRockMenu(int SelectedArrow)
		{
			switch (SelectedArrow)
			{
				case 0:
					mButtons.Add(new ImageButton("arrow_left", Vector2.Zero, new Action(() => { })));
					break;
				case 1:
					mButtons.Add(new ImageButton("arrow_up", Vector2.Zero, new Action(() => { })));
					break;
				case 2:
					mButtons.Add(new ImageButton("arrow_right", Vector2.Zero, new Action(() => { })));
					break;
				case 3:
					mButtons.Add(new ImageButton("arrow_down", Vector2.Zero, new Action(() => { })));
					break;
			}
			SetButtonPositions(Position, 3);
		}

		public string GetRockMenuDataString(int SelectedArrow)
		{
			switch (SelectedArrow)
			{
				case 0:
					return "arrow_left";
				case 1:
					return "arrow_up";
				case 2:
					return "arrow_right";
				case 3:
					return "arrow_up";
			}
			return "";
		}

		protected void SetButtonPositions(Vector2 pMenuPosition, int pButtonCount)
		{
			for (int i = 0; i < mButtons.Count; ++i)
			{
				mButtons[i].Position = new Vector2(pMenuPosition.X + 10 + i * ButtonSpacing, pMenuPosition.Y + 10);
			}
		}

		public void Draw(SpriteBatch pSpriteBatch, float pAlpha)
		{
			if (mBackground != null)
				pSpriteBatch.Draw(mBackground, Position, Color.White * pAlpha);
			if (mButtons != null)
				foreach (ImageButton btn in mButtons)
					btn.Draw(pSpriteBatch, pAlpha);
		}

		#endregion
	}
}
