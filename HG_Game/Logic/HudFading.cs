using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HanselAndGretel.Data;
using KryptonEngine;
using Microsoft.Xna.Framework;

namespace HG_Game
{
	public class HudFading
	{
		#region Properties

		protected struct FadingState
		{
			public bool ShowHud;
			public float Visibility;
			public double FadingTimer;
			public float FadingDuration;
			public bool Flashing;
		};

		protected FadingState StateHansel;
		protected FadingState StateGretel;

		#endregion

		#region Getter & Setter

		public bool ShowHudHansel { get { return StateHansel.ShowHud; } set { StateHansel.ShowHud = value; } }
		public bool ShowHudGretel { get { return StateGretel.ShowHud; } set { StateGretel.ShowHud = value; } }

		public float VisibilityHansel { get { return StateHansel.Visibility; } }
		public float VisibilityGretel { get { return StateGretel.Visibility; } }

		#endregion

		#region Constructor

		public HudFading(float pFadingDurationHansel = 1f, float pFadingDurationGretel = 1f, bool pFlashing = false)
		{
			StateHansel.FadingDuration = pFadingDurationHansel;
			StateGretel.FadingDuration = pFadingDurationGretel;
			StateHansel.Flashing = pFlashing;
			StateGretel.Flashing = pFlashing;
		}

		#endregion

		#region Methods

		public void Update()
		{
			//Update States
			StateHansel.FadingTimer += EngineSettings.Time.ElapsedGameTime.TotalSeconds;
			StateGretel.FadingTimer += EngineSettings.Time.ElapsedGameTime.TotalSeconds;

			if (StateHansel.Flashing && StateHansel.FadingTimer > StateHansel.FadingDuration)
				SwitchShowHud(true);
			if (StateGretel.Flashing && StateGretel.FadingTimer > StateGretel.FadingDuration)
				SwitchShowHud(false);

			float TmpDeltaHansel = (float)(EngineSettings.Time.ElapsedGameTime.TotalSeconds / (double)StateHansel.FadingDuration);
			float TmpDeltaGretel = (float)(EngineSettings.Time.ElapsedGameTime.TotalSeconds / (double)StateGretel.FadingDuration);

			//Update Fading
			if (!StateHansel.ShowHud)
				TmpDeltaHansel = -TmpDeltaHansel;
			if (!StateGretel.ShowHud)
				TmpDeltaGretel = -TmpDeltaGretel;

			StateHansel.Visibility += TmpDeltaHansel;
			StateGretel.Visibility += TmpDeltaGretel;

			StateHansel.Visibility = MathHelper.Clamp(StateHansel.Visibility, 0f, 1f);
			StateGretel.Visibility = MathHelper.Clamp(StateGretel.Visibility, 0f, 1f);
		}

		/// <summary>
		/// Ändert ShowHud
		/// </summary>
		/// <param name="pHansel">true = Hansel wird beeinflusst, false = Gretel</param>
		/// <param name="PreciseValue">false = ShowHud wird geswitched, bei true wird ShowHud auf pShoHud gesetzt</param>
		/// <param name="pShowHud">Wert für ShowHud, wird nur beachtet wenn PreciseValue auf true gesetzt ist</param>
		/// <param name="pResetTimer">Ob der FadingTimer zurückgesetzt werden soll</param>
		public void SwitchShowHud(bool pHansel, bool PreciseValue = false, bool pShowHud = true, bool pResetTimer = true)
		{
			if (!PreciseValue)
			{
				if (pHansel)
				{
					StateHansel.ShowHud = !StateHansel.ShowHud;
					if (pResetTimer)
						StateHansel.FadingTimer = 0;
				}
				else
				{
					StateGretel.ShowHud = !StateGretel.ShowHud;
					if (pResetTimer)
						StateGretel.FadingTimer = 0;
				}
				return;
			}
			if (pHansel)
			{
				StateHansel.ShowHud = pShowHud;
				if (pResetTimer)
					StateHansel.FadingTimer = 0;
			}
			else
			{
				StateGretel.ShowHud = pShowHud;
				if (pResetTimer)
					StateGretel.FadingTimer = 0;
			}
		}

		#endregion

	}
}