using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.SceneManagement;
using KryptonEngine.Entities;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KryptonEngine.Controls;
using Microsoft.Xna.Framework.Input;
using KryptonEngine;

namespace HG_Game
{
	public class CreditsScene : Scene
	{
		#region Properties

		protected enum FadingState
		{
			FadeIn,
			Idle,
			FadeOut
		};

		protected FadingState State;
		protected Texture2D rTexture;
		protected SteppingProgress Visibility;

		#endregion

		#region Constructor

		public CreditsScene(String pSceneName)
            : base(pSceneName)
        {
			
        }

		#endregion

		#region Override Methods

		public override void Initialize()
		{
			mCamera = new Camera();
			State = FadingState.FadeIn;
			Visibility = new SteppingProgress(2f);
		}

		public override void LoadContent()
		{
			rTexture = TextureManager.Instance.GetElementByString("credits");
		}

		public override void Update()
		{
			switch (State)
			{
				case FadingState.FadeIn:
					Visibility.StepForward();
					if (Visibility.Complete)
						State = FadingState.Idle;
					break;
				case FadingState.Idle:
					if (InputHelper.ButtonJustPressed2Player(Buttons.B))
						State = FadingState.FadeOut;
					break;
				case FadingState.FadeOut:
					Visibility.StepBackward();
					if (Visibility.Progress <= 0f)
					{
						State = FadingState.FadeIn;
						Visibility.Reset();
						SceneManager.Instance.SetCurrentSceneTo("Menu");
					}
					break;
			}
		}

		public override void Draw()
		{
			EngineSettings.Graphics.GraphicsDevice.Clear(Color.Black);
			mSpriteBatch.Begin();
			mSpriteBatch.Draw(rTexture, Vector2.Zero, Color.White);
			mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Rectangle(0, 0, 1280, 720), Color.Black * Visibility.ProgressInverse);
			mSpriteBatch.End();
		}

		#endregion
	}
}
