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
	{
		#region Properties

		HudFading InventoryFading;
		protected Vector2 InventoryOffset;

		#endregion

		#region Constructor

		public ItemHandler()
		{
			InventoryFading = new HudFading(0.3f, 0.3f);
			InventoryOffset = new Vector2(25, -100);
		}

		#endregion

		#region Methods

		public void Update(SceneData pScene, Hansel pHansel, Gretel pGretel, Savegame pSavegame)
		{
			//Das eigentliche InventoryUpdate wird automatisch über den ActivityHandler ausgeführt.
			if (pHansel.mCurrentActivity == ActivityHandler.None && pHansel.Input.SwitchItemJustPressed)
				pHansel.mCurrentActivity = new SwitchItem(pHansel, pGretel);
			if (pGretel.mCurrentActivity == ActivityHandler.None && pGretel.Input.SwitchItemJustPressed)
				pGretel.mCurrentActivity = new SwitchItem(pHansel, pGretel);

			UpdateVisibility(pScene, pGretel);
			CollectItems(pScene, pHansel, pGretel);
			CollectCollectables(pSavegame, pScene, pHansel, pGretel);
			InventoryFading.Update();
		}

		#region Update

		protected void UpdateVisibility(SceneData pScene, Gretel pGretel)
		{
			//Get Lantern
			Lantern lantern = (Lantern)pGretel.Inventory.GetItemByType(typeof(Lantern));
			if (lantern == null)
				return;

			//Update Items
			foreach (Item item in pScene.Items)
			{
				item.IsVisible = false;
				if (!item.IsHidden || item.CollisionBox.Intersects(pGretel.CollisionBox)) //Item befindet sich im Lichtkreis
				{
					item.IsVisible = true;
				}
			}

			//Update Collectables
			foreach (Collectable col in pScene.Collectables)
			{
				col.IsVisible = false;
				if (!col.IsHidden || col.CollisionBox.Intersects(pGretel.CollisionBox)) //Collectable befindet sich im Lichtkreis
				{
					col.IsVisible = true;
				}
			}
		}

		protected void CollectItems(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			foreach (Item item in pScene.Items)
			{
				if (item.IsVisible)
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
		}

		protected void CollectCollectables(Savegame pSavegame, SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			foreach (Collectable col in pScene.Collectables)
			{
				if (col.IsVisible && col.CollisionBox.Intersects(pHansel.CollisionBox) || col.CollisionBox.Intersects(pGretel.CollisionBox))
				{
					pSavegame.Collectables.Add(col);
					pScene.Collectables.Remove(col);
				}
			}
		}

		#endregion

		#region Draw Inventory

		public void DrawInventory(Hansel pHansel, Gretel pGretel, SpriteBatch pSpriteBatch)
		{
			int TmpFocusHansel = 3;
			int TmpFocusGretel = 3;
			if (pHansel.mCurrentActivity.GetType() == typeof(SwitchItem))
				TmpFocusHansel = ((SwitchItem)pHansel.mCurrentActivity).InventoryFocusHansel;
			if (pGretel.mCurrentActivity.GetType() == typeof(SwitchItem))
				TmpFocusGretel = ((SwitchItem)pGretel.mCurrentActivity).InventoryFocusGretel;
			pHansel.Inventory.Draw(pSpriteBatch, pHansel.Position + InventoryOffset, InventoryFading.VisibilityHansel, TmpFocusHansel);
			pGretel.Inventory.Draw(pSpriteBatch, pGretel.Position + InventoryOffset, InventoryFading.VisibilityGretel, TmpFocusGretel);
		}

		#endregion

		#endregion
	}
}
