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

		public void Update(Savegame pSavegame, SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			SceneSwitch.TestForSwitch(pScene, pHansel, pGretel, pSavegame.Scenes);
			if (SceneSwitch.Switching)
				SceneSwitch.DoSwitch();
		}

		#endregion
	}
}
