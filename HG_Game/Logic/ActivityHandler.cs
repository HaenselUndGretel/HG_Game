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
			ActionInfoButton = TextureManager.Instance.GetElementByString("ButtonX");
			ActionInfo = new Texture2D[20]; //Anzahl an möglichen Activities
			string prefix = "ActivityInfo_";
			ActionInfo[2] = TextureManager.Instance.GetElementByString(prefix + "FreeFromCobweb");
			ActionInfo[4] = TextureManager.Instance.GetElementByString(prefix + "FreeFromSwamp");
			ActionInfo[5] = TextureManager.Instance.GetElementByString(prefix + "KnockOverTree");
			ActionInfo[6] = TextureManager.Instance.GetElementByString(prefix + "BalanceOverTree");
			ActionInfo[7] = TextureManager.Instance.GetElementByString(prefix + "PushRock");
			ActionInfo[8] = TextureManager.Instance.GetElementByString(prefix + "SlipThroughRock");
			ActionInfo[9] = TextureManager.Instance.GetElementByString(prefix + "JumpOverGap");
			ActionInfo[10] = TextureManager.Instance.GetElementByString(prefix + "LegUp");
			ActionInfo[11] = TextureManager.Instance.GetElementByString(prefix + "LegUpGrab");
			ActionInfo[12] = TextureManager.Instance.GetElementByString(prefix + "UseKey");
			ActionInfo[13] = TextureManager.Instance.GetElementByString(prefix + "PushDoor");
			ActionInfo[14] = TextureManager.Instance.GetElementByString(prefix + "PullDoor");
			ActionInfo[15] = TextureManager.Instance.GetElementByString(prefix + "UseChalk");
			ActionInfo[16] = TextureManager.Instance.GetElementByString(prefix + "UseWell");
			ActionInfo[19] = TextureManager.Instance.GetElementByString(prefix + "BalanceOverBrokenTree");

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

			//-----Activity ggf starten-----
			if (TestHansel || TestGretel)
			{
				//Betretene InteractiveObjects bestimmen
				InteractiveObject IObjIntersectsHansel = null;
				InteractiveObject IObjIntersectsGretel = null;
				Activity ActivityHansel = Activity.None;
				Activity ActivityGretel = Activity.None;
				foreach (InteractiveObject iObj in pScene.InteractiveObjects)
				{
					foreach (Rectangle rect in iObj.ActionRectList)
					{
						if (TestHansel && rect.Intersects(pHansel.CollisionBox))
						{
							IObjIntersectsHansel = iObj;
							ActivityHansel = iObj.ActivityState.GetPossibleActivity(pHansel, pGretel);
						}
						if (TestGretel && rect.Intersects(pGretel.CollisionBox))
						{
							IObjIntersectsGretel = iObj;
							ActivityGretel = iObj.ActivityState.GetPossibleActivity(pGretel, pHansel);
						}
					}
				}

				//Activity aufgrund von Spielereingabe starten?
				if (TestHansel &&
					IObjIntersectsHansel != null &&
					Conditions.ActionPressed(pHansel) &&
					ActivityHansel != Activity.None)
				{
					pHansel.mCurrentActivity = IObjIntersectsHansel.ActivityState;
				}
				if (TestGretel &&
					IObjIntersectsGretel != null &&
					Conditions.ActionPressed(pGretel) &&
					ActivityGretel != Activity.None)
				{
					pHansel.mCurrentActivity = IObjIntersectsGretel.ActivityState;
				}

				//-----Update ActionInfoState-----
				ActionInfoFading.ShowHudHansel = false;
				ActionInfoFading.ShowHudGretel = false;
				if (ActivityHansel != Activity.None)
				{
					ActionInfoFading.ShowHudHansel = true;
					ActionInfoHansel = (int)ActivityHansel;
				}
				if (ActivityGretel != Activity.None)
				{
					ActionInfoFading.ShowHudGretel = true;
					ActionInfoGretel = (int)ActivityGretel;
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
			pSpriteBatch.Draw(ActionInfo[ActionInfoHansel], pHansel.Position + ActionInfoOffset, Color.White * ActionInfoFading.VisibilityHansel);
			pSpriteBatch.Draw(ActionInfo[ActionInfoGretel], pGretel.Position + ActionInfoOffset, Color.White * ActionInfoFading.VisibilityGretel);
			//ButtonX
			pSpriteBatch.Draw(ActionInfoButton, pHansel.Position + ActionInfoButtonOffset, Color.White * ActionInfoFading.VisibilityHansel);
			pSpriteBatch.Draw(ActionInfoButton, pGretel.Position + ActionInfoButtonOffset, Color.White * ActionInfoFading.VisibilityGretel);
		}

		#endregion
	}
}
