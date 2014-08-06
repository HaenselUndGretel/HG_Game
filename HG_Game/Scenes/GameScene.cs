using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.SceneManagement;
using KryptonEngine.Entities;
using HanselAndGretel.Data;
using KryptonEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using KryptonEngine.HG_Data;
using KryptonEngine.Manager;
using KryptonEngine.Controls;
using KryptonEngine.FModAudio;
using KryptonEngine.AI;
using Microsoft.Xna.Framework.Input;

namespace HG_Game
{
	public class GameScene : Scene
	{
		#region Properties

		public static bool End;

		public enum GameState
		{
			Running,
			Paused,
			CollectableInfo,
			End,
			EndScene
		}

		protected GameState mState;
		protected SteppingProgress mEndGameFading;
		protected Texture2D mEndGameTexture;

		protected Hansel mHansel;
		protected Gretel mGretel;

		protected Savegame mSavegame;
		protected SceneData mScene;

		protected Logic mLogic;

		protected PauseMenu mPauseMenu;

		protected EndScene DasEnde;

		#endregion

		#region Constructor

		public GameScene(String pSceneName)
			: base(pSceneName)
		{

		}

		#endregion

		#region Override Methods

		public override void Initialize()
		{
#if DEBUG
			EngineSettings.IsDebug = true;
#endif
			//GameState
			mState = GameState.Running;
			mEndGameFading = new SteppingProgress(Hardcoded.End_FadingDuration);
			//Player
			mHansel = new Hansel();
			mGretel = new Gretel();
			mHansel.mCurrentActivity = ActivityHandler.None;
			mGretel.mCurrentActivity = ActivityHandler.None;
			//Camera
			mCamera = new Camera();
			//Savegame
			mSavegame = new Savegame();
			//Logic
			mLogic = new Logic();
			//PauseMenu
			mPauseMenu = new PauseMenu();

			//EndScene
			DasEnde = new EndScene();
			DasEnde.Initialize();

			FmodMediaPlayer.FadingSpeed = 1 / 90.0f;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			mEndGameTexture = TextureManager.Instance.GetElementByString("endgame");
			//Logic
			mLogic.LoadContent(mHansel, mGretel);
			//Player
			mHansel.LoadContent();
			mGretel.LoadContent();
			mHansel.mCurrentActivity = ActivityHandler.None;
			mGretel.mCurrentActivity = ActivityHandler.None;
			mHansel.LoadReferences(mCamera, mGretel);
			mGretel.LoadReferences(mCamera, mHansel);
			mHansel.ApplySettings();
			mGretel.ApplySettings();
			//Savegame
			mSavegame = Savegame.Load(mHansel, mGretel);
			mScene = mSavegame.Scenes[mSavegame.SceneId];
			SceneData.BackgroundTexture.LoadBackgroundTextures(Savegame.LevelNameFromId(mSavegame.SceneId));
			mRenderer.AmbientLight = mScene.SceneAmbientLight;
			//Camera
			mCamera.GameScreen = mScene.GamePlane;
			//Set ActivityStates in IObjs
			mLogic.ActivityHandler.SetupInteractiveObjectsFromDeserialization(mSavegame, mHansel, mGretel);
			//PauseMenu
			mPauseMenu.LoadContent();
			mHansel.Lantern = true;

			GameReferenzes.ReferenzHansel = mHansel;
			GameReferenzes.ReferenzGretel = mGretel;
			GameReferenzes.GameCamera = mCamera;
			GameReferenzes.Level = mScene;
			GameReferenzes.SaveGame = mSavegame;

			GameReferenzes.SceneID = mSavegame.SceneId;

			AIManager.Instance.ChangeMap(mCamera.GameScreen, mScene.MoveArea);

			foreach (InteractiveObject iobj in mScene.InteractiveObjects)
				AIManager.Instance.SetInterActiveObjects(mScene.InteractiveObjects);

			AIManager.Instance.SetAgents(mScene.Enemies);

			mRenderer.SetMinFogHeight(0.1f);
			mRenderer.SetMaxFogHeight(0.2f);

			//EndScene
			DasEnde.LoadContent();
		}

