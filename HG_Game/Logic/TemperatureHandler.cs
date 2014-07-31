using HanselAndGretel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class TemperatureHandler
	{
		#region Properties

		protected SteppingProgress Frost;

		#endregion

		#region Constructor

		public TemperatureHandler()
		{
			Frost = new SteppingProgress(Hardcoded.Temp_SteppingDuration);
		}

		#endregion

		#region Methods

		public void Update(Hansel pHansel, Gretel pGretel)
		{
			if ((pGretel.SkeletonPosition - pHansel.SkeletonPosition).Length() > Hardcoded.Temp_Distance)
				Frost.StepForward();
			else
				Frost.StepBackward();
			if (Frost.Progress > Hardcoded.Temp_MinBodyTemperature)
			{
				pHansel.BodyTemperature = Hardcoded.Temp_MinBodyTemperature + Frost.ProgressInverse;
				pGretel.BodyTemperature = Hardcoded.Temp_MinBodyTemperature + Frost.ProgressInverse;
			}
			else
			{
				pHansel.BodyTemperature = 1f;
				pGretel.BodyTemperature = 1f;
			}
		}

		#endregion

	}
}
