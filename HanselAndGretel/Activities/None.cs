using KryptonEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	class None : ActivityState
	{
		public None()
			:base()
		{
			mMovementSpeedFactorHansel = 1f;
			mMovementSpeedFactorGretel = 1f;
		}

		public override Activity GetPossibleActivity(bool pContains)
		{
			return Activity.None;
		}
	}
}
