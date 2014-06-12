using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class ItemHandler
	{/*
		#region Properties

		protected const float InventoryFadingDuration = 0.3f;
		protected float InventoryVisibilityHansel;
		protected float InventoryVisibilityGretel;
		protected Vector2 InventoryOffset;

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
			InventoryOffset = new Vector2(25, -100);
		}

		public void Update(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			CollectItems(pScene, pHansel, pGretel);
			UpdateShowInventory(pHansel, pGretel);
			UpdateInventoryFading(pHansel, pGretel);
		}

		#region Update Inventory

		protected void UpdateShowInventory(Hansel pHansel, Gretel pGretel)
		{ //Das eigentliche InventoryUpdate wird automatisch über den ActivityHandler ausgeführt.
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
			float TmpFadingDelta = (float)EngineSettings.Time.ElapsedGameTime.TotalSeconds / InventoryFadingDuration;
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

		#region Draw Inventory

		public void DrawInventory(Hansel pHansel, Gretel pGretel, SpriteBatch pSpriteBatch, SkeletonRenderer pSkeletonRenderer)
		{
			int TmpFocusHansel = 3;
			int TmpFocusGretel = 3;
			if (pHansel.mCurrentActivity.GetType() == typeof(SwitchItem))
				TmpFocusHansel = ((SwitchItem)pHansel.mCurrentActivity).InventoryFocusHansel;
			if (pGretel.mCurrentActivity.GetType() == typeof(SwitchItem))
				TmpFocusGretel = ((SwitchItem)pGretel.mCurrentActivity).InventoryFocusGretel;
			pHansel.Inventory.Draw(pSpriteBatch, pHansel.Position + InventoryOffset, InventoryVisibilityHansel, TmpFocusHansel);
			pGretel.Inventory.Draw(pSpriteBatch, pGretel.Position + InventoryOffset, InventoryVisibilityGretel, TmpFocusGretel);
		}

		#endregion

		#endregion
	*/}
}
