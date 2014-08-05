using HanselAndGretel.Data;
using KryptonEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class LightHandler
	{
		public LightHandler()
		{

		}

		public void Update()
		{
			foreach (Light l in GameReferenzes.Level.Lights)
				l.Update();
		}
	}
}
