using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Controls;
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
		protected string ActionInfoHansel;
		protected string ActionInfoGretel;

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
			ActionInfoHansel = "Not Initialized";
			ActionInfoGretel = "Not Initialized";
		}

		public void Update(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			UpdatePossibleActivities(pScene, pHansel, pGretel);
			StartActivityFromInput(pHansel, pGretel);
			if (PrepareActivity(pHansel))
				if (PerformActivity(pHansel))
					FinishActivity(pHansel);
			if (PrepareActivity(pGretel))
				if (PerformActivity(pGretel))
					FinishActivity(pGretel);
			UpdateButton(pHansel, pGretel, pScene);
			UpdateButtonShowing();
		}

		#region Update PossibleActivities

		public void UpdatePossibleActivities(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			//Flush Lists
			PossibleActivitiesHansel.Clear();
			PossibleActivitiesGretel.Clear();

			//-----Add Activity when standing in ActionRect of InteractiveObject-----
			InteractiveObject TmpIObjToTest;
			//Hansel
			TmpIObjToTest = GetEnteredInteractiveObject(pHansel, pScene, false);
			if (pHansel.mCurrentActivity == Activity.None && DisarmPossibleActivity(pHansel, pGretel, TmpIObjToTest, PossibleActivitiesHansel))
				PossibleActivitiesHansel.Add((Activity)TmpIObjToTest.ActionId);
			//Gretel
			TmpIObjToTest = GetEnteredInteractiveObject(pGretel, pScene, false);
			if (pGretel.mCurrentActivity == Activity.None && DisarmPossibleActivity(pGretel, pHansel, TmpIObjToTest, PossibleActivitiesGretel))
				PossibleActivitiesGretel.Add((Activity)TmpIObjToTest.ActionId);

			//-----Test for other possible Activities-----
			//Hansel
			if (pHansel.mCurrentActivity == Activity.None)
			{
				if (pHansel.CheckForAbility(Activity.UseItem))
					PossibleActivitiesHansel.Add(Activity.UseItem);
				if (pHansel.CheckForAbility(Activity.SwitchItem))
					PossibleActivitiesHansel.Add(Activity.SwitchItem);
			}
			//Gretel
			if (pGretel.mCurrentActivity == Activity.None)
			{
				if (pGretel.CheckForAbility(Activity.UseItem))
					PossibleActivitiesGretel.Add(Activity.UseItem);
				if (pGretel.CheckForAbility(Activity.SwitchItem))
					PossibleActivitiesGretel.Add(Activity.SwitchItem);
			}			
		}

		/// <summary>
		/// Try to add Activity & check for triggering
		/// </summary>
		/// <param name="pPlayer">Player to test on</param>
		/// <param name="pOtherPlayer">Other Player for some checks</param>
		/// <param name="pActivity">Activity to test / disarm</param>
		/// <returns>true = Successfully disarmed (nothing happened). false = Something happened (e.g. a activity started). We need to break here.</returns>
		public bool DisarmPossibleActivity(Player pPlayer, Player pOtherPlayer, InteractiveObject pIObj, List<Activity> PossibleActivities)
		{
			switch ((Activity)pIObj.ActionId)
			{
				case Activity.CaughtInCobweb:
					if (!pIObj.IsActivated) //Im Netz steckt kein Spieler
					{
						//Im Netz
						pPlayer.mCurrentActivity = Activity.CaughtInCobweb;
						pIObj.IsActivated = true;
						return false;
					}
					if (pPlayer.mCurrentActivity != Activity.None && !pPlayer.CheckForAbility(Activity.FreeFromCobweb)) //Spieler kann aus dem Netz befreien?
						return true;
					foreach (InventorySlot slot in pPlayer.Inventory.ItemSlots) //Spielerinventar durchgehen
					{
						if (slot.Item.GetType() == typeof(Knife)) //Spieler hat ein Messer dabei
							PossibleActivities.Add(Activity.FreeFromCobweb); //Spieler kann anderen Spieler befreien
					}	
					return true;
				case Activity.CaughtInSwamp:
					if (!pIObj.IsActivated) //Im Sumpf steckt kein Spieler
					{
						//In den Sumpf fallen
						pPlayer.mCurrentActivity = Activity.FreeFromSwamp;
						pIObj.IsActivated = true;
						return false;
					}
					if (pPlayer.mCurrentActivity != Activity.None && !pPlayer.CheckForAbility(Activity.FreeFromSwamp)) //Spieler kann aus dem Sumpf befreien?
						return true;
					foreach (InventorySlot slot in pPlayer.Inventory.ItemSlots) //Spielerinventar durchgehen
					{
						if (slot.Item.GetType() == typeof(Branch)) //Spieler hat einen Ast dabei
							PossibleActivities.Add(Activity.FreeFromSwamp); //Spieler kann anderen Spieler befreien
					}
					return true;
				default:
					return true;
			}
		}

		#endregion

		#region Update Activity

		public void StartActivityFromInput(Hansel pHansel, Gretel pGretel)
		{
			if (pHansel.mCurrentActivity == Activity.None) //Hansel könnte eine Activity ausführen
			{
				if (pHansel.Input.ActionIsPressed) //Activity an InteractiveObject starten
				{
					if (PossibleActivitiesHansel.Contains(Activity.LegUp))
						pHansel.mCurrentActivity = Activity.LegUp;
				}
				else if (pHansel.Input.UseItemJustPressed)
				{

				}
				else if (pHansel.Input.SwitchItemJustPressed)
				{

				}
			}
			if (pGretel.mCurrentActivity == Activity.None) //Gretel könnte eine Activity ausführen
			{
				if (pGretel.Input.ActionIsPressed) //Activity an InteractiveObject starten
				{
					if (PossibleActivitiesHansel.Contains(Activity.LegUp))
						pHansel.mCurrentActivity = Activity.LegUp;
				}
				else if (pGretel.Input.UseItemJustPressed)
				{

				}
				else if (pGretel.Input.SwitchItemJustPressed)
				{

				}
			}
		}

		public bool PrepareActivity(Player pPlayer)
		{
			switch(pPlayer.mCurrentActivity)
			{

			}
			return false;
		}

		public bool PerformActivity(Player pPlayer)
		{

		}

		public FinishActivity(Player pPlayer)
		{

		}

		#endregion

		#region Update ButtonInfo

		/// <summary>
		/// Updated den ShowInfo&Button State.
		/// </summary>
		public void UpdateButton(Hansel pHansel, Gretel pGretel, SceneData pScene)
		{
			//Get Activity to show
			Activity ActivityHansel = GetPrimaryPossibleActivity(PossibleActivitiesHansel);
			Activity ActivityGretel = GetPrimaryPossibleActivity(PossibleActivitiesGretel);
			if (pHansel.mCurrentActivity != Activity.None || ActivityHansel == Activity.None)
			{
				ShowButtonHansel = false; //Keine Info anzeigen
			}
			else
			{
				ActionInfoHansel = ActivityHansel.ToString(); //Info aktualisieren
			}
			
			if (pGretel.mCurrentActivity != Activity.None || ActivityGretel == Activity.None)
			{
				ShowButtonGretel = false; //Keine Info anzeigen
			}
			else
			{
				ActionInfoGretel = ActivityGretel.ToString(); //Info aktualisieren
			}
		}

		/// <summary>
		/// Holt aus den PossibleActivities die gerade relevante raus.
		/// </summary>
		/// <returns>Returned bei UswItem & SwitchItem Activity.None, da bei diesen keine ButtonInfo angezeigt wird</returns>
		public Activity GetPrimaryPossibleActivity(List<Activity> pPossibleActivities)
		{
			else if (pPossibleActivities.Contains(Activity.LegUp))
			{
				return Activity.LegUp;
			}
			else if (pPossibleActivities.Contains(Activity.))
			return Activity.None;

			//-----Activities-----
			//None
			//CaughtInCobweb, FreeFromCobweb, CaughtInSwamp, FreeFromSwamp
			//KnockOverTree, BalanceOverTree
			//PushRock, SlipThroughRock, Crawl, JumpOverGap
			//LegUp, LegUpGrab
			//UseKey, PullDoor, UseChalk, UseWell
			//UseItem, SwitchItem
		}

		/// <summary>
		/// Der Fadingstatus der ButtonInfos wird geupdated.
		/// </summary>
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

		/// <summary>
		/// Getted das InteractiveObject in dessen Action Rectangle der angegebene Player steht.
		/// </summary>
		/// <param name="pPlayer">Spieler gegen den getestet werden soll</param>
		/// <param name="pScene">Scene mit zu testenden InteractiveObjects</param>
		/// <param name="pContains">False = Player muss nur intersecten. True = Player muss komplett im Action Rectangle stehen.</param>
		public InteractiveObject GetEnteredInteractiveObject(Player pPlayer, SceneData pScene, bool pContains)
		{
			foreach (InteractiveObject iObj in pScene.InteractiveObjects)
			{
				foreach (Rectangle rect in iObj.ActionRectList)
				{
					if (rect.Intersects(pPlayer.CollisionBox)) //Hansel kann mit Object interacten
					{
						if (!pContains || rect.Contains(pPlayer.CollisionBox))
							return iObj;
					}
				}
			}
			return null;
		}

		#endregion
	}
}
