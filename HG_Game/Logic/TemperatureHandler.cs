using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.FModAudio;
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

		private const float SOUND_COOLDOWN = 23000.0f;
		private float soundTimer;

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
				UpdateSound();
			}
			else
			{
				pHansel.BodyTemperature = 1f;
				pGretel.BodyTemperature = 1f;
				soundTimer = 0.0f;
			}
		}

		private void UpdateSound()
		{
			soundTimer += EngineSettings.Time.ElapsedGameTime.Milliseconds;
			if (soundTimer < SOUND_COOLDOWN) return;

			int player = EngineSettings.Randomizer.Next(0, 2);

			if(player == 0)
				FmodMediaPlayer.Instance.AddSong("gretel_shiver");
			else
				FmodMediaPlayer.Instance.AddSong("hansel_shiver");

			soundTimer -= SOUND_COOLDOWN;
		}

		#endregion

	}
}
