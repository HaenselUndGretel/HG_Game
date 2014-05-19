using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
using KryptonEngine.SceneManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	public class GameScene : Scene
	{
		#region Properties

		protected Savegame mSavegame;
		protected Hansel mHansel;
		protected Gretel mGretel;
		protected SceneData mScene;

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
			mHansel.Position = mSavegame.PositionHansel;
			mGretel.Position = mSavegame.PositionGretel;
			mHansel.LoadReferences(mCamera, mGretel, mScene);
			mGretel.LoadReferences(mCamera, mHansel, mScene);
		}

		public override void Update()
		{
			mHansel.Update();
			mGretel.Update();
			mCamera.MoveCamera(mHansel.CollisionBox, mGretel.CollisionBox);
		}

		public override void Draw()
		{
			//Collect Packages
			List<DrawPackage> DrawPackagesPlanes = new List<DrawPackage>();
			DrawPackagesPlanes.AddRange(mScene.DrawPackagesPlanesBackground);
			
			List<DrawPackage> DrawPackagesGame = new List<DrawPackage>();
			DrawPackagesGame.Add(mHansel.DrawPackage);
			DrawPackagesGame.Add(mGretel.DrawPackage);
			DrawPackagesGame.AddRange(mScene.DrawPackagesGame);

			//ToDo: DrawPackage Sorting!

			//Draw
			DrawBackground();
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTranslationMatrix());
			//mSpriteBatch.Begin();
			foreach(DrawPackage dPack in DrawPackagesGame)
			{
				dPack.Draw(mSpriteBatch, mCamera.Position);
			}
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
