using KryptonEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class SteppingProgress
	{
		#region Properties

		protected float mProgress;
		protected float ProgressDuration;

		#endregion

		#region Getter & Setter

		protected float ProgressSpeed { get { return (float)(EngineSettings.Time.ElapsedGameTime.TotalSeconds / (double)ProgressDuration); } }
		public float Progress { get { return mProgress; } }
		public float ProgressInverse { get { return 1f - mProgress; } }
		public bool Complete { get { return (Progress >= 1f) ? true : false; } }

		#endregion

		#region Constructor

		public SteppingProgress(float pProgressDuration = 1f)
		{
			mProgress = 0f;
			ProgressDuration = pProgressDuration;
		}

		#endregion

		#region Methods

		public void StepForward()
		{
			mProgress += ProgressSpeed;
			if (mProgress > 1f)
				mProgress = 1f;
		}

		public void StepBackward()
		{
			mProgress -= ProgressSpeed;
			if (mProgress < 0f)
				mProgress = 0f;
		}

		public void StepFromRotation(float pRotation, float pSpeed, float pUpFrictionFactor)
		{ //Speed: Wieviel der Gesamtstrecke mit einer Controllerumdrehung zurück gelegt wird
			if (pRotation < 0)
				pSpeed *= pUpFrictionFactor;
			mProgress += pRotation * pSpeed;
		}

		public void Reset(bool pMax = false)
		{
			mProgress = 0f;
			if (pMax)
				mProgress = 1f;
		}

		#endregion
	}
}
