using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.SceneManagement;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework.Graphics;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using KryptonEngine.Controls;
using Microsoft.Xna.Framework.Input;
using KryptonEngine;

namespace HG_Game
{
	public class MenuScene : Scene
	{
		#region Properties

		protected Texture2D mBackground;
		protected List<ImageButton> mButtons;
		protected List<LockedImageButton> mCollectableButton;
		protected int SelectedIndex;
		#endregion

		#region Getter & Setter
		#endregion

		#region Constructor
		public MenuScene(String pName)
			:base(pName)
		{
			SelectedIndex = 0;
		}
		#endregion

		#region Override Methods

		public override void LoadContent()
		{
			base.LoadContent();
			mBackground = TextureManager.Instance.GetElementByString("MainMenuBackground");
			List<String> Names = new List<String>() { "continue", "newGame", "extras", "options", "exitGame"};
			List<Action> Actions = new List<Action>() { ContinueGame, StartNewGame, ShowExtras, ShowOptions, ExitGame };
			mButtons = new List<ImageButton>();

			int TextureWidth = 226;
			int TextureHeight = 720 - 226 - 25;

			for (int i = 0; i < 5; i++ )
				mButtons.Add(new ImageButton(Names[i], new Vector2(25 + i * TextureWidth + i * 25, TextureHeight), Actions[i]));
			mButtons[SelectedIndex].IsSelected = true;


			mCollectableButton = new List<LockedImageButton>();
			for(int i = 0; i < 9; i++)
			{
				mCollectableButton.Add(new LockedImageButton("Collectable", new Vector2(25 + i * TextureWidth + i * 25, 25), null));
			}
		}

		public override void Update()
		{
			mButtons[SelectedIndex].IsSelected = false;

			if (InputHelper.Player1.ButtonJustPressed(Buttons.DPadRight))
				SelectedIndex++;
			else if (InputHelper.Player1.ButtonJustPressed(Buttons.DPadLeft))
				SelectedIndex--;

			if (SelectedIndex == -1)
				SelectedIndex = 4;
			if (SelectedIndex == 5)
				SelectedIndex = 0;

			mButtons[SelectedIndex].IsSelected = true;

			if (InputHelper.Player1.ButtonJustPressed(Buttons.A))
				mButtons[SelectedIndex].IsClicked();
		}

		public override void Draw()
		{
			mSpriteBatch.Begin();

			mSpriteBatch.Draw(mBackground, Vector2.Zero, Color.White);

			foreach (ImageButton ib in mButtons)
				ib.Draw(mSpriteBatch);

			foreach (LockedImageButton lib in mCollectableButton)
				lib.Draw(mSpriteBatch);

			mSpriteBatch.End();
		}
		#endregion

		#region Methods

		private void None()
		{

		}

		private void ContinueGame()
		{
			// Startet Spielstand aus der SaveDatei.
			SceneManager.Instance.SetCurrentSceneTo("Game");
		}

		private void StartNewGame()
		{
			// Hauptgame Scene
			SceneManager.Instance.SetCurrentSceneTo("Game");
		}

		private void ExitGame()
		{
			EngineSettings.Close = true;
		}

		private void ShowExtras()
		{

		}

		private void ShowOptions()
		{

		}
		#endregion
	}
}
