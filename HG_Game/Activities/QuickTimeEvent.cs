﻿using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Controls;
using KryptonEngine.Entities;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class QuickTimeEvent : BaseObject
	{
		#region Properties

		public enum QTEState
		{
			Hansel,
			Gretel,
			HanselTurnAround,
			GretelTurnAround,
			Finished,
			Successfull,
			Failed
		};

		public QTEState State;

		//Timing
		protected float DelayHansel;
		protected float DelayGretel;

		protected double TimerHansel;
		protected double TimerGretel;

		protected float SlowdownHansel;
		protected float SlowdownGretel;

		protected const float SlowdownTurnAround = 0.5f;

		//InputHelper
		protected InputHelper InputHansel;
		protected InputHelper InputGretel;

		//Inputs
		protected InputHelper.Input CurrentInputHansel;
		protected InputHelper.Input CurrentInputGretel;

		//protected InputHelper.Input NextInputHansel;
		//protected InputHelper.Input NextInputGretel;

		//Progress
		public float Progress;
		protected float ProgressSteps;
		protected bool KeepProgress; //Save Progress between runs?

		//QTE behavior
		protected bool OnlyX;
		protected bool OnlyOnePlayer;
		public bool OnlyOnePlayerIsHansel;

		//ButtonHUD
		HudFading ButtonFading;
		Texture2D ButtonHansel;
		Texture2D ButtonGretel;
		Vector2 ButtonOffset;

		#endregion

		#region Constructor

		public QuickTimeEvent(InputHelper pInputHansel, InputHelper pInputGretel, bool pKeepProgress = true, bool pOnlyX = false, bool pOnlyOnePlayer = false , bool pOnlyOnePlayerIsHansel = false, float pProgressSteps = 0.1f, float pDelayHansel = 1000f, float pDelayGretel = 1000f, float pSlowdownHansel = 1f, float pSlowdownGretel = 1f)
			: base()
		{
			InputHansel = pInputHansel;
			InputGretel = pInputGretel;
			ProgressSteps = pProgressSteps;
			DelayHansel = pDelayHansel;
			DelayGretel = pDelayGretel;
			SlowdownHansel = pSlowdownHansel;
			SlowdownGretel = pSlowdownGretel;
			Progress = 0;
			KeepProgress = pKeepProgress;
			OnlyX = pOnlyX;
			OnlyOnePlayer = pOnlyOnePlayer;
			OnlyOnePlayerIsHansel = pOnlyOnePlayerIsHansel;
			//ResetTimer();
			ButtonFading = new HudFading(0.5f, 0.5f, OnlyX);
			ButtonOffset = new Vector2(-30, -150);
		}

		#endregion

		#region Methods

		public void Update()
		{
			if (((State == QTEState.Hansel || State == QTEState.HanselTurnAround) && PressedWrongInput(true)) ||
				((State == QTEState.Gretel || State == QTEState.GretelTurnAround) && PressedWrongInput(false)) ||
				(GetCurrentTimeoutProgress() > 1f && !OnlyX))
			{
				NotPressed();
				return;
			}
			if (((State == QTEState.Hansel || State == QTEState.HanselTurnAround) && InputHansel.InputJustPressed(CurrentInputHansel)) ||
				((State == QTEState.Gretel || State == QTEState.GretelTurnAround) && InputGretel.InputJustPressed(CurrentInputGretel)))
				Pressed();
			UpdateButtonHud();
			if (State == QTEState.Finished && ButtonFading.VisibilityHansel == 0f && ButtonFading.VisibilityGretel == 0)
				State = QTEState.Successfull;
		}

		public void UpdateButtonHud()
		{
			UpdateButtonTextures();
			ButtonFading.ShowHudHansel = false;
			ButtonFading.ShowHudGretel = false;
			if (State == QTEState.Hansel || State == QTEState.HanselTurnAround)
				ButtonFading.ShowHudHansel = true;
			if (State == QTEState.Gretel || State == QTEState.GretelTurnAround)
				ButtonFading.ShowHudGretel = true;
			if (State == QTEState.HanselTurnAround)
				ButtonFading.VisibilityHansel = 1f;
			if (State == QTEState.GretelTurnAround)
				ButtonFading.VisibilityGretel = 1f;

			if (State == QTEState.Finished || State == QTEState.Failed)
			{
				ButtonFading.VisibilityHansel = 0f;
				ButtonFading.VisibilityGretel = 0f;
			}

			ButtonFading.Update();
		}

		protected void UpdateButtonTextures()
		{
			if (CurrentInputHansel == InputHelper.mAction)
				ButtonHansel = TextureManager.Instance.GetElementByString("button_x");
			else if (CurrentInputHansel == InputHelper.mBack)
				ButtonHansel = TextureManager.Instance.GetElementByString("button_b");
			else if (CurrentInputHansel == InputHelper.mSwitchItem)
				ButtonHansel = TextureManager.Instance.GetElementByString("button_y");
			else if (CurrentInputHansel == InputHelper.mUseItem)
				ButtonHansel = TextureManager.Instance.GetElementByString("button_a");

			if (CurrentInputGretel == InputHelper.mAction)
				ButtonGretel = TextureManager.Instance.GetElementByString("button_x");
			else if (CurrentInputGretel == InputHelper.mBack)
				ButtonGretel = TextureManager.Instance.GetElementByString("button_b");
			else if (CurrentInputGretel == InputHelper.mSwitchItem)
				ButtonGretel = TextureManager.Instance.GetElementByString("button_y");
			else if (CurrentInputGretel == InputHelper.mUseItem)
				ButtonGretel = TextureManager.Instance.GetElementByString("button_a");
		}

		public void DrawButtonHud(SpriteBatch pSpriteBatch, Player pHansel, Player pGretel)
		{
			if (ButtonHansel != null)
				pSpriteBatch.Draw(ButtonHansel, pHansel.SkeletonPosition + ButtonOffset, Color.White * ButtonFading.VisibilityHansel);
			if (ButtonGretel != null)
				pSpriteBatch.Draw(ButtonGretel, pGretel.SkeletonPosition + ButtonOffset, Color.White * ButtonFading.VisibilityGretel);
		}

		protected bool PressedWrongInput(bool pHansel)
		{
			//Wenn nur X gedrückt werden soll passiert nichts wenn etwas anderes gedrückt wird
			if (OnlyX)
				return false;
			//Abzufragende States ermitteln
			InputHelper input;
			InputHelper.Input RightInput;
			if (pHansel)
			{
				input = InputHansel;
				RightInput = CurrentInputHansel;
			}
			else
			{
				input = InputGretel;
				RightInput = CurrentInputGretel;
			}
			//Eigentliche Abfrage
			if ((input.InputJustPressed(InputHelper.mAction) && RightInput != InputHelper.mAction) ||
				(input.InputJustPressed(InputHelper.mBack) && RightInput != InputHelper.mBack) ||
				(input.InputJustPressed(InputHelper.mSwitchItem) && RightInput != InputHelper.mSwitchItem) ||
				(input.InputJustPressed(InputHelper.mUseItem) && RightInput != InputHelper.mUseItem))
			{
				return true;
			}
			return false;
		}

		public void StartQTE()
		{
			if (!KeepProgress)
				Progress = 0;
			CurrentInputHansel = GetRandomInput();
			CurrentInputGretel = GetRandomInput();
			if (OnlyOnePlayer)
			{
				if (OnlyOnePlayerIsHansel)
					State = QTEState.Hansel;
				else
					State = QTEState.Gretel;
			}
			else
			{
				if (EngineSettings.Randomizer.Next(2) == 1)
					State = QTEState.Hansel;
				else
					State = QTEState.Gretel;
			}
			ResetTimer();
		}

		protected void Pressed()
		{
			Progress += ProgressSteps;
			if (Progress >= 1f)
				State = QTEState.Finished;
			switch(State)
			{
				case QTEState.Hansel:
					if (OnlyOnePlayer)
					{
						State = QTEState.Hansel;
						CurrentInputHansel = GetRandomInput();
					}
					else
					{
						State = QTEState.Gretel;
						CurrentInputGretel = GetRandomInput();
					}
					ResetTimer();
					break;
				case QTEState.Gretel:
					if (OnlyOnePlayer)
					{
						State = QTEState.Gretel;
						CurrentInputGretel = GetRandomInput();
					}
					else
					{
						State = QTEState.Hansel;
						CurrentInputHansel = GetRandomInput();
					}
					ResetTimer();
					break;
				case QTEState.HanselTurnAround:
					State = QTEState.Hansel;
					CurrentInputHansel = GetRandomInput();
					ResetTimer();
					break;
				case QTEState.GretelTurnAround:
					State = QTEState.Gretel;
					CurrentInputGretel = GetRandomInput();
					ResetTimer();
					break;
			}
		}

		protected void NotPressed()
		{
			switch (State)
			{
				case QTEState.Hansel:
					if (OnlyOnePlayer)
					{
						State = QTEState.HanselTurnAround;
						CurrentInputHansel = GetRandomInput();
					}
					else
					{
						State = QTEState.GretelTurnAround;
						CurrentInputGretel = GetRandomInput();
					}
					ResetTimer();
					break;
				case QTEState.Gretel:
					if (OnlyOnePlayer)
					{
						State = QTEState.GretelTurnAround;
						CurrentInputGretel = GetRandomInput();
					}
					else
					{
						State = QTEState.HanselTurnAround;
						CurrentInputHansel = GetRandomInput();
					}
					ResetTimer();
					break;
				case QTEState.HanselTurnAround:
					State = QTEState.Failed;
					break;
				case QTEState.GretelTurnAround:
					State = QTEState.Failed;
					break;
			}
		}

		protected void ResetTimer()
		{
			TimerHansel = EngineSettings.Time.TotalGameTime.TotalMilliseconds;
			TimerGretel = EngineSettings.Time.TotalGameTime.TotalMilliseconds;
		}

		protected InputHelper.Input GetRandomInput()
		{
			if (OnlyX)
				return InputHelper.mAction;
			int RandomChoice = EngineSettings.Randomizer.Next(4);
			switch (RandomChoice)
			{
				case 0:
					return InputHelper.mAction;
				case 1:
					return InputHelper.mBack;
				case 2:
					return InputHelper.mSwitchItem;
				case 3:
					return InputHelper.mUseItem;
			}
			throw new Exception("Randomizer hat " + RandomChoice.ToString() + " raus geworfen. Kann das sein?");
		}

		public float GetCurrentTimeoutProgress()
		{
			if (State == QTEState.Finished || State == QTEState.Successfull) //Sollte eigentlich nie abgefragt werden wenn QTE bereits Successfull war
				return 1.1f;
			switch (State)
			{
				case QTEState.Hansel:
					return (float)(EngineSettings.Time.ElapsedGameTime.TotalMilliseconds - TimerHansel) / (DelayHansel * SlowdownHansel);
				case QTEState.Gretel:
					return (float)(EngineSettings.Time.ElapsedGameTime.TotalMilliseconds - TimerGretel) / (DelayGretel * SlowdownGretel);
				case QTEState.HanselTurnAround:
					return (float)(EngineSettings.Time.ElapsedGameTime.TotalMilliseconds - TimerHansel) / (DelayHansel * SlowdownHansel * SlowdownTurnAround);
				case QTEState.GretelTurnAround:
					return (float)(EngineSettings.Time.ElapsedGameTime.TotalMilliseconds - TimerGretel) / (DelayGretel * SlowdownGretel * SlowdownTurnAround);
			}
			throw new Exception("Progress bei dafür nicht gültigem State abgefragt.");
		}

		#endregion
	}
}
