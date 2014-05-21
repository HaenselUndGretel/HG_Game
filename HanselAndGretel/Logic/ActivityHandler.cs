using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	public class ActivityHandler
	{
		#region Properties

		//ButtonFading
		public float ButtonShowingHansel;
		public float ButtonShowingGretel;
		protected const float ShowButtonFadingDuration = 1.0f;
		protected bool ShowButtonHansel;
		protected bool ShowButtonGretel;

		//Currently possible Activities
		public List<Activity> PossibleActivitiesHansel;
		public List<Activity> PossibleActivitiesGretel;

		#endregion

		#region Constructor

		public ActivityHandler()
		{
			Initialize();
		}

		#endregion

		#region Methods

		public void Initialize()
		{
			ButtonShowingHansel = 0;
			ButtonShowingGretel = 0;
		}

		public void Update(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			UpdatePossibleActivities(pScene, pHansel, pGretel);
			UpdateButtonShowing();
		}

		public void UpdatePossibleActivities(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			//Flush Lists
			PossibleActivitiesHansel = new List<Activity>();
			PossibleActivitiesGretel = new List<Activity>();

			//-----Add Activity when standing in ActionRect of InteractiveObject-----
			foreach (InteractiveObject iObj in pScene.InteractiveObjects)
			{
				foreach (Rectangle rect in iObj.ActionRectList)
				{
					if (rect.Intersects(pHansel.CollisionBox)) //Hansel kann mit Object interacten
					{
						PossibleActivitiesHansel.Add((Activity)iObj.ActionId);
					}
					if (rect.Intersects(pGretel.CollisionBox)) //Gretel kann mit Object interacten
					{
						PossibleActivitiesGretel.Add((Activity)iObj.ActionId);
					}
				}
			}

			//-----Test for other possible Activities-----
			//UseItem
			if (pHansel.mCurrentActivity == Activity.None)
				PossibleActivitiesHansel.Add(Activity.UseItem);
			if (pGretel.mCurrentActivity == Activity.None)
				PossibleActivitiesGretel.Add(Activity.UseItem);
			//SwitchItem
			if (pHansel.mCurrentActivity == Activity.None)
				PossibleActivitiesHansel.Add(Activity.SwitchItem);
			if (pGretel.mCurrentActivity == Activity.None)
				PossibleActivitiesGretel.Add(Activity.SwitchItem);


			//-----Activities-----
			//None,
			//
			//CaughtInCobweb,
			//FreeFromCobweb,
			//CaughtInSwamp,
			//FreeFromSwamp,
			//KnockOverTree,
			//BalanceOverTree,
			//PushRock,
			//SlipThroughRock,
			//Crawl,
			//JumpOverGap,
			//LegUp,
			//LegUpGrab,
			//UseKey,
			//PullDoor,
			//UseChalk,
			//UseWell,
			// 
			//UseItem,
			//SwitchItem,
		}

		/// <summary>
		/// Try to add Activity & check for triggering
		/// </summary>
		/// <param name="pPlayer">Player to test on</param>
		/// <param name="pOtherPlayer">Other Player for some checks</param>
		/// <param name="pActivity">Activity to test / disarm</param>
		/// <returns>true = Successfull disarmed (nothing happened). false = Something happened (e.g. a activity started). We need to break here.</returns>
		public bool DisarmPossibleActivity(Player pPlayer, Player pOtherPlayer, Activity pActivity)
		{
			return false;
		}

		public void UpdateButtonShowing()
		{
			//Fade ButtonShowing
			float TmpFadingDelta = ShowButtonFadingDuration * (EngineSettings.Time.ElapsedGameTime.Milliseconds / 1000f);
			if (ShowButtonHansel) //Fade ButtonShowingHansel in
				ButtonShowingHansel += TmpFadingDelta;
			else //Fade ButtonShowingHansel out
				ButtonShowingHansel -= TmpFadingDelta;
			if (ShowButtonGretel) //Fade ButtonShowingGretel in
				ButtonShowingGretel += TmpFadingDelta;
			else //Fade ButtonShowingGretel out
				ButtonShowingGretel -= TmpFadingDelta;
			//Clamp ButtonShowing to 0-1
			ButtonShowingHansel = MathHelper.Clamp(ButtonShowingHansel, 0f, 1f);
			ButtonShowingGretel = MathHelper.Clamp(ButtonShowingGretel, 0f, 1f);
		}

		#endregion
	}
}
