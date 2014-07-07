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
			mHansel = new Hansel("spineboy");
			mGretel = new Gretel("spineboy");
			//Camera
			mCamera = new Camera();
			//Savegame
			mSavegame = new Savegame();
			//Logic
			mLogic = new Logic();
		}

		public override void LoadContent()
		{
			//Logic
			mLogic.LoadContent();

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
			//--------------------Renderer (Game & Lighting)--------------------
			mScene.RenderList = mScene.RenderList.OrderBy(iobj => iobj.DrawZ).ToList();
			mRenderer.SetGBuffer();
			mRenderer.ClearGBuffer();
			mRenderer.Begin(mCamera.Transform);
				//Render Background
				mRenderer.Draw(mScene.BackgroundTexture.Textures, Vector2.Zero);
				//Render Game
				foreach (InteractiveObject iObj in mScene.RenderList)
					mRenderer.Draw(iObj.Skeleton, iObj.Textures, iObj.DrawZ);
			mRenderer.End();

			//--------------------SpriteBatch (HUD & Infos)--------------------
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, mCamera.Transform);
				//Render ActionInfo
				mLogic.ActivityHandler.DrawActionInfo(mSpriteBatch, mHansel, mGretel);
				//Render ChalkMenues
				foreach (InteractiveObject iObj in mScene.InteractiveObjects)
					if (iObj.Activity == Activity.UseChalk)
						((UseChalk)iObj.ActivityState).DrawMenues(mSpriteBatch);
			mSpriteBatch.End();

			//--------------------DrawToScreen--------------------
			mRenderer.DrawDebugRendertargets(mSpriteBatch);
		}

		#endregion
	}
}
