using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Controls;
using KryptonEngine.Entities;
using KryptonEngine.Manager;
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

		//ActionInfo
		public float ActionInfoVisibilityHansel;
		public float ActionInfoVisibilityGretel;
		protected const float ActionInfoFadingDuration = 1.0f;
		protected bool ShowActionInfoHansel;
		protected bool ShowActionInfoGretel;
		public string ActionInfoHansel;
		public string ActionInfoGretel;

		#endregion

		#region Constructor

		public ActivityHandler()
		{
			Initialize();
		}

		#endregion

		#region Methods

		protected void Initialize()
		{
			ActionInfoVisibilityHansel = 0;
			ActionInfoVisibilityGretel = 0;
			ShowActionInfoHansel = false;
			ShowActionInfoGretel = false;
			ActionInfoHansel = "Not Initialized";
			ActionInfoGretel = "Not Initialized";
		}

		public void Update(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			InteractiveObject TmpIObjEnteredByHansel = GetEnteredInteractiveObject(pHansel, pScene);
			InteractiveObject TmpIObjEnteredByGretel = GetEnteredInteractiveObject(pGretel, pScene);
			//Get Possible Activity from EnteredInteractiveObject
			Activity TmpPossibleActivityHansel;
			Activity TmpPossibleActivityGretel;
			if (TmpIObjEnteredByHansel == null || !TmpIObjEnteredByHansel.ActivityState.IsAvailable)
				TmpPossibleActivityHansel = Activity.None;
			else
				TmpPossibleActivityHansel = TmpIObjEnteredByHansel.ActivityState.GetPossibleActivity(TestInteractiveObjectContains(pHansel, TmpIObjEnteredByHansel));
			if (TmpIObjEnteredByGretel == null || !TmpIObjEnteredByGretel.ActivityState.IsAvailable)
				TmpPossibleActivityGretel = Activity.None;
			else
				TmpPossibleActivityGretel = TmpIObjEnteredByGretel.ActivityState.GetPossibleActivity(TestInteractiveObjectContains(pGretel, TmpIObjEnteredByGretel));
			//Checken ob der Player diese Activity ausführen kann
			bool TmpHandicappedHansel = !pHansel.CheckForAbility(TmpPossibleActivityHansel);
			bool TmpHandicappedGretel = !pGretel.CheckForAbility(TmpPossibleActivityGretel);
			//Wenn Player diese Activity nicht ausführen kann, oder ihm der Schlüssel fehlt oder er auf der falschen Seite des Baums steht, die PossibleActivity entsprechend anpassen
			if (TmpHandicappedHansel || (TmpPossibleActivityHansel == Activity.UseKey && !pHansel.Inventory.Contains(typeof(Key))) || (TmpPossibleActivityHansel == Activity.KnockOverTree && TmpIObjEnteredByHansel.ActivityState.NearestActionPosition(pHansel.Position) != TmpIObjEnteredByHansel.ActionPosition1))
				TmpPossibleActivityHansel = Activity.None;
			if (TmpHandicappedGretel || (TmpPossibleActivityGretel == Activity.UseKey && !pGretel.Inventory.Contains(typeof(Key))) || (TmpPossibleActivityGretel == Activity.KnockOverTree && TmpIObjEnteredByGretel.ActivityState.NearestActionPosition(pGretel.Position) != TmpIObjEnteredByGretel.ActionPosition1))
				TmpPossibleActivityGretel = Activity.None;

			//Update Activities
			TryToStartActivity(pHansel, pGretel, TmpIObjEnteredByHansel, TmpIObjEnteredByGretel, TmpHandicappedHansel, TmpHandicappedGretel);
			UpdateActivities(pHansel, pGretel);
			//Update ActionInfo
			UpdateActionInfo(pHansel.mCurrentActivity, pGretel.mCurrentActivity, TmpPossibleActivityHansel, TmpPossibleActivityGretel);
		}

		#region Update Input & Activities

		/// <summary>
		/// Activity anhand von Situation und Input ggf starten
		/// </summary>
		protected void TryToStartActivity(Hansel pHansel, Gretel pGretel, InteractiveObject pEnteredIObjHansel, InteractiveObject pEnteredIObjGretel, bool pHandicappedHansel, bool pHandicappedGretel)
		{
			//Hansel idled, ActivityState ist available, ActivityState.StateHansel idled, Hansel darf diese Activity ausführen
			if (pHansel.mCurrentActivity.GetType() == typeof(None) && pEnteredIObjHansel != null && pEnteredIObjHansel.ActivityState.IsAvailable && pEnteredIObjHansel.ActivityState.mStateHansel == ActivityState.State.Idle && !pHandicappedHansel)
			{ //Action pressed || (Cobweb && !PlayerTrapped) || (Swamp && HanselContaines)
				if (pHansel.Input.ActionIsPressed || ((pEnteredIObjHansel.Activity == Activity.CaughtInCobweb && !pEnteredIObjHansel.ActivityState.m2ndState) || (pEnteredIObjHansel.Activity == Activity.CaughtInSwamp && TestInteractiveObjectContains(pHansel, pEnteredIObjHansel))))
				{
					//Instant Swamp Trap
					if (pEnteredIObjHansel.Activity == Activity.CaughtInSwamp && TestInteractiveObjectContains(pHansel, pEnteredIObjHansel))
					{
						((CaughtInSwamp)pEnteredIObjHansel.ActivityState).HanselTrapped = true;
						if (((CaughtInSwamp)pEnteredIObjHansel.ActivityState).GretelTrapped)
							throw new Exception("Beide Player im Sumpf getrapped. N00B!");
					}
					//Bei Swamp && PlayerTrapped nur free ausführen wenn distance klein genug ist
					if ((pEnteredIObjHansel.Activity == Activity.CaughtInSwamp && pEnteredIObjHansel.ActivityState.m2ndState) && !((CaughtInSwamp)pEnteredIObjHansel.ActivityState).WithinMaxFreeDistance())
						return;
					pHansel.mCurrentActivity = pEnteredIObjHansel.ActivityState;
					pHansel.mCurrentActivity.mStateHansel = ActivityState.State.Preparing;
				}
			}

			//Gretel idled, ActivityState ist available, ActivityState.StateHansel idled, Gretel darf diese Activity ausführen
			if (pGretel.mCurrentActivity.GetType() == typeof(None) && pEnteredIObjGretel != null && pEnteredIObjGretel.ActivityState.IsAvailable && pEnteredIObjGretel.ActivityState.mStateGretel == ActivityState.State.Idle && !pHandicappedGretel)
			{ //Action pressed || (Cobweb && !PlayerTrapped) || (Swamp && GretelContaines)
				if (pGretel.Input.ActionIsPressed || ((pEnteredIObjGretel.Activity == Activity.CaughtInCobweb && !pEnteredIObjGretel.ActivityState.m2ndState) || (pEnteredIObjGretel.Activity == Activity.CaughtInSwamp && TestInteractiveObjectContains(pGretel, pEnteredIObjGretel))))
				{
					//Instant Swamp Trap
					if (pEnteredIObjGretel.Activity == Activity.CaughtInSwamp && TestInteractiveObjectContains(pGretel, pEnteredIObjGretel))
					{
						((CaughtInSwamp)pEnteredIObjGretel.ActivityState).GretelTrapped = true;
						if (((CaughtInSwamp)pEnteredIObjGretel.ActivityState).HanselTrapped)
							throw new Exception("Beide Player im Sumpf getrapped. N00B!");
					}
					//Bei Swamp && PlayerTrapped nur free ausführen wenn distance klein genug ist
					if ((pEnteredIObjGretel.Activity == Activity.CaughtInSwamp && pEnteredIObjGretel.ActivityState.m2ndState) && !((CaughtInSwamp)pEnteredIObjGretel.ActivityState).WithinMaxFreeDistance())
						return;
					pGretel.mCurrentActivity = pEnteredIObjGretel.ActivityState;
					pGretel.mCurrentActivity.mStateGretel = ActivityState.State.Preparing;
				}
			}
		}

		/// <summary>
		/// Activities der Player passend Aktualisieren
		/// </summary>
		protected void UpdateActivities(Hansel pHansel, Gretel pGretel)
		{
			pHansel.mCurrentActivity.GetUpdateMethodForPlayer(pHansel)(pHansel);
			pGretel.mCurrentActivity.GetUpdateMethodForPlayer(pGretel)(pGretel);
		}

		#endregion

		#region Get&Test InteractiveObject

		/// <summary>
		/// Getted das InteractiveObject dessen ActionRectangles der angegebene Player intersected.
		/// </summary>
		/// <param name="pPlayer">Spieler gegen den getestet werden soll</param>
		/// <param name="pScene">Scene mit zu testenden InteractiveObjects</param>
		/// <param name="pContains">False = Player muss nur intersecten. True = Player muss komplett im Action Rectangle stehen.</param>
		protected InteractiveObject GetEnteredInteractiveObject(Player pPlayer, SceneData pScene)
		{
			foreach (InteractiveObject iObj in pScene.InteractiveObjects)
			{
				foreach (Rectangle rect in iObj.ActionRectList)
				{
					if (rect.Intersects(pPlayer.CollisionBox)) //Intersects
					{
						return iObj;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Testet ob der Player von einem ActionRectangle des InteractiveObjects contained wird.
		/// </summary>
		protected bool TestInteractiveObjectContains(Player pPlayer, InteractiveObject pIObj)
		{
			if (pIObj == null)
				return false;
			foreach (Rectangle rect in pIObj.ActionRectList)
			{
				if (rect.Contains(pPlayer.CollisionBox)) //Contains
				{
					return true;
				}
			}
			return false;
		}

		#endregion

		#region Update ActionInfo
		
		/// <summary>
		/// Die ActionInfo wird geupdated incl Sichtbarkeit.
		/// </summary>
		protected void UpdateActionInfo(ActivityState pActivityHansel, ActivityState pActivityGretel, Activity pPossibleActivityHansel, Activity pPossibleActivityGretel)
		{
			if (pPossibleActivityHansel == Activity.None || pActivityHansel.GetType() != typeof(None))
			{
				ShowActionInfoHansel = false;
			}
			else
			{
				ShowActionInfoHansel = true;
				ActionInfoHansel = ActivityState.ActivityInfo[pPossibleActivityHansel];
			}
			if (pPossibleActivityGretel == Activity.None || pActivityGretel.GetType() != typeof(None))
			{
				ShowActionInfoGretel = false;
			}
			else
			{
				ShowActionInfoGretel = true;
				ActionInfoGretel = ActivityState.ActivityInfo[pPossibleActivityGretel];
			}
			UpdateActionInfoFading();
		}

		/// <summary>
		/// Der Fadingstatus der ActionInfo wird geupdated.
		/// </summary>
		protected void UpdateActionInfoFading()
		{
			//Fade ButtonShowing
			float TmpFadingDelta = ActionInfoFadingDuration * (EngineSettings.Time.ElapsedGameTime.Milliseconds / 1000f);
			if (ShowActionInfoHansel) //Fade ButtonShowingHansel in
				ActionInfoVisibilityHansel += TmpFadingDelta;
			else //Fade ButtonShowingHansel out
				ActionInfoVisibilityHansel -= TmpFadingDelta;
			if (ShowActionInfoGretel) //Fade ButtonShowingGretel in
				ActionInfoVisibilityGretel += TmpFadingDelta;
			else //Fade ButtonShowingGretel out
				ActionInfoVisibilityGretel -= TmpFadingDelta;
			//Clamp ButtonShowing to 0-1
			ActionInfoVisibilityHansel = MathHelper.Clamp(ActionInfoVisibilityHansel, 0f, 1f);
			ActionInfoVisibilityGretel = MathHelper.Clamp(ActionInfoVisibilityGretel, 0f, 1f);
		}

		#endregion

		#region Setup InteractiveObjects.ActivityState

		//-----Workaround für linerare Abhängigkeit HG_Game -> HG_Data -> KryptonEngine-----
		public void SetupInteractiveObjectsFromDeserialization(Savegame pSavegame, Hansel pHansel, Gretel pGretel)
		{
			for (int i = 0; i < pSavegame.Scenes.Length; i++)
			{
				foreach (InteractiveObject iObj in pSavegame.Scenes[i].InteractiveObjects)
				{
					switch (iObj.Activity)
					{
						case Activity.CaughtInCobweb:
							iObj.ActivityState = new CaughtInCobweb(pHansel, pGretel, iObj);
							break;
						case Activity.CaughtInSwamp:
							iObj.ActivityState = new CaughtInSwamp(pHansel, pGretel, iObj);
							break;
						case Activity.KnockOverTree:
							iObj.ActivityState = new KnockOverTree(pHansel, pGretel, iObj);
							break;
						case Activity.BalanceOverTree:
							iObj.ActivityState = new KnockOverTree(pHansel, pGretel, iObj);
							//ToDo Dummy 2ndState Animation applyen.
							iObj.ActivityState.m2ndState = true;
							break;
						case Activity.PushRock:
							iObj.ActivityState = new PushRock(pHansel, pGretel, iObj);
							break;
						case Activity.SlipThroughRock:
							iObj.ActivityState = new SlipThroughRock(pHansel, pGretel, iObj);
							break;
						case Activity.Crawl:
							iObj.ActivityState = new Crawl(pHansel, pGretel, iObj);
							break;
						case Activity.JumpOverGap:
							iObj.ActivityState = new JumpOverGap(pHansel, pGretel, iObj);
							break;
						case Activity.LegUp:
							iObj.ActivityState = new LegUp(pHansel, pGretel, iObj);
							break;
						case Activity.LegUpGrab:
							iObj.ActivityState = new LegUpGrab(pHansel, pGretel, iObj);
							break;
						case Activity.UseKey:
							iObj.ActivityState = new UseKey(pHansel, pGretel, iObj);
							break;
						case Activity.PushDoor:
							iObj.ActivityState = new UseKey(pHansel, pGretel, iObj);
							//ToDo Dummy 2ndState Animation applyen.
							iObj.ActivityState.m2ndState = true;
							break;
						case Activity.PullDoor:
							iObj.ActivityState = new PullDoor(pHansel, pGretel, iObj);
							break;
						case Activity.UseChalk:
							iObj.ActivityState = new UseChalk(pHansel, pGretel, iObj);
							break;
						case Activity.UseWell:
							iObj.ActivityState = new UseWell(pHansel, pGretel, iObj);
							break;
						default:
							throw new Exception("Im InteractiveObject " + iObj.ObjectId.ToString() + " in Scene " + i.ToString() + " ist eine ungültige Action angegeben!");
					}
				}
			}
		}

		#endregion

		#endregion


		#region Old Code

		/*
		 * Old Update
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
			*/
		/*
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
				ShowActionInfoansel = false; //Keine Info anzeigen
			}
			else
			{
				ActionInfoHansel = ActivityHansel.ToString(); //Info aktualisieren
			}
			
			if (pGretel.mCurrentActivity != Activity.None || ActivityGretel == Activity.None)
			{
				ShowActionInfoGretel = false; //Keine Info anzeigen
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

		

		#endregion
		*/
		#endregion

	}
}
