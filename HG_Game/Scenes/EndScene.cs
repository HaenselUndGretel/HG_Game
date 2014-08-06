using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
using KryptonEngine.Manager;
using KryptonEngine.Rendering;
using KryptonEngine.SceneManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class EndScene
	{
		#region Properties

		protected enum EndState
		{
			Setup,
			WalkToHouse,
			FadeThisShit,
			Wait5sec,
			MoveCamera,
			Wait10sec,
			FadeOut
		}

		protected EndState State;

		protected Mother mMother;

		protected Sprite Background;

		protected SteppingProgress VisibilityPlayer;
		protected SteppingProgress Waiting5sec;
		protected SteppingProgress Waiting10sec;
		protected SteppingProgress FadingOut;

		protected Vector2 PositionMother;
		protected Vector2 InitPosHansel;
		protected Vector2 InitPosGretel;
		protected Vector2 HouseWalkDelta;

		protected Vector2 MoveCameraDirection;

		protected const int MaxCameraX = 1500;

		protected Rectangle SceneSize;

		#endregion

		#region Constructor

		public EndScene()
		{

		}

		#endregion

		#region Methods

		public void Initialize()
		{
			State = EndState.Setup;
			mMother = new Mother();

			Background = new Sprite();
			Background.TextureName = "018";

			VisibilityPlayer = new SteppingProgress(2);
			VisibilityPlayer.Reset(true);
			Waiting5sec = new SteppingProgress(5f);
			Waiting10sec = new SteppingProgress(10f);
			FadingOut = new SteppingProgress(4f);

			PositionMother = new Vector2(1500, 600);
			InitPosHansel = new Vector2(50, 200);
			InitPosHansel = new Vector2(50, 300);
			HouseWalkDelta = new Vector2(1000, 500);

			MoveCameraDirection = new Vector2(1, 0);
		}

		public void LoadContent()
		{
			mMother.LoadContent();
			Background.LoadBackgroundTextures();
			SceneSize = Background.CollisionBox;
		}

		public void Update(Camera pCamera)
		{
			switch (State)
			{
				case EndState.Setup:
					pCamera.GameScreen = SceneSize;
					pCamera.MoveCamera(GameReferenzes.ReferenzHansel.CollisionBox, GameReferenzes.ReferenzGretel.CollisionBox);
					GameReferenzes.ReferenzHansel.Lantern = false;
					GameReferenzes.ReferenzGretel.Lantern = false;
					Sequences.SetToPosition(GameReferenzes.ReferenzHansel, InitPosHansel);
					Sequences.SetToPosition(GameReferenzes.ReferenzGretel, InitPosGretel);
					Sequences.SetToPosition(mMother, PositionMother);
					State = EndState.WalkToHouse;
					break;
				case EndState.WalkToHouse:
					Sequences.MoveToPosition(GameReferenzes.ReferenzHansel, InitPosHansel + HouseWalkDelta, 1f, true);
					Sequences.MoveToPosition(GameReferenzes.ReferenzGretel, InitPosGretel + HouseWalkDelta, 1f, true);
					if (GameReferenzes.ReferenzHansel.SkeletonPosition == InitPosHansel + HouseWalkDelta &&
						GameReferenzes.ReferenzGretel.SkeletonPosition == InitPosGretel + HouseWalkDelta
						)
						State = EndState.FadeThisShit;
					break;
				case EndState.FadeThisShit:
					VisibilityPlayer.StepBackward();
					if (VisibilityPlayer.Progress <= 0f)
						State = EndState.FadeThisShit;
					break;
				case EndState.Wait5sec:
					Waiting5sec.StepForward();
					if (Waiting5sec.Complete)
						State = EndState.MoveCamera;
					break;
				case EndState.MoveCamera:
					pCamera.MoveCamera(MoveCameraDirection * 1f);
					if (pCamera.Position.X > MaxCameraX)
						State = EndState.Wait10sec;
					break;
				case EndState.Wait10sec:
					Waiting10sec.StepForward();
					if (Waiting10sec.Complete)
						State = EndState.MoveCamera;
					break;
				case EndState.FadeOut:
					FadingOut.StepForward();
					if (FadingOut.Complete)
					{
						SceneManager.Instance.SetCurrentSceneTo("Menu");
					}
					break;
			}
		}

		public void Draw(TwoDRenderer pRenderer, SpriteBatch pSpriteBatch, Camera pCamera)
		{
			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(null);

			pRenderer.SetGBuffer();
			pRenderer.ClearGBuffer();
			pRenderer.Begin(pCamera.Transform);

			Background.Draw(pRenderer);
			mMother.Draw(pRenderer);
			Color color = Color.White;
			color.A = (byte)(VisibilityPlayer.Progress * 255);
			pRenderer.Draw(GameReferenzes.ReferenzHansel.Skeleton, GameReferenzes.ReferenzHansel.Textures, color);
			pRenderer.Draw(GameReferenzes.ReferenzGretel.Skeleton, GameReferenzes.ReferenzGretel.Textures, color);

			pRenderer.End();
			
			//--------------------DrawToScreen--------------------
			pRenderer.DisposeGBuffer();
			pRenderer.ProcessLight(new List<Light>(), pCamera.Transform);
			pRenderer.ProcessFinalScene();

			pRenderer.DrawFinalTargettOnScreen(pSpriteBatch);

			//--------------------SpriteBatch--------------------
			pSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				pSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Rectangle(0, 0, 1280, 720), Color.Black * FadingOut.Progress);
			pSpriteBatch.End();
		}

		#endregion
	}
}
