using KryptonEngine;
using KryptonEngine.Controls;
using KryptonEngine.Entities;
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
			Successfull,
			Failed
		};

		public QTEState State;

		protected Random Randomizer;

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

		#endregion

		#region Constructor

		public QuickTimeEvent(InputHelper pInputHansel, InputHelper pInputGretel, float pDelayHansel = 1000f, float pDelayGretel = 1000f, float pSlowdownHansel = 1f, float pSlowdownGretel = 1f)
			: base()
		{
			InputHansel = pInputHansel;
			InputGretel = pInputGretel;
			DelayHansel = pDelayHansel;
			DelayGretel = pDelayGretel;
			SlowdownHansel = pSlowdownHansel;
			SlowdownGretel = pSlowdownGretel;
			ResetTimer();
		}

		#endregion

		#region Methods

		public void Update()
		{
			if (GetCurrentTimeoutProgress() > 1f)
				NotPressed();
			if (((State == QTEState.Hansel || State == QTEState.HanselTurnAround) && InputHansel.InputJustPressed(CurrentInputHansel)) ||
				((State == QTEState.Gretel || State == QTEState.GretelTurnAround) && InputGretel.InputJustPressed(CurrentInputGretel)))
				Pressed();
		}

		protected void Pressed()
		{
			switch(State)
			{
				case QTEState.Hansel:
					State = QTEState.Gretel;
					ResetTimer();
					break;
				case QTEState.Gretel:
					State = QTEState.Hansel;
					ResetTimer();
					break;
				case QTEState.HanselTurnAround:
					State = QTEState.Hansel;
					ResetTimer();
					break;
				case QTEState.GretelTurnAround:
					State = QTEState.Gretel;
					ResetTimer();
					break;
			}
		}

		protected void NotPressed()
		{
			switch (State)
			{
				case QTEState.Hansel:
					State = QTEState.GretelTurnAround;
					ResetTimer();
					break;
				case QTEState.Gretel:
					State = QTEState.HanselTurnAround;
					ResetTimer();
					break;
				case QTEState.HanselTurnAround:
					//TOD
					break;
				case QTEState.GretelTurnAround:
					//TOD
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
			int RandomChoice = Randomizer.Next(4);
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
