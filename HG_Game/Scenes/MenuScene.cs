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
using KryptonEngine.FModAudio;

namespace HG_Game
{
	enum MenuState
	{
		Main, Collectables, Credits, Options, Volume, Controller
	}

	public class MenuScene : Scene
	{
		#region Properties

		protected Texture2D mBackground;
		protected Texture2D mSoundbar;
		protected List<ImageButton> mButtons;
		protected List<LockedImageButton> mCollectableButton;
		protected List<ImageButton> mOptionButtons;
		protected int SelectedIndex;
		protected int SelectedIndexCollectable;
		protected int SelectedIndexOptions;

		private MenuState menuState = MenuState.Main;

		#region MoveInOut
		private const int MAX_MOVE_OUT = -720;
		private const int MAX_MOVE_IN = 0;
		private const int MAX_MOVE_SPEED = 20;
		private int CurrentMenuPosition = MAX_MOVE_OUT;

		// False = Links | True = Rechts
		private bool MoveDirection = false;
		private bool StartMoveing = false;
		#endregion
		
		#endregion

		#region Getter & Setter
		#endregion

		#region Constructor
		public MenuScene(String pName)
			:base(pName)
		{
			SelectedIndex = 0;
			SelectedIndexCollectable = 0;
			SelectedIndexOptions = 0;
			FmodMediaPlayer.Instance.SetBackgroundSong(GameReferenzes.MainTheme);
		}
		#endregion

		#region Override Methods

		public override void LoadContent()
		{
			base.LoadContent();
			mBackground = TextureManager.Instance.GetElementByString("MainMenuBackground");
			mSoundbar = TextureManager.Instance.GetElementByString("Soundbar");

			List<String> Names = new List<String>() { "continue", "newGame", "extras", "options", "exitGame"};
			List<Action> Actions = new List<Action>() { ContinueGame, StartNewGame, ShowExtras, ShowOptions, ExitGame };
			mButtons = new List<ImageButton>();

			int TextureWidth = 226;
			int TextureHeight = 720 - 226 - 25;

			for (int i = 0; i < 5; i++ )
				mButtons.Add(new ImageButton(Names[i], new Vector2(25 + i * TextureWidth + i * 25, TextureHeight), Actions[i]));
			mButtons[SelectedIndex].IsSelected = true;

			int OffsetScreenX = 1280;
			int OffsetScreenY = 720;

			mCollectableButton = new List<LockedImageButton>();

			for (int i = 0; i < 5; i++)
				mCollectableButton.Add(new LockedImageButton("Collectable1", new Vector2(25 + i * TextureWidth + i * 25, + 25), null));
			for (int i = 5; i < 9; i++)
				mCollectableButton.Add(new LockedImageButton("Collectable1", new Vector2(138 + (i - 5) * TextureWidth + (i - 5) * 25, 276), null));
			for (int i = 0; i < 9; i++)
				mCollectableButton[i].IsUnlocked = true;

			int OffsetRight = (1280 - (3 * TextureWidth + 25)) / 2;

			mOptionButtons = new List<ImageButton>();
			Names = new List<String>() { "volume", "controller", "credits" };
			Actions = new List<Action>() { SetVolumeSetting, ShowControllerSettings, ShowCredits };
			for(int i = 0; i < 3; i++)
				mOptionButtons.Add(new ImageButton(Names[i], new Vector2(OffsetRight + i * TextureWidth + i * 25, 276), Actions[i]));
		}

		public override void Update()
		{
			switch(menuState)
			{
				case MenuState.Main: UpdateMainMenu();
					break;
				case MenuState.Collectables: UpdateCollectablesMenu();
					break;
				case MenuState.Options: UpdateOptionsMenu();
					break;
				case MenuState.Controller: UpdateControllerMenu();
					break;
				case MenuState.Credits: UpdateCredits();
					break;
			}
		}

		private void UpdateMainMenu()
		{
			mButtons[SelectedIndex].IsSelected = false;

			if (InputHelper.ButtonJustPressed2Player(Buttons.DPadRight))
				SelectedIndex++;
			else if (InputHelper.ButtonJustPressed2Player(Buttons.DPadLeft))
				SelectedIndex--;

			if (SelectedIndex == -1)
				SelectedIndex = 4;
			if (SelectedIndex == 5)
				SelectedIndex = 0;

			mButtons[SelectedIndex].IsSelected = true;

			if (InputHelper.ButtonJustPressed2Player(Buttons.A) || InputHelper.Player1.ActionJustPressed)
				mButtons[SelectedIndex].IsClicked();
		}

		private void UpdateCollectablesMenu()
		{
			if(StartMoveing)
			{
				if (!MoveDirection)
					MoveListIn(new List<GameObject>(mCollectableButton));
				else
					MoveListOut(new List<GameObject>(mCollectableButton));
			}
			else
			{
				mCollectableButton[SelectedIndexCollectable].IsSelected = false;

				if (InputHelper.ButtonJustPressed2Player(Buttons.DPadRight))
					SelectedIndexCollectable++;
				else if (InputHelper.ButtonJustPressed2Player(Buttons.DPadLeft))
					SelectedIndexCollectable--;
				else if (InputHelper.ButtonJustPressed2Player(Buttons.DPadUp)
					&& SelectedIndexCollectable > 4)
					SelectedIndexCollectable -= 5;
				else if (InputHelper.ButtonJustPressed2Player(Buttons.DPadDown)
				   && SelectedIndexCollectable < 4)
					SelectedIndexCollectable += 5;
				else if (InputHelper.ButtonJustPressed2Player(Buttons.DPadDown)
					&& SelectedIndexCollectable == 4)
					SelectedIndexCollectable += 4;

				if (SelectedIndexCollectable == -1)
					SelectedIndexCollectable = 8;
				if (SelectedIndexCollectable == 9)
					SelectedIndexCollectable = 0;

				mCollectableButton[SelectedIndexCollectable].IsSelected = true;

				if (InputHelper.ButtonJustPressed2Player(Buttons.A))
					mCollectableButton[SelectedIndexCollectable].IsClicked();

				if(InputHelper.ButtonJustPressed2Player(Buttons.B))
				{
					StartMoveing = true;
					MoveDirection = true;
					mCollectableButton[SelectedIndexCollectable].IsSelected = false;
				}
			}
		}

