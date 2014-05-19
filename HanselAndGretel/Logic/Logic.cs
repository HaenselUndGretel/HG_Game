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

		#endregion
	}
}
