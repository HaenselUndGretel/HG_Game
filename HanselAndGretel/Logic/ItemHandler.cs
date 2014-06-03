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
	public class ItemHandler
	{
		#region Properties

		protected const float InventoryFadingDuration = 1f;
		protected float InventoryVisibilityHansel;
		protected float InventoryVisibilityGretel;

		#endregion

		#region Constructor

		public ItemHandler()
		{
			Initialize();
		}

		#endregion

		#region Methods

		protected void Initialize()
		{

		}

		public void Update(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			CollectItems(pScene, pHansel, pGretel);
			ShowInventory(pHansel, pGretel);
			UpdateInventory(pHansel, pGretel);
			UpdateInventoryFading(pHansel, pGretel);
		}

		protected void ShowInventory(Hansel pHansel, Gretel pGretel)
		{
			//Hansel
			if (pHansel.mCurrentActivity.GetType() == typeof(None) && pHansel.Input.SwitchItemJustPressed)
			{
				pHansel.mCurrentActivity = new SwitchItem(pHansel, pGretel);
				pHansel.mCurrentActivity.mStateHansel = ActivityState.State.Preparing;
			}
			else if (pHansel.mCurrentActivity.GetType() == typeof(SwitchItem) && (pHansel.Input.SwitchItemJustPressed || pHansel.Input.BackJustPressed))
			{
				pHansel.mCurrentActivity = new None();
			}

			//Gretel
			if (pGretel.mCurrentActivity.GetType() == typeof(None) && pGretel.Input.SwitchItemJustPressed)
			{
				pGretel.mCurrentActivity = new SwitchItem(pHansel, pGretel);
				pGretel.mCurrentActivity.mStateGretel = ActivityState.State.Preparing;
			}
			else if (pGretel.mCurrentActivity.GetType() == typeof(SwitchItem) && (pGretel.Input.SwitchItemJustPressed || pGretel.Input.BackJustPressed))
			{
				pGretel.mCurrentActivity = new None();
			}
		}

		protected void UpdateInventory(Hansel pHansel, Gretel pGretel)
		{
			if (pHansel.mCurrentActivity.GetType() == typeof(SwitchItem))
				pHansel.mCurrentActivity.GetUpdateMethodForPlayer(pHansel)(pHansel);
			if (pGretel.mCurrentActivity.GetType() == typeof(SwitchItem))
				pGretel.mCurrentActivity.GetUpdateMethodForPlayer(pGretel)(pGretel);
		}

		protected void CollectItems(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			foreach (Item item in pScene.Items)
			{
				if (item.CollisionBox.Intersects(pHansel.CollisionBox))
				{
					if (pHansel.Inventory.TryToStore(item))
					{
						pScene.Items.Remove(item);
						return;
					}
				}
				else if (item.CollisionBox.Intersects(pGretel.CollisionBox))
				{
					if (pGretel.Inventory.TryToStore(item))
					{
						pScene.Items.Remove(item);
						return;
					}
				}
			}
		}

		protected void UpdateInventoryFading(Hansel pHansel, Gretel pGretel)
		{
			//Fade ButtonShowing
			float TmpFadingDelta = InventoryFadingDuration * (float)(EngineSettings.Time.ElapsedGameTime.TotalMilliseconds / 1000d);
			if (pHansel.mCurrentActivity.GetType() == typeof(SwitchItem)) //Fade ButtonShowingHansel in
				InventoryVisibilityHansel += TmpFadingDelta;
			else //Fade ButtonShowingHansel out
				InventoryVisibilityHansel -= TmpFadingDelta;
			if (pGretel.mCurrentActivity.GetType() == typeof(SwitchItem)) //Fade ButtonShowingGretel in
				InventoryVisibilityGretel += TmpFadingDelta;
			else //Fade ButtonShowingGretel out
				InventoryVisibilityGretel -= TmpFadingDelta;
			//Clamp ButtonShowing to 0-1
			InventoryVisibilityHansel = MathHelper.Clamp(InventoryVisibilityHansel, 0f, 1f);
			InventoryVisibilityGretel = MathHelper.Clamp(InventoryVisibilityGretel, 0f, 1f);
		}

		#endregion
	}
}
