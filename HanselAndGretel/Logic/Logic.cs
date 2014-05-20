using HanselAndGretel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	public class Logic
	{
		#region Properties

		public SceneSwitch SceneSwitch;

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
			SceneSwitch = new SceneSwitch();
		}

		public void Update(Savegame pSavegame, ref SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			//Update Logic Parts
			SceneSwitch.Update(pSavegame, ref pScene, pHansel, pGretel);

			//Check whether Player may move
			HanselMayMove = true;
			GretelMayMove = true;
			if (SceneSwitch.CurrentState != SceneSwitch.State.Idle)
			{
				HanselMayMove = false;
				GretelMayMove = false;
			}
		}

		#endregion
	}
}
