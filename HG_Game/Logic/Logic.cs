﻿using HanselAndGretel.Data;
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

		public void LoadContent()
		{
			ActivityHandler.LoadContent();
		}

		public void Update(Savegame pSavegame, ref SceneData pScene, Hansel pHansel, Gretel pGretel, Camera pCamera, TwoDRenderer pRenderer)
		{
			//Update Logic Parts
			SceneSwitchHandler.Update(pSavegame, ref pScene, pHansel, pGretel, pCamera, pRenderer);
			ActivityHandler.Update(pScene, pHansel, pGretel);
			ItemHandler.Update(pScene, pHansel, pGretel, pSavegame);

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
