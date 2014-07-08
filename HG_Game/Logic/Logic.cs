using HanselAndGretel.Data;
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

			UpdateEventTrigger(pScene, pHansel, pGretel);
		}

		private void UpdateEventTrigger(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			if (pScene.Events.Count == 0) return;

			List<EventTrigger> delEvents = new List<EventTrigger>();
			foreach (EventTrigger et in pScene.Events)
			{
				if (et.CollisionBox.Contains(pHansel.CollisionBox) || et.CollisionBox.Contains(pGretel.CollisionBox))
				{
					et.TriggerActivity();
					delEvents.Add(et);
				}
			}

			foreach (EventTrigger et in delEvents)
				pScene.Events.Remove(et);

		}
		#endregion
	}
}
