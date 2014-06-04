using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
using KryptonEngine.Manager;
using KryptonEngine.SceneManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	public class GameScene : Scene
	{
		#region Properties

		protected Logic mLogic;
		protected Savegame mSavegame;
		protected Hansel mHansel;
		protected Gretel mGretel;
		protected SceneData mScene;
		protected SkeletonRenderer mSkeletonRenderer;

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
			mLogic = new Logic();
			mSkeletonRenderer = EngineSettings.SpineRenderer;
			EngineSettings.IsDebug = true;
			mCamera = new Camera();
			mCamera.GameScreen = new Rectangle(0, 0, 3000, 3000);
			mSavegame = new Savegame();
			mHansel = new Hansel();
			mGretel = new Gretel();
		}

		public override void LoadContent()
		{
			//Hansel & Gretel
			mHansel.LoadContent();
			mGretel.LoadContent();
			mHansel.mCurrentActivity = new None();
			mGretel.mCurrentActivity = new None();
			mHansel.LoadReferences(mCamera, mGretel);
			mGretel.LoadReferences(mCamera, mHansel);
			//Savegame
			mSavegame = Savegame.Load(mHansel, mGretel);
			mLogic.ActivityHandler.SetupInteractiveObjectsFromDeserialization(mSavegame, mHansel, mGretel);
			mScene = mSavegame.Scenes[mSavegame.SceneId];
			//Camera
			mCamera.GameScreen = mScene.GamePlane;
		}

		public override void Update()
		{
			//Update Logic
			mLogic.Update(mSavegame, ref mScene, mHansel, mGretel, mCamera);
			//Update Player
			mHansel.Update(mLogic.HanselMayMove, mHansel.mCurrentActivity.mMovementSpeedFactorHansel, mScene);
			mGretel.Update(mLogic.GretelMayMove, mGretel.mCurrentActivity.mMovementSpeedFactorGretel, mScene);
			//Update Camera
			mCamera.MoveCamera(mHansel.CollisionBox, mGretel.CollisionBox);
		}

		public override void Draw()
		{
			//---------------Collect Packages
			List<DrawPackage> DrawPackagesGame = new List<DrawPackage>();
			DrawPackagesGame.AddRange(mScene.DrawPackages);
			DrawPackagesGame.Add(mHansel.DrawPackage);
			DrawPackagesGame.Add(mGretel.DrawPackage);

			//ToDo: DrawPackage Sorting!

			//---------------Draw
			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(mRenderTarget);
			//DrawBackground();

			Matrix TmpTransformation = mCamera.GetTranslationMatrix();
			mSkeletonRenderer.Effect.World = TmpTransformation;
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, TmpTransformation);
			//DrawBackground
			mSpriteBatch.Draw(mScene.BackgroundTextures[0], Vector2.Zero, Color.White);
			//Draw Game
			foreach (DrawPackage dPack in DrawPackagesGame)
			{
				if (!dPack.Spine)
					dPack.Draw(mSpriteBatch, mSkeletonRenderer);
			}
			mSpriteBatch.End();

			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, TmpTransformation);
			//Draw Spine
			foreach (DrawPackage dPack in DrawPackagesGame)
			{
				if (dPack.Spine)
					dPack.Draw(mSpriteBatch, mSkeletonRenderer);
			}
			//Draw ActionInfo
			mLogic.ActivityHandler.DrawActionInfo(mHansel, mGretel, mSpriteBatch, mSkeletonRenderer, TextureManager.Instance.GetElementByString("button_x"));
			mLogic.ItemHandler.DrawInventory(mHansel, mGretel, mSpriteBatch, mSkeletonRenderer);
			mSpriteBatch.End();

			//Draw to Screen
			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(null);
			mSpriteBatch.Begin();
			if (mLogic.SceneSwitchHandler.CurrentState == SceneSwitchHandler.State.Switching)
				mSpriteBatch.Draw(mRenderTarget, Vector2.Zero, new Color(mLogic.SceneSwitchHandler.Fading, mLogic.SceneSwitchHandler.Fading, mLogic.SceneSwitchHandler.Fading));
			else
				mSpriteBatch.Draw(mRenderTarget, Vector2.Zero, Color.White);
			mSpriteBatch.End();
		}

		#endregion
	}
}
