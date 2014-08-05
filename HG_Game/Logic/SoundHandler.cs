using KryptonEngine;
using KryptonEngine.FModAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class SoundHandler
	{
		#region Singleton
		private static SoundHandler instance;
		public static SoundHandler Instance { get { if (instance == null) instance = new SoundHandler(); return instance; } }
		#endregion

		private float TimeInScene;
		private bool SecondChannelPlay;

		private bool MuteChannel3Scene0;
		private bool MuteChannel1Scene0;

		private bool BossFightMusic;

		public SoundHandler()
		{
			TimeInScene = 0;
			SecondChannelPlay = false;
		}

		public void Update()
		{
			TimeInScene += EngineSettings.Time.ElapsedGameTime.Milliseconds;

			if (TimeInScene > 10000 & !SecondChannelPlay && GameReferenzes.SceneID != 0 )
			{
				FmodMediaPlayer.Instance.FadeBackgroundChannelIn(1);
				SecondChannelPlay = true;
			}

			if (GameReferenzes.SceneID == 0)
			{
				if(TimeInScene > 10000 & !MuteChannel3Scene0)
				{
					FmodMediaPlayer.Instance.FadeBackgroundChannelOut(3);
					MuteChannel3Scene0 = true;
				}
				if (TimeInScene > 20000 & !MuteChannel1Scene0)
				{
					FmodMediaPlayer.Instance.FadeBackgroundChannelOut(1);
					MuteChannel1Scene0 = true;
				}
			}

			if (GameReferenzes.SceneID == 16 & !BossFightMusic && !GameReferenzes.IsSceneSwitching)
			{
				BossFightMusic = true;
				FmodMediaPlayer.Instance.FadeBackgroundChannelIn(1);
				FmodMediaPlayer.Instance.FadeBackgroundChannelIn(3);
				FmodMediaPlayer.Instance.FadeBackgroundChannelIn(4);
			}

		}

		public void ResetTime()
		{
			TimeInScene = 0;
			SecondChannelPlay = false;
		}
	}
}
