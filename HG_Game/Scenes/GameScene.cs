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
			mHansel = new Hansel("skeleton");
			mGretel = new Gretel("skeleton");
			mHansel.mCurrentActivity = ActivityHandler.None;
			mGretel.mCurrentActivity = ActivityHandler.None;
			//Camera
			mCamera = new Camera();
			//Savegame
			mSavegame = new Savegame();
			//Logic
			mLogic = new Logic();
		}

		public override void LoadContent()
		{
			base.LoadContent();

			//Logic
			mLogic.LoadContent();

			//Player
			mHansel.LoadContent();
			mGretel.LoadContent();
			mHansel.mCurrentActivity = ActivityHandler.None;
			mGretel.mCurrentActivity = ActivityHandler.None;
			mHansel.LoadReferences(mCamera, mGretel);
			mGretel.LoadReferences(mCamera, mHansel);
			mHansel.SkeletonPosition = new Vector2(50, 100);
			mGretel.SkeletonPosition = new Vector2(50, 50);
			mHansel.MoveInteractiveObject(Vector2.Zero);
			mGretel.MoveInteractiveObject(Vector2.Zero);

			//Savegame
			mSavegame = Savegame.Load(mHansel, mGretel);
			mScene = mSavegame.Scenes[mSavegame.SceneId];
			mRenderer.AmbientLight = mScene.SceneAmbientLight;

			//Camera
			mCamera.GameScreen = mScene.GamePlane;

			//Set ActivityStates in IObjs
			mLogic.ActivityHandler.SetupInteractiveObjectsFromDeserialization(mSavegame, mHansel, mGretel);

			mGretel.Chalk = 2;
			mHansel.Inventory.TryToStore(new Branch("cobweb"));
			mHansel.Inventory.TryToStore(new Key("cobweb"));
			mHansel.Inventory.TryToStore(new Knife("cobweb"));
			mHansel.Inventory.GetItemByType(typeof(Branch)).LoadContent();
			mHansel.Inventory.GetItemByType(typeof(Key)).LoadContent();
			mHansel.Inventory.GetItemByType(typeof(Knife)).LoadContent();
		}

		public override void Update()
		{
			if (InputHelper.ButtonJustPressed2Player(Microsoft.Xna.Framework.Input.Buttons.Start))
			{
				FmodMediaPlayer.Instance.FadeBackgroundChannelIn(1);
				FmodMediaPlayer.Instance.FadeBackgroundChannelIn(2);
				FmodMediaPlayer.Instance.FadeBackgroundChannelIn(3);
				SceneManager.Instance.SetCurrentSceneTo("Menu");
			}

			//Update Logic
			mLogic.Update(mSavegame, ref mScene, mHansel, mGretel, mCamera, mRenderer);
			//Update Player
			mHansel.Update(mLogic.HanselMayMove, mHansel.mCurrentActivity.mMovementSpeedFactorHansel, mScene);
			mGretel.Update(mLogic.GretelMayMove, mGretel.mCurrentActivity.mMovementSpeedFactorGretel, mScene);
			//Update Camera
			mCamera.MoveCamera(mHansel.CollisionBox, mGretel.CollisionBox);
		}

		public override void Draw()
		{
			//--------------------Renderer (Game & Lighting)--------------------
			mScene.RenderList = mScene.RenderList.OrderBy(iobj => iobj.DrawZ).ToList(); //DrawZ unbrauchbar da sie nirgends relativ zur Welt geupdated wird

			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(null);

			mRenderer.SetGBuffer();
			mRenderer.ClearGBuffer();
			mRenderer.Begin(mCamera.Transform);
				//Render Background
				mScene.BackgroundTexture.Draw(mRenderer);
				//Render Game
				foreach (InteractiveObject iObj in mScene.RenderList)
					iObj.Draw(mRenderer);
			mRenderer.End();
			
			//--------------------DrawToScreen--------------------
			mRenderer.DisposeGBuffer();
			mRenderer.ProcessLight(mScene.Lights, mCamera.Transform);
			mRenderer.ProcessFinalScene();

			mRenderer.DrawFinalTargettOnScreen(mSpriteBatch);

			//--------------------SpriteBatch (HUD & Infos)--------------------
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, mCamera.Transform);
			//Render ActionInfo
			mLogic.ActivityHandler.DrawActionInfo(mSpriteBatch, mHansel, mGretel);
			//Render ChalkMenues
			foreach (InteractiveObject iObj in mScene.InteractiveObjects)
				if (iObj.ActivityId == Activity.UseChalk)
					((UseChalk)iObj.ActivityState).DrawMenues(mSpriteBatch);
			//Render Inventory
			mLogic.ItemHandler.DrawInventory(mHansel, mGretel, mSpriteBatch);
			//Render ButtonHud
			mLogic.ActivityHandler.DrawButtonHud(mSpriteBatch, mHansel, mGretel);
			mSpriteBatch.End();

#if DEBUG
			if (EngineSettings.IsDebug)
			{

				mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, mCamera.Transform);
				mScene.DrawDebug(mSpriteBatch);
				for (int i = mScene.RenderList.Count - 1; i >= 0; --i)
				{
					mScene.RenderList[i].DrawDebug(mSpriteBatch);
				}
				mSpriteBatch.End();

			SpriteFont font = FontManager.Instance.GetElementByString("font");
			StringBuilder sb = new StringBuilder();
			//sb.AppendLine("Hansel:" + mHansel.SkeletonPosition.ToString() + "," + mHansel.CollisionBox.ToString());
			//sb.AppendLine("Gretel:" + mGretel.SkeletonPosition.ToString() + "," + mGretel.CollisionBox.ToString());
			//sb.AppendLine("Camera:" + (mCamera.Position - new Vector2(EngineSettings.VirtualResWidth, EngineSettings.VirtualResHeight) / 2).ToString() + "," + mCamera.GameScreen.ToString());
			//sb.AppendLine("Hansel:"+mHansel.mCurrentState + " " +mHansel.mCurrentActivity.GetType().ToString());
			//sb.AppendLine("Gretel:" + mGretel.mCurrentState + " " + mGretel.mCurrentActivity.GetType().ToString());
			
			mSpriteBatch.Begin();
			//mSpriteBatch.DrawString(font, sb.ToString(), new Vector2(100, 100), Color.White);
			mSpriteBatch.End();
			}
#endif
		}

		#endregion
	}
}
