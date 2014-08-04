using KryptonEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework.Graphics;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;

namespace HG_Game
{
	public class SplashScene : Scene
	{
		#region Properties

		protected enum LogoState
		{
			GA_FadeIn,
			GA_Idle,
			GA_FadeOut,
			Fables_FadeIn,
			Fables_Idle,
			Fables_FadeOut
		};

		protected LogoState State;
		protected Texture2D rTextureGA;
		protected Texture2D rTextureFables;
		protected SteppingProgress Visibility;
		protected SteppingProgress IdleTimer;

		#endregion

		#region Constructor

		public SplashScene(String pSceneName)
            : base(pSceneName)
        {

        }

		#endregion

		#region Override Methods

		public override void Initialize()
		{
			mCamera = new Camera();
			State = LogoState.GA_FadeIn;
			Visibility = new SteppingProgress(1.5f);
			IdleTimer = new SteppingProgress(4f);
		}

		public override void LoadContent()
		{
			rTextureGA = TextureManager.Instance.GetElementByString("logo_ga");
			rTextureFables = TextureManager.Instance.GetElementByString("logo_fables");
		}

		public override void Update()
		{
			switch (State)
			{
				case LogoState.GA_FadeIn:
					Visibility.StepForward();
					if (Visibility.Complete)
						State = LogoState.GA_Idle;
					break;
				case LogoState.GA_Idle:
					IdleTimer.StepForward();
					IdleTimer.StepForward();
					if (IdleTimer.Complete)
					{
						IdleTimer.Reset();
						State = LogoState.GA_FadeOut;
					}
					break;
				case LogoState.GA_FadeOut:
					Visibility.StepBackward();
					if (Visibility.Progress <= 0f)
						State = LogoState.Fables_FadeIn;
					break;
				case LogoState.Fables_FadeIn:
					Visibility.StepForward();
					if (Visibility.Complete)
						State = LogoState.Fables_Idle;
					break;
				case LogoState.Fables_Idle:
					IdleTimer.StepForward();
					if (IdleTimer.Complete)
					{
						IdleTimer.Reset();
						State = LogoState.Fables_FadeOut;
					}
					break;
				case LogoState.Fables_FadeOut:
					Visibility.StepBackward();
					if (Visibility.Progress <= 0f)
						SceneManager.Instance.SetCurrentSceneTo("Menu");
					break;
			}
		}

		public override void Draw()
		{
			Texture2D Logo = rTextureGA;
			if (State == LogoState.Fables_FadeIn || State == LogoState.Fables_Idle || State == LogoState.Fables_FadeOut)
				Logo = rTextureFables;
			mSpriteBatch.Begin();
			mSpriteBatch.Draw(Logo, Vector2.Zero, Color.White);
			mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Rectangle(0, 0, 1280, 720), Color.Black * Visibility.ProgressInverse);
			mSpriteBatch.End();
		}

		#endregion
	}
}
