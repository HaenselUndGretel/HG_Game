﻿using KryptonEngine.Controls;
using KryptonEngine.Entities;
using KryptonEngine.FModAudio;
using KryptonEngine.Manager;
using KryptonEngine.SceneManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class PauseMenu
	{
		#region Properties

		private bool mIsActive;
		private int mSelectedIndex;
		private List<ImageButton> mButtons;

		#endregion

		#region Constructor

		public PauseMenu()
		{
			mButtons = new List<ImageButton>();
		}

		#endregion

		#region Methods

		public void LoadContent()
		{
			mIsActive = false;
			mSelectedIndex = 0;
			mButtons.Add(new ImageButton("continueIngame", new Vector2(100, 100), ContinueButton));
			mButtons.Add(new ImageButton("exitIngame", new Vector2(300, 100), ExitButton));
		}

		public bool Update()
		{
			if (InputHelper.ButtonJustPressed2Player(Buttons.Start))
			{
				mIsActive = !mIsActive;
				mSelectedIndex = 0;
			}

			if (mIsActive)
			{
				HandleInput();
				return true;
			}
			return false;
		}

		public void Draw(SpriteBatch pSpriteBatch)
		{
			if (mIsActive)
			{
				pSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Rectangle(0, 0, 1280, 720), Color.Black * 0.8f);
				foreach (ImageButton ib in mButtons)
					ib.Draw(pSpriteBatch);
			}
		}


		private void HandleInput()
		{
			mButtons[mSelectedIndex].IsSelected = false;

			if (InputHelper.ButtonJustPressed2Player(Buttons.DPadRight))
				++mSelectedIndex;
			else if (InputHelper.ButtonJustPressed2Player(Buttons.DPadLeft))
				--mSelectedIndex;

			if (mSelectedIndex == -1)
				mSelectedIndex = 1;
			if (mSelectedIndex == 2)
				mSelectedIndex = 0;

			mButtons[mSelectedIndex].IsSelected = true;

			if (InputHelper.ButtonJustPressed2Player(Buttons.A) || InputHelper.Player1.ActionJustPressed)
				mButtons[mSelectedIndex].IsClicked();
		}

		private void ContinueButton()
		{
			mIsActive = false;
		}

		private void ExitButton()
		{
			mIsActive = false;
			FmodMediaPlayer.Instance.FadeBackgroundChannelIn(1);
			FmodMediaPlayer.Instance.FadeBackgroundChannelIn(2);
			FmodMediaPlayer.Instance.FadeBackgroundChannelIn(3);
			SceneManager.Instance.SetCurrentSceneTo("Menu");
		}

		#endregion
	}
}
