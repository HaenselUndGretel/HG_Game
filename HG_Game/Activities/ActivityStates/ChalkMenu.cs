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
			SelectedIndex = 1;
		}

		public int Update(Player pPlayer)
		{
			//Vor Update-Aufruf auf Arrow oder Rock Menu setzen
			mButtons[SelectedIndex].IsSelected = false;

			if (pPlayer.Input.ButtonJustPressed(Buttons.DPadRight))
				SelectedIndex++;
			else if (pPlayer.Input.ButtonJustPressed(Buttons.DPadLeft))
				SelectedIndex--;

			if (SelectedIndex < 1)
				SelectedIndex = mButtons.Count;
			if (SelectedIndex > mButtons.Count)
				SelectedIndex = 1;

			mButtons[SelectedIndex].IsSelected = true;

			if (pPlayer.Input.ButtonJustPressed(Buttons.A))
				return SelectedIndex;
			return 0;
		}

		public void Draw(SpriteBatch pSpriteBatch)
		{
			pSpriteBatch.Begin();

			pSpriteBatch.Draw(mBackground, Vector2.Zero, Color.White);

			foreach (ImageButton ib in mButtons)
				ib.Draw(pSpriteBatch);

			pSpriteBatch.End();
		}

		#endregion

		#region Methods

		public void SetArrowMenu(Vector2 pMenuPosition, float pButtonSpacing)
		{
			Position = pMenuPosition;
			mBackground = TextureManager.Instance.GetElementByString("ArrowMenuBackground");
			List<String> Names = new List<String>() { "arrow_left", "arrow_up", "arrow_right", "arrow_down" }; //Reihenfolge muss zu Reihenfolge in AddButtonToRockMenu und GetRockMenuDataString passen
			mButtons = new List<ImageButton>();

			for (int i = 0; i < 4; i++)
				mButtons.Add(new ImageButton(Names[i], Vector2.Zero, new Action(() => { })));
			mButtons[SelectedIndex].IsSelected = true;
			SetPosition(pMenuPosition, pButtonSpacing, mButtons.Count);
		}

		public void SetRockMenu(Vector2 pMenuPosition, float pButtonSpacing, List<string> pData)
		{
			Position = pMenuPosition;
			mBackground = TextureManager.Instance.GetElementByString("RockMenuBackground");

			mButtons = new List<ImageButton>();
			foreach (string str in pData)
			{
				mButtons.Add(new ImageButton(str, Vector2.Zero, new Action(() => { })));
			}
			SetPosition(pMenuPosition, pButtonSpacing, 3);
		}

		public void AddButtonToRockMenu(int SelectedArrow)
		{
			switch (SelectedArrow)
			{
				case 1:
					mButtons.Add(new ImageButton("arrow_left", Vector2.Zero, new Action(() => { })));
					break;
				case 2:
					mButtons.Add(new ImageButton("arrow_up", Vector2.Zero, new Action(() => { })));
					break;
				case 3:
					mButtons.Add(new ImageButton("arrow_right", Vector2.Zero, new Action(() => { })));
					break;
				case 4:
					mButtons.Add(new ImageButton("arrow_down", Vector2.Zero, new Action(() => { })));
					break;
			}
		}

		public string GetRockMenuDataString(int SelectedArrow)
		{
			switch (SelectedArrow)
			{
				case 1:
					return "arrow_left";
				case 2:
					return "arrow_up";
				case 3:
					return "arrow_right";
				case 4:
					return "arrow_up";
			}
			return "";
		}

		protected void SetPosition(Vector2 pMenuPosition, float pButtonSpacing, int pButtonCount)
		{
			Vector2 MenuPosition = new Vector2(pMenuPosition.X - pButtonCount * pButtonSpacing / 2, pMenuPosition.Y);
			for (int i = 0; i < mButtons.Count; ++i)
			{
				mButtons[i].Position = new Vector2(MenuPosition.X + i * pButtonSpacing, MenuPosition.Y);
			}
		}

		public void Draw(SpriteBatch pSpriteBatch, float pAlpha)
		{
			if (mBackground != null)
				pSpriteBatch.Draw(mBackground, Position, Color.Wheat * pAlpha);
			if (mButtons != null)
				foreach (ImageButton btn in mButtons)
					btn.Draw(pSpriteBatch);
		}

		#endregion
	}
}
