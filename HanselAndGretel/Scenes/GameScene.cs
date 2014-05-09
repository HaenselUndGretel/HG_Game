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
			mSavegame = new Savegame();
		}

		public override void LoadContent()
		{
			//mSavegame.Load();
			mSavegame = new Savegame();
			mSavegame.Scenes = new SceneData[1]{new SceneData()};
			mScene = mSavegame.Scenes[0];
			mScene.MoveArea = new List<Rectangle>();
			mScene.MoveArea.Add(new Rectangle(50, 50, 50, 50));
			mScene.MoveArea.Add(new Rectangle(500, 500, 20, 20));
			mScene.MoveArea.Add(new Rectangle(800, 400, 100, 300));
			mHansel = new Hansel(new Vector2(50,500));
			mGretel = new Gretel(new Vector2(100, 300));
		}

		public override void Update()
		{
			List<Rectangle> TmpMoveArea = new List<Rectangle>(mScene.MoveArea);
			TmpMoveArea.Add(mGretel.CollisionBox);
			mHansel.Update(TmpMoveArea);
			TmpMoveArea = new List<Rectangle>(mScene.MoveArea);
			TmpMoveArea.Add(mHansel.CollisionBox);
			mGretel.Update(TmpMoveArea);
		}

		public override void Draw()
		{
			//Collect Packages
			List<DrawPackage> DrawPackages = new List<DrawPackage>();
			DrawPackages.Add(mHansel.DrawPackage);
			DrawPackages.Add(mGretel.DrawPackage);
			DrawPackages.AddRange(mScene.DrawPackages);

			//ToDo: DrawPackag Sorting!

			//Draw
			DrawBackground();
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTranslationMatrix());
			foreach(DrawPackage dPack in DrawPackages)
			{
				dPack.Draw(mSpriteBatch, mCamera.Position);
			}
			mSpriteBatch.End();
		}

		#endregion
	}
}