		public override void Update()
		{
			if (End)
			{
				mState = GameState.End;
				End = false;
			}
			mPauseMenu.Update(ref mState);
			switch (mState)
			{
				case GameState.Running:
					//Update Logic
					mLogic.Update(mSavegame, ref mScene, mHansel, mGretel, mCamera, mRenderer, ref mState);
					//Update Player
					mHansel.Update(mLogic.HanselMayMove, mHansel.mCurrentActivity.mMovementSpeedFactorHansel, mScene);
					mGretel.Update(mLogic.GretelMayMove, mGretel.mCurrentActivity.mMovementSpeedFactorGretel, mScene);
#if DEBUG
					//DebugCheats, im finalen Spiel löschen
					Cheats.Update(mSavegame, mScene, mHansel, mGretel);
#endif
					//Update Camera
					mCamera.MoveCamera(mHansel.CollisionBox, mGretel.CollisionBox);

					foreach (InteractiveObject io in mScene.InteractiveObjects)
						io.Update();
					break;
				case GameState.CollectableInfo:
					FmodMediaPlayer.Instance.SetBackgroundVolume(0.1f);
					if (InputHelper.ButtonJustPressed2Player(Buttons.B))
					{
						FmodMediaPlayer.Instance.StopSong("collectable1");
						FmodMediaPlayer.Instance.StopSong("collectable2");
						FmodMediaPlayer.Instance.StopSong("collectable3");
						FmodMediaPlayer.Instance.SetBackgroundVolume(1.0f);
						mState = GameState.Running;
					}
					break;
				case GameState.End:
					mEndGameFading.StepForward();
					if (mEndGameFading.Complete && InputHelper.ButtonJustPressed2Player(Buttons.A))
					{ //Wenn komplett sichtbar & ein Spieler drückt A
						mEndGameFading.Reset();
						RestartGame();
						mState = GameState.Running;
					}
					break;
				case GameState.EndScene:
					DasEnde.Update(mCamera);
					break;
			}
		}

		public override void Draw()
		{
			if (mState == GameState.EndScene)
			{
				DasEnde.Draw(mRenderer, mSpriteBatch, mCamera);
				return;
			}
			//--------------------Prepare Draw--------------------
			mScene.RenderList = mScene.RenderList.OrderBy(iobj => iobj.NormalZ).ToList(); //Wird DrawZ vom Character genommen wenn er als IObj betrachtet wird?
			//Temporär immer neu aufbauen, sollte später anders umgesetzt werden, wie RenderList
			List<Light> LightList = new List<Light>(mScene.Lights) { mLogic.ItemHandler.LanternLight };

			//--------------------Renderer (Game & Lighting)--------------------
			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(null);

			mRenderer.SetGBuffer();
			mRenderer.ClearGBuffer();
			mRenderer.Begin(mCamera.Transform);
				//Render Background
				SceneData.BackgroundTexture.Draw(mRenderer);
				//Render Game
				foreach (InteractiveObject iObj in mScene.RenderList)
					if (iObj.IsVisible)
						iObj.Draw(mRenderer);
			mRenderer.End();
			
			//--------------------DrawToScreen--------------------
			mRenderer.DisposeGBuffer();
			mRenderer.ProcessLight(LightList, mCamera.Transform);
			mRenderer.ProcessFinalScene();

			mRenderer.DrawFinalTargettOnScreen(mSpriteBatch);

			if (GameReferenzes.Level.Fog)
				mRenderer.ApplyFog(mCamera.Transform);

			//--------------------SpriteBatch WorldSpace(HUD & Infos)--------------------
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, mCamera.Transform);
			//Render ActionInfo
			mLogic.ActivityHandler.DrawActionInfo(mSpriteBatch, mHansel, mGretel);
			//Render ButtonHud
			mLogic.ActivityHandler.DrawActivityInstruction(mSpriteBatch, mHansel, mGretel);
			mSpriteBatch.End();

