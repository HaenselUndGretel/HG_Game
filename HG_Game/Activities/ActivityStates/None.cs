using KryptonEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class None : ActivityState
	{
		public None()
			:base()
		{
			mMovementSpeedFactorHansel = 1f;
			mMovementSpeedFactorGretel = 1f;
		}

	}
}
