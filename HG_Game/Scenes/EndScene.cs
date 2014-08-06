using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
using KryptonEngine.FModAudio;
using KryptonEngine.HG_Data;
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

		protected SteppingProgress VisibilityPlayer;
		protected SteppingProgress Waiting5sec;
		protected SteppingProgress Waiting10sec;
		protected SteppingProgress FadingOut;

		protected Vector2 PositionMother;
		protected Vector2 InitPosHansel;
		protected Vector2 InitPosGretel;
		protected Vector2 HouseWalkDelta;

		protected Vector2 MoveCameraDirection;

		protected const int MaxCameraX = 1800;

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

			VisibilityPlayer = new SteppingProgress(2);
			VisibilityPlayer.Reset(true);
			Waiting5sec = new SteppingProgress(5f);
			Waiting10sec = new SteppingProgress(10f);
			FadingOut = new SteppingProgress(4f);

			PositionMother = new Vector2(2000, 700);
			InitPosHansel = new Vector2(50, 700);
			InitPosGretel = new Vector2(50, 800);
			HouseWalkDelta = new Vector2(1000, 0);

			MoveCameraDirection = new Vector2(1, 0);
		}

		public void LoadContent()
		{
			mMother.LoadContent();
		}

		public void Update(Camera pCamera, TwoDRenderer pRenderer)
		{
			switch (State)
			{
				case EndState.Setup:
					pRenderer.AmbientLight = new AmbientLight();
					SceneData.BackgroundTexture.LoadBackgroundTextures("018");
					pCamera.GameScreen = new Rectangle(0,0, 2560, 1440);
					GameReferenzes.ReferenzHansel.Lantern = false;
					GameReferenzes.ReferenzGretel.Lantern = false;
					Sequences.SetToPosition(GameReferenzes.ReferenzHansel, InitPosHansel);
					Sequences.SetToPosition(GameReferenzes.ReferenzGretel, InitPosGretel);
					pCamera.MoveCamera(GameReferenzes.ReferenzHansel.CollisionBox, GameReferenzes.ReferenzGretel.CollisionBox);
					Sequences.SetToPosition(mMother, PositionMother);
					mMother.SetAnimation("shovel");
					State = EndState.WalkToHouse;
					FmodMediaPlayer.Instance.SetBackgroundSong(new List<String>() { "MusicHouse0", "SfxDigging" });
					break;
				case EndState.WalkToHouse:
					Sequences.MoveToPosition(GameReferenzes.ReferenzHansel, InitPosHansel + HouseWalkDelta, 1f, true);
					Sequences.MoveToPosition(GameReferenzes.ReferenzGretel, InitPosGretel + HouseWalkDelta, 1f, true);
					if (GameReferenzes.ReferenzHansel.SkeletonPosition == InitPosHansel + HouseWalkDelta &&
						GameReferenzes.ReferenzGretel.SkeletonPosition == InitPosGretel + HouseWalkDelta
						)
					{
						State = EndState.FadeThisShit;
						FmodMediaPlayer.FadingSpeed = 1 / 300.0f;
						FmodMediaPlayer.Instance.FadeBackgroundChannelOut(0);
						FmodMediaPlayer.Instance.FadeBackgroundChannelIn(1);
					}
					break;
				case EndState.FadeThisShit:
					/*VisibilityPlayer.StepBackward();
					if (VisibilityPlayer.Progress <= 0f)
					{*/
						GameReferenzes.ReferenzHansel.IsVisible = false;
						GameReferenzes.ReferenzGretel.IsVisible = false;
						State = EndState.Wait5sec;
					//}
					break;
				case EndState.Wait5sec:
					Waiting5sec.StepForward();
					if (Waiting5sec.Complete)
						State = EndState.MoveCamera;
					break;
				case EndState.MoveCamera:
					pCamera.MoveCameraFree(MoveCameraDirection * 2f);
					if (pCamera.Position.X > MaxCameraX)
						State = EndState.Wait10sec;
					break;
				case EndState.Wait10sec:
					Waiting10sec.StepForward();
					if (Waiting10sec.Complete)
						State = EndState.FadeOut;
					break;
				case EndState.FadeOut:
					FadingOut.StepForward();
					if (FadingOut.Complete)
					{
						FmodMediaPlayer.FadingSpeed = 1 / 90.0f;
						FmodMediaPlayer.Instance.SetBackgroundSong(new List<String>() { "MusicMainTheme" });
						SceneManager.Instance.SetCurrentSceneTo("Credits");
					}
					break;
			}
			GameReferenzes.ReferenzHansel.Update(false, 1f, null);
			GameReferenzes.ReferenzGretel.Update(false, 1f, null);
			mMother.Update();
		}

		public void Draw(TwoDRenderer pRenderer, SpriteBatch pSpriteBatch, Camera pCamera)
		{
			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(null);

			pRenderer.SetGBuffer();
			pRenderer.ClearGBuffer();
			pRenderer.Begin(pCamera.Transform);

			SceneData.BackgroundTexture.Draw(pRenderer);
			
			mMother.Draw(pRenderer);
			Color color = Color.Pink;
			color.A = (byte)(VisibilityPlayer.Progress * 255);
			if (GameReferenzes.ReferenzHansel.IsVisible)
				pRenderer.Draw(GameReferenzes.ReferenzHansel.Skeleton, GameReferenzes.ReferenzHansel.Textures, color);
			if (GameReferenzes.ReferenzGretel.IsVisible) 
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
				pSpriteBatch.DrawString(FontManager.Instance.GetElementByString("font"), State.ToString(), new Vector2(10, 10), Color.White);
				pSpriteBatch.DrawString(FontManager.Instance.GetElementByString("font"), GameReferenzes.ReferenzHansel.SkeletonPosition.ToString(), new Vector2(10, 30), Color.White);
				pSpriteBatch.DrawString(FontManager.Instance.GetElementByString("font"), GameReferenzes.ReferenzGretel.SkeletonPosition.ToString(), new Vector2(10, 50), Color.White);
				pSpriteBatch.DrawString(FontManager.Instance.GetElementByString("font"), color.A.ToString(), new Vector2(10, 70), Color.White);
			pSpriteBatch.End();
		}

		#endregion
	}
}
