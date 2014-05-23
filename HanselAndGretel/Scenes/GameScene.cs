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
		protected Texture2D mActionButton;

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
			mActionButton = TextureManager.Instance.GetElementByString("button_x");
			mSavegame = Savegame.Load();
			//-----Workaround für linerare Abhängigkeit HG_Game -> HG_Data -> KryptonEngine-----
			#region Setup InteractiveObjects.ActivityState
			for (int i = 0; i < mSavegame.Scenes.Length; i++)
			{
				foreach (InteractiveObject iObj in mSavegame.Scenes[i].InteractiveObjects)
				{
					switch (iObj.Activity)
					{
						case Activity.CaughtInCobweb:
							iObj.ActivityState = new CaughtInCobweb(mHansel, mGretel);
							break;
						case Activity.CaughtInSwamp:
							iObj.ActivityState = new CaughtInSwamp(mHansel, mGretel);
							break;
						case Activity.KnockOverTree:
							iObj.ActivityState = new KnockOverTree(mHansel, mGretel);
							break;
						case Activity.BalanceOverTree:
							iObj.ActivityState = new KnockOverTree(mHansel, mGretel);
							iObj.ActivityState.m2ndState = true;
							break;
						case Activity.PushRock:
							iObj.ActivityState = new PushRock(mHansel, mGretel);
							break;
						case Activity.SlipThroughRock:
							iObj.ActivityState = new SlipThroughRock(mHansel, mGretel);
							break;
						case Activity.Crawl:
							iObj.ActivityState = new Crawl(mHansel, mGretel);
							break;
						case Activity.JumpOverGap:
							iObj.ActivityState = new JumpOverGap(mHansel, mGretel);
							break;
						case Activity.LegUp:
							iObj.ActivityState = new LegUp(mHansel, mGretel);
							break;
						case Activity.LegUpGrab:
							iObj.ActivityState = new LegUpGrab(mHansel, mGretel);
							break;
						case Activity.UseKey:
							iObj.ActivityState = new UseKey(mHansel, mGretel);
							break;
						case Activity.PullDoor:
							iObj.ActivityState = new UseKey(mHansel, mGretel);
							iObj.ActivityState.m2ndState = true;
							break;
						case Activity.UseChalk:
							iObj.ActivityState = new UseChalk(mHansel, mGretel);
							break;
						case Activity.UseWell:
							iObj.ActivityState = new UseWell(mHansel, mGretel);
							break;
						default:
							throw new Exception("Im InteractiveObject " + iObj.ObjectId.ToString() + " in Scene " + i.ToString() + "ist eine ungültige Action angegeben!");
					}
				}
			}
			#endregion

			mScene = mSavegame.Scenes[mSavegame.SceneId];
			mHansel.LoadContent();
			mGretel.LoadContent();
			mHansel.mCurrentActivity = new None();
			mGretel.mCurrentActivity = new None();
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
			mHansel.Update(mLogic.HanselMayMove, mHansel.mCurrentActivity.mMovementSpeedFactorHansel, mScene);
			mGretel.Update(mLogic.GretelMayMove, mGretel.mCurrentActivity.mMovementSpeedFactorGretel, mScene);
			//Update Camera
			mCamera.MoveCamera(mHansel.CollisionBox, mGretel.CollisionBox);
		}

		public override void Draw()
		{
			//---------------Collect Packages
			List<DrawPackage> DrawPackagesGame = new List<DrawPackage>();
			DrawPackagesGame.AddRange(mScene.DrawPackagesGame);
			DrawPackagesGame.Add(mHansel.DrawPackage);
			DrawPackagesGame.Add(mGretel.DrawPackage);
			DrawPackagesGame.Add(new DrawPackage(new Vector2(mHansel.PositionX - 50, mHansel.PositionY - 100), 0f, mActionButton.Bounds, Color.White, mActionButton, mLogic.ActivityHandler.ActionInfoVisibilityHansel));
			DrawPackagesGame.Add(new DrawPackage(new Vector2(mGretel.PositionX - 50, mGretel.PositionY - 100), 0f, mActionButton.Bounds, Color.White, mActionButton, mLogic.ActivityHandler.ActionInfoVisibilityGretel));

			//ToDo: DrawPackage Sorting!

			//---------------Draw
			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(mRenderTarget);
			DrawBackground();

			Matrix TmpTransformation = mCamera.GetTranslationMatrix();
			
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, TmpTransformation);
			mSkeletonRenderer.Effect.World = TmpTransformation;
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
