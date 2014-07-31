using HanselAndGretel.Data;
using KryptonEngine.AI;
using KryptonEngine.Entities;
using KryptonEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class Logic
	{
		#region Properties

		public SceneSwitchHandler SceneSwitchHandler;
		public ActivityHandler ActivityHandler;
		public ItemHandler ItemHandler;
		public EventHandler EventHandler;
		public TemperatureHandler TemperatureHandler;
		public EnemyHandler EnemyHandler;

		public bool HanselMayMove;
		public bool GretelMayMove;

		#endregion

		#region Constructor

		public Logic()
		{
			Initialize();
		}

		#endregion

		#region Methods

		public void Initialize()
		{
			SceneSwitchHandler = new SceneSwitchHandler();
			ActivityHandler = new ActivityHandler();
			ItemHandler = new ItemHandler();
			EventHandler = new EventHandler();
			TemperatureHandler = new TemperatureHandler();
			EnemyHandler = new EnemyHandler();
		}

		public void LoadContent()
		{
			ActivityHandler.LoadContent();
		}

		public void Update(Savegame pSavegame, ref SceneData pScene, Hansel pHansel, Gretel pGretel, Camera pCamera, TwoDRenderer pRenderer, ref GameScene.GameState pGameState)
		{
			//Update Logic Parts
			SceneSwitchHandler.Update(pSavegame, ref pScene, pHansel, pGretel, pCamera, pRenderer);
			ActivityHandler.Update(pScene, pHansel, pGretel, pSavegame);
			ItemHandler.Update(pScene, pHansel, pGretel, pSavegame, ref pGameState);
			EventHandler.Update(pScene, pHansel, pGretel);
			TemperatureHandler.Update(pHansel, pGretel);

			AIManager.Instance.Update();

			EnemyHandler.Update();
			SoundHandler.Instance.Update();

			//Check whether Player may move
			HanselMayMove = true;
			GretelMayMove = true;
			if (SceneSwitchHandler.CurrentState != SceneSwitchHandler.State.Idle)
			{
				HanselMayMove = false;
				GretelMayMove = false;
			}
		}

		#endregion
	}
}
