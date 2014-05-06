using DragonEngine;
using DragonEngine.Entities;
using DragonEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	public class GameScene : Scene
	{
		#region Properties

		#region Logic & Graphics

		public const float LogicFPS = 60;
		public const float LogicTick =  1 / LogicFPS;
		public double RemainingTime;

		public GameLogic GameLogic;
		public GameGraphics GameGraphics;

		#endregion

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
			RemainingTime = 0;
			GameLogic = new GameLogic();
			GameGraphics = new GameGraphics();
			
		}

		public override void LoadContent()
		{

		}

		public override void Update()
		{
			RemainingTime += EngineSettings.Time.ElapsedGameTime.Milliseconds;
			while (RemainingTime >= (double)LogicTick)
			{
				GameLogic.Update();
				RemainingTime -= (double)LogicTick;
			}
			float GIP = (float)(RemainingTime / (double)LogicTick);
			GameGraphics.Update(GIP);
		}

		public override void Draw()
		{
			
			GameGraphics.Draw();
		}

		#endregion
	}
}
