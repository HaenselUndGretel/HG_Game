using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
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
			mSavegame = Savegame.Load();
			mScene = mSavegame.Scenes[mSavegame.SceneId];
			mHansel.LoadContent();
			mGretel.LoadContent();
			mHansel.Position = mSavegame.PositionHansel;
			mGretel.Position = mSavegame.PositionGretel;
			mHansel.LoadReferences(mCamera, mGretel);
			mGretel.LoadReferences(mCamera, mHansel);
		}

		public override void Update()
		{
			//Update Logic
			mLogic.Update(mSavegame, ref mScene, mHansel, mGretel);
			//Update Player
			mHansel.Update(mLogic.HanselMayMove, mScene);
			mGretel.Update(mLogic.GretelMayMove, mScene);
			//Update Camera
			mCamera.MoveCamera(mHansel.CollisionBox, mGretel.CollisionBox);
		}

		public override void Draw()
		{
			//---------------Collect Packages
			List<DrawPackage> DrawPackagesPlanes = new List<DrawPackage>();
			DrawPackagesPlanes.AddRange(mScene.DrawPackagesPlanesBackground);
			
			List<DrawPackage> DrawPackagesGame = new List<DrawPackage>();
			DrawPackagesGame.AddRange(mScene.DrawPackagesGame);
			DrawPackagesGame.Add(mHansel.DrawPackage);
			DrawPackagesGame.Add(mGretel.DrawPackage);
			

			//ToDo: DrawPackage Sorting!

			//---------------Draw
			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(mRenderTarget);
			DrawBackground();

			Matrix TmpTransformation = mCamera.GetTranslationMatrix();
			
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, TmpTransformation);
			mSkeletonRenderer.Effect.World = TmpTransformation;
			foreach (DrawPackage dPack in DrawPackagesPlanes)
				dPack.Draw(mSpriteBatch, mSkeletonRenderer);
			foreach(DrawPackage dPack in DrawPackagesGame)
				dPack.Draw(mSpriteBatch, mSkeletonRenderer);
			mSpriteBatch.End();

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
