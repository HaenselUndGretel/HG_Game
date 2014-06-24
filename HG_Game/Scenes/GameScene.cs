﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.SceneManagement;
using KryptonEngine.Entities;
using HanselAndGretel.Data;
using KryptonEngine;
using Microsoft.Xna.Framework.Graphics;

namespace HG_Game
{
	public class GameScene : Scene
	{
		#region Properties

		protected Hansel mHansel;
		protected Gretel mGretel;

		protected Savegame mSavegame;
		protected SceneData mScene;

		protected Logic mLogic;

		protected Texture2D[] TmpSpineStuff;

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
			//Player
			mHansel = new Hansel("ashbrett");
			mGretel = new Gretel("sweetcheeks");
			//Camera
			mCamera = new Camera();
			//Savegame
			mSavegame = new Savegame();
			//Logic
			mLogic = new Logic();

			TmpSpineStuff = new Texture2D[4];
		}

		public override void LoadContent()
		{
			//Player
			mHansel.LoadContent();
			mGretel.LoadContent();
			mHansel.mCurrentActivity = ActivityHandler.None;
			mGretel.mCurrentActivity = ActivityHandler.None;
			mHansel.LoadReferences(mCamera, mGretel);
			mGretel.LoadReferences(mCamera, mHansel);

			//Savegame
			mSavegame = Savegame.Load(mHansel, mGretel);
			mScene = mSavegame.Scenes[mSavegame.SceneId];

			//Camera
			mCamera.GameScreen = mScene.GamePlane;

			for (int i = 0; i < 4; ++i)
				TmpSpineStuff[i] = KryptonEngine.Manager.TextureManager.Instance.GetElementByString("sweetcheeks");
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
			mScene.RenderList = mScene.RenderList.OrderBy(iobj => iobj.DrawZ).ToList();

			mRenderer.SetGBuffer();
			mRenderer.Begin(mCamera.Transform);
			mRenderer.Draw(mHansel.Skeleton, TmpSpineStuff);

			mRenderer.End();
			mRenderer.DrawDebugRendertargets(mSpriteBatch);
		}

		#endregion
	}
}