			//--------------------SpriteBatch ScreenSpace(PauseMenu & SceneSwitch)--------------------
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			//Render SceneSwitch darken
			mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Rectangle(0, 0, 1280, 720), Color.Black * mLogic.SceneSwitchHandler.Fading);
			//Render PauseMenu
			if (mState == GameState.CollectableInfo)
			{
				string name = "ShowTexture" + mSavegame.Collectables[mSavegame.Collectables.Count - 1].CollectableId;
				mSpriteBatch.Draw(TextureManager.Instance.GetElementByString(name), Vector2.Zero, Color.White);
			}
			mPauseMenu.Draw(mSpriteBatch);
			if (mState == GameState.End)
			{
				mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Rectangle(0, 0, 1280, 720), Color.Black * mEndGameFading.Progress);
				mSpriteBatch.Draw(mEndGameTexture, Vector2.Zero, Color.White);
			}
			mSpriteBatch.End();

			#region Debug
#if DEBUG
			if (EngineSettings.IsDebug)
			{
				mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, mCamera.Transform);
				mScene.DrawDebug(mSpriteBatch);
				for (int i = mScene.RenderList.Count - 1; i >= 0; --i)
				{
					mScene.RenderList[i].DrawDebug(mSpriteBatch);
				}
				AIManager.Instance.DrawDebugAiGrid(mSpriteBatch);
				mSpriteBatch.End();

				SpriteFont font = FontManager.Instance.GetElementByString("font");
				StringBuilder sb = new StringBuilder();

				if (GameReferenzes.TargetPlayer != null && GameReferenzes.UntargetPlayer != null)
					sb.AppendLine("Abstand: " + Vector2.Distance(GameReferenzes.TargetPlayer.Position, GameReferenzes.UntargetPlayer.Position).ToString());
				sb.AppendLine("--------------------Player--------------------");
				sb.AppendLine("Hansel: __Pos_ " + mHansel.SkeletonPosition.ToString() + " __CBox_ " + mHansel.CollisionBox.ToString() + " __BTemp_ " + mHansel.BodyTemperature.ToString());
				sb.AppendLine("Gretel: __Pos_ " + mGretel.SkeletonPosition.ToString() + " __CBox_ " + mGretel.CollisionBox.ToString() + " __BTemp_ " + mGretel.BodyTemperature.ToString());
				sb.AppendLine("--------------------Camera--------------------");
				sb.AppendLine("Camera: __Pos_ " + (mCamera.Position - new Vector2(EngineSettings.VirtualResWidth, EngineSettings.VirtualResHeight) / 2).ToString() + " __GameScreen_ " + mCamera.GameScreen.ToString());
				sb.AppendLine("--------------------ActivityStates--------------------");
				sb.AppendLine("Hansel: __State_ " + mHansel.mCurrentState + " __ActivityState_ " + mHansel.mCurrentActivity.GetType().ToString());
				sb.AppendLine("Gretel: __State_ " + mGretel.mCurrentState + " __ActivityState " + mGretel.mCurrentActivity.GetType().ToString());
				sb.AppendLine("--------------------ThumbstickRotation--------------------");
				sb.AppendLine("Rotation: " + mHansel.Input.LeftStickRotation.ToString());
				sb.AppendLine("Mit Uhrzeiger");
				for (int i = 0; i < (int)(mHansel.Input.LeftStickRotation * 1000); ++i)
					sb.Append("|");
				sb.AppendLine("");
				sb.AppendLine("Gegen Uhrzeiger");
				for (int i = 0; i > (int)(mHansel.Input.LeftStickRotation * 1000); --i)
					sb.Append("|");
				sb.AppendLine("");
				sb.AppendLine("-------------------- --------------------");
				
				mSpriteBatch.Begin();
				mSpriteBatch.DrawString(font, sb, new Vector2(10, 10), Color.White * 0.5f, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);
				mSpriteBatch.End();
			}
#endif
			#endregion

		}

		#endregion

		#region Methods

		public void RestartSavegame()
		{
			Savegame.Delete();
			RestartGame();
		}

		public void RestartGame()
		{
			Initialize();
			LoadContent();
		}

		#endregion
	}
}
