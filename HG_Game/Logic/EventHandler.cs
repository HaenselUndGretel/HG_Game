using HanselAndGretel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class EventHandler
	{
		#region Constructor

		public EventHandler()
		{

		}

		#endregion

		#region Methods

		public void Update(SceneData pScene, Hansel pHansel, Gretel pGretel)
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
