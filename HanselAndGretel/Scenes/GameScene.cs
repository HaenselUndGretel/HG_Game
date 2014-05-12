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
			mHansel = new Hansel(new Vector2(50, 500));
			mGretel = new Gretel(new Vector2(100, 300));
			mHansel.LoadReferences(mCamera, mGretel, mScene);
			mGretel.LoadReferences(mCamera, mHansel, mScene);
		}

		public override void LoadContent()
		{
			Savegame.Load(mSavegame);
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
			List<DrawPackage> DrawPackages = new List<DrawPackage>();
			DrawPackages.Add(mHansel.DrawPackage);
			DrawPackages.Add(mGretel.DrawPackage);
			DrawPackages.AddRange(mScene.DrawPackages);

			//ToDo: DrawPackage Sorting!

			//Draw
			DrawBackground();
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTranslationMatrix());
			//mSpriteBatch.Begin();
			foreach(DrawPackage dPack in DrawPackages)
			{
				dPack.Draw(mSpriteBatch, mCamera.Position);
			}
			mSpriteBatch.End();
		}

		#endregion
	}
}