		private void UpdateOptionsMenu()
		{
			if (StartMoveing)
			{
				if (!MoveDirection)
					MoveListIn(new List<GameObject>(mOptionButtons));
				else
					MoveListOut(new List<GameObject>(mOptionButtons));
			}
			else
			{
				mOptionButtons[SelectedIndexOptions].IsSelected = false;

				if (InputHelper.ButtonJustPressed2Player(Buttons.DPadRight))
					SelectedIndexOptions++;
				else if (InputHelper.ButtonJustPressed2Player(Buttons.DPadLeft))
					SelectedIndexOptions--;

				if (SelectedIndexOptions == -1)
					SelectedIndexOptions = 2;
				if (SelectedIndexOptions == 3)
					SelectedIndexOptions = 0;

				mOptionButtons[SelectedIndexOptions].IsSelected = true;

				if (InputHelper.ButtonJustPressed2Player(Buttons.A))
					mOptionButtons[SelectedIndexOptions].IsClicked();

				if (InputHelper.ButtonJustPressed2Player(Buttons.B))
				{
					StartMoveing = true;
					MoveDirection = true;
					mOptionButtons[SelectedIndexOptions].IsSelected = false;
				}
			}
		}

		private void UpdateControllerMenu()
		{
			if (InputHelper.ButtonJustPressed2Player(Buttons.B))
				menuState = MenuState.Options;
		}

		private void UpdateCredits()
		{
			if (InputHelper.ButtonJustPressed2Player(Buttons.B))
				menuState = MenuState.Options;
		}

		public override void Draw()
		{
			mSpriteBatch.Begin();

			mSpriteBatch.Draw(mBackground, Vector2.Zero, Color.White);

			switch(menuState)
			{
				case MenuState.Main:
					foreach (ImageButton ib in mButtons)
						ib.Draw(mSpriteBatch);
					break;
				case MenuState.Collectables:
					foreach (LockedImageButton lib in mCollectableButton)
						lib.Draw(mSpriteBatch);
					break;
				case MenuState.Options:
					foreach (ImageButton ib in mOptionButtons)
						ib.Draw(mSpriteBatch);
					break;
				case MenuState.Controller:
					DrawController();
					break;
				case MenuState.Credits: DrawCredits();
					break;
					
			}

			mSpriteBatch.End();
		}

		private void DrawController()
		{
			mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Rectangle(0, 0, 1280, 720), Color.Black * 0.8f);
		}

		private void DrawCredits()
		{
			mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Rectangle(0, 0, 1280, 720), Color.Black * 0.8f);
		}
		#endregion

		#region Methods

		private void None()
		{

		}

		private void ShowControllerSettings()
		{
			menuState = MenuState.Controller;
		}

		private void SetVolumeSetting()
		{
			FmodMediaPlayer.Instance.Mute();
		}

		private void ShowCredits()
		{
			SceneManager.Instance.SetCurrentSceneTo("Credits");
			//menuState = MenuState.Credits;
		}

		private void ContinueGame()
		{
			// Startet Spielstand aus der SaveDatei.
			FmodMediaPlayer.Instance.SetBackgroundSong(GameReferenzes.GetBackgroundMusic());
			SceneManager.Instance.SetCurrentSceneTo("Game");
		}

		private void StartNewGame()
		{
			// Hauptgame Scene
			((GameScene)SceneManager.Instance.GetScene("Game")).RestartSavegame();
			FmodMediaPlayer.Instance.SetBackgroundSong(GameReferenzes.HouseTheme);
			FmodMediaPlayer.Instance.FadeBackgroundChannelIn(1);
			FmodMediaPlayer.Instance.FadeBackgroundChannelIn(2);
			FmodMediaPlayer.Instance.FadeBackgroundChannelIn(3);

			GameReferenzes.SceneID= 0;

			SceneManager.Instance.SetCurrentSceneTo("Game");
		}

		private void ExitGame()
		{
			EngineSettings.Close = true;
		}

		private void ShowExtras()
		{
			menuState = MenuState.Collectables;
			SelectedIndexCollectable = 0;
		}

		private void ShowOptions()
		{
			menuState = MenuState.Options;
			SelectedIndexOptions = 0;
		}

		private void MoveListIn(List<GameObject> gos)
		{
			if (CurrentMenuPosition == MAX_MOVE_IN)
			{
				StartMoveing = false;
				return;
			}

			CurrentMenuPosition += MAX_MOVE_SPEED;

			for (int i = 0; i < gos.Count; i++)
				gos[i].PositionY += MAX_MOVE_SPEED; 
		}

		private void MoveListOut(List<GameObject> gos)
		{
			if (CurrentMenuPosition == MAX_MOVE_OUT)
			{
				StartMoveing = false;
				menuState = MenuState.Main;
				return;
			}

			CurrentMenuPosition -= MAX_MOVE_SPEED;

			for (int i = 0; i < gos.Count; i++)
				gos[i].PositionY -= MAX_MOVE_SPEED;
		}
		#endregion
	}
}
