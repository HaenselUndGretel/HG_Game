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
		protected const float Distance = 300f;
		protected const float MinBodyTemperature = 0.5f;

		#endregion

		#region Constructor

		public TemperatureHandler()
		{
			Frost = new SteppingProgress(8f);
		}

		#endregion

		#region Methods

		public void Update(Hansel pHansel, Gretel pGretel)
		{
			if ((pGretel.SkeletonPosition - pHansel.SkeletonPosition).Length() > Distance)
				Frost.StepForward();
			else
				Frost.StepBackward();
			if (Frost.Progress > MinBodyTemperature)
			{
				pHansel.BodyTemperature = MinBodyTemperature + Frost.ProgressInverse;
				pGretel.BodyTemperature = MinBodyTemperature + Frost.ProgressInverse;
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
