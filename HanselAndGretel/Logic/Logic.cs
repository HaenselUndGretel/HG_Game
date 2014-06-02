using HanselAndGretel.Data;
using KryptonEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	public class Logic
	{
		#region Properties

		public SceneSwitchHandler SceneSwitchHandler;
		public ActivityHandler ActivityHandler;
		public ItemHandler ItemHandler;

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
		}

		public void Update(Savegame pSavegame, ref SceneData pScene, Hansel pHansel, Gretel pGretel, Camera pCamera)
		{
			//Update Logic Parts
			SceneSwitchHandler.Update(pSavegame, ref pScene, pHansel, pGretel, pCamera);
			ActivityHandler.Update(pScene, pHansel, pGretel);
			ItemHandler.Update(pScene, pHansel, pGretel);

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
