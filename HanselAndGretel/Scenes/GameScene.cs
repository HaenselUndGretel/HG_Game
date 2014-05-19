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
			mHansel.LoadReferences(mCamera, mGretel, mScene);
			mGretel.LoadReferences(mCamera, mHansel, mScene);
		}

		public override void Update()
		{
			//Update Player
			mHansel.Update();
			mGretel.Update();
			//Update Logic
			mLogic.Update(mSavegame, mScene, mHansel, mGretel);
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
			if (mLogic.SceneSwitch.Switching)
				mSpriteBatch.Draw(mRenderTarget, Vector2.Zero, new Color(mLogic.SceneSwitch.Fading, mLogic.SceneSwitch.Fading, mLogic.SceneSwitch.Fading));
			else
				mSpriteBatch.Draw(mRenderTarget, Vector2.Zero, Color.White);
			mSpriteBatch.End();
		}

		#endregion

		#region Methods

		public void SwitchScene(Waypoint pWPHansel, Waypoint pWPGretel)
		{
			if (pWPHansel.DestinationScene != pWPGretel.DestinationScene)
				throw new Exception("SceneSwitch Versuch mit unterschiedlichen Zielen. Was geht denn hier ab?!?");
			mScene = mSavegame.Scenes[pWPHansel.DestinationScene];
			mHansel.Position = mScene.Waypoints[pWPHansel.DestinationWaypoint].Position;
			mGretel.Position = mScene.Waypoints[pWPGretel.DestinationWaypoint].Position;
		}

		#endregion
	}
}
