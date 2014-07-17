using HanselAndGretel.Data;
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
	public class ActivityHandler
	{
		#region Properties

		public static None None = new None();
		
		protected HudFading ActionInfoFading;

		protected int ActionInfoHansel;
		protected int ActionInfoGretel;
		protected Texture2D ActionInfoButton;
		protected Texture2D[] ActionInfo;

		protected Vector2 ActionInfoOffset;
		protected Vector2 ActionInfoButtonOffset;

		#endregion

		#region Constructor

		public ActivityHandler()
		{
			ActionInfoFading = new HudFading();
			ActionInfoOffset = new Vector2(-100, -200);
			ActionInfoButtonOffset = new Vector2(-50, -300);
		}

		#endregion

		#region Methods

		public void LoadContent()
		{
			ActionInfoButton = TextureManager.Instance.GetElementByString("button_x");
			ActionInfo = new Texture2D[20]; //Anzahl an möglichen Activities
			string prefix = "ActivityInfo_";
			ActionInfo[1] = TextureManager.Instance.GetElementByString(prefix + "KnockOverTree");
			ActionInfo[2] = TextureManager.Instance.GetElementByString(prefix + "BalanceOverTree");
			ActionInfo[3] = TextureManager.Instance.GetElementByString(prefix + "PushRock");
			ActionInfo[4] = TextureManager.Instance.GetElementByString(prefix + "SlipThroughRock");
			ActionInfo[5] = TextureManager.Instance.GetElementByString(prefix + "JumpOverGap");
			ActionInfo[6] = TextureManager.Instance.GetElementByString(prefix + "LegUp");
			ActionInfo[7] = TextureManager.Instance.GetElementByString(prefix + "LegUpGrab");
			ActionInfo[8] = TextureManager.Instance.GetElementByString(prefix + "PushDoor");
			ActionInfo[9] = TextureManager.Instance.GetElementByString(prefix + "PullDoor");
			ActionInfo[10] = TextureManager.Instance.GetElementByString(prefix + "UseWell");

			/*
			FreeFromCobweb, "Befreien [Netz]"
			FreeFromSwamp, "Befreien [Sumpf]"
			KnockOverTree, "Umwerfen [Baum]"
			BalanceOverTree, "Balancieren [Baum]"
			PushRock, "Drücken [Fels]"
			SlipThroughRock, "Durch schlüpfen [Fels]"
			JumpOverGap, "Springen [Abgrund]"
			LegUp, "Räuberleiter"
			LegUpGrab, "Hoch heben"
			UseKey, "Schlüssel benutzen [Tür]"
			PushDoor, "Drücken [Tür]"
			PullDoor, "Ziehen [Tür]"
			UseChalk, "Markieren [Kreide]"
			UseWell, "Herablassen [Brunnen]"
			BalanceOverBrokenTree, "Balancieren [Brüchiger Baum]"
			*/

		}

		public void Update(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			//-----Ist Player verfügbar für eine Activity?-----
			bool TestHansel = false;
			bool TestGretel = false;
			if (pHansel.mCurrentActivity == None)
				TestHansel = true;
			if (pGretel.mCurrentActivity == None)
				TestGretel = true;

			//-----Wenn nicht getestet wird keine ActionInfo anzeigen-----
			ActionInfoFading.ShowHudHansel = false;
			ActionInfoFading.ShowHudGretel = false;
			//-----Activity ggf starten-----
			if (TestHansel || TestGretel)
			{
				//Betretene InteractiveObjects bestimmen
				InteractiveObject IObjIntersectsHansel = null;
				InteractiveObject IObjIntersectsGretel = null;
				Activity PossibleActivityHansel = Activity.None;
				Activity PossibleActivityGretel = Activity.None;
				foreach (InteractiveObject iObj in pScene.InteractiveObjects)
				{
					foreach (Rectangle rect in iObj.ActionRectList)
					{
						if (TestHansel && rect.Contains(new Point((int)pHansel.SkeletonPosition.X, (int)pHansel.SkeletonPosition.Y)))
						{
							IObjIntersectsHansel = iObj;
							PossibleActivityHansel = iObj.ActivityState.GetPossibleActivity(pHansel, pGretel);
						}
						if (TestGretel && rect.Contains(new Point((int)pGretel.SkeletonPosition.X, (int)pGretel.SkeletonPosition.Y)))
						{
							IObjIntersectsGretel = iObj;
							PossibleActivityGretel = iObj.ActivityState.GetPossibleActivity(pGretel, pHansel);
						}
					}
				}

				//Activity aufgrund von Spielereingabe starten?
				if (TestHansel &&
					IObjIntersectsHansel != null &&
					Conditions.ActionPressed(pHansel) &&
					PossibleActivityHansel != Activity.None)
				{
					pHansel.mCurrentActivity = IObjIntersectsHansel.ActivityState;
				}
				if (TestGretel &&
					IObjIntersectsGretel != null &&
					Conditions.ActionPressed(pGretel) &&
					PossibleActivityGretel != Activity.None)
				{
					pGretel.mCurrentActivity = IObjIntersectsGretel.ActivityState;
				}

				//-----Update ActionInfoState-----
				if (PossibleActivityHansel != Activity.None)
				{
					ActionInfoFading.ShowHudHansel = true;
					ActionInfoHansel = (int)PossibleActivityHansel;
				}
				if (PossibleActivityGretel != Activity.None)
				{
					ActionInfoFading.ShowHudGretel = true;
					ActionInfoGretel = (int)PossibleActivityGretel;
				}
			}

			//-----Update Activities-----
			pHansel.mCurrentActivity.Update(pHansel, pGretel);
			pGretel.mCurrentActivity.Update(pGretel, pHansel);

			ActionInfoFading.Update();
		}

		public void DrawActionInfo(SpriteBatch pSpriteBatch, Hansel pHansel, Gretel pGretel)
		{
			//ActionInfo
			if (ActionInfoHansel != 0)
				pSpriteBatch.Draw(ActionInfo[ActionInfoHansel], pHansel.SkeletonPosition + ActionInfoOffset, Color.White * ActionInfoFading.VisibilityHansel);
			if (ActionInfoGretel != 0)
				pSpriteBatch.Draw(ActionInfo[ActionInfoGretel], pGretel.SkeletonPosition + ActionInfoOffset, Color.White * ActionInfoFading.VisibilityGretel);
			//ButtonX
			pSpriteBatch.Draw(ActionInfoButton, pHansel.SkeletonPosition + ActionInfoButtonOffset, Color.White * ActionInfoFading.VisibilityHansel);
			pSpriteBatch.Draw(ActionInfoButton, pGretel.SkeletonPosition + ActionInfoButtonOffset, Color.White * ActionInfoFading.VisibilityGretel);
		}

		public void DrawActivityInstruction(SpriteBatch pSpriteBatch, Hansel pHansel, Gretel pGretel)
		{
			if (pHansel.mCurrentActivity != null)
			{
				pHansel.mCurrentActivity.Draw(pSpriteBatch, pHansel, pGretel);
			}
			if (pGretel.mCurrentActivity != null)
			{
				if (pHansel.mCurrentActivity != null && pGretel.mCurrentActivity == pHansel.mCurrentActivity)
					return; //Nicht doppelt zeichnen
				pGretel.mCurrentActivity.Draw(pSpriteBatch, pGretel, pHansel);
			}
		}

		#region Setup InteractiveObjects.ActivityState

		//-----Workaround für linerare Abhängigkeit HG_Game -> HG_Data -> KryptonEngine-----
		public void SetupInteractiveObjectsFromDeserialization(Savegame pSavegame, Hansel pHansel, Gretel pGretel)
		{
			for (int i = 0; i < pSavegame.Scenes.Length; i++)
			{
				foreach (InteractiveObject iObj in pSavegame.Scenes[i].InteractiveObjects)
				{
					switch (iObj.ActivityId)
					{
						case Activity.None:
							iObj.ActivityState = None;
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
						case Activity.JumpOverGap:
							iObj.ActivityState = new JumpOverGap(pHansel, pGretel, iObj);
							break;
						case Activity.LegUp:
							iObj.ActivityState = new LegUp(pHansel, pGretel, iObj);
							break;
						case Activity.LegUpGrab:
							iObj.ActivityState = new LegUp(pHansel, pGretel, iObj);
							iObj.ActivityState.m2ndState = true;
							break;
						case Activity.PushDoor:
							iObj.ActivityState = new PushDoor(pHansel, pGretel, iObj);
							//ToDo Dummy 2ndState Animation applyen.
							iObj.ActivityState.m2ndState = true;
							break;
						case Activity.PullDoor:
							iObj.ActivityState = new PushDoor(pHansel, pGretel, iObj);
							//(ToDo Dummy 2ndState Animation applyen.)
							iObj.ActivityState.m2ndState = true;
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
	}
}
