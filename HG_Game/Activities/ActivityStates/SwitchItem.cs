using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	class SwitchItem : ActivityState
	{
		protected const float MaxSwapDistance = 200f;
		public int InventoryFocusHansel;
		public int InventoryFocusGretel;

		public SwitchItem(Hansel pHansel, Gretel pGretel)
			: base(pHansel, pGretel)
		{
			InventoryFocusHansel = 1;
			InventoryFocusGretel = 1;
		}

		#region Override Methods

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0:
					if (pPlayer.GetType() == typeof(Hansel))
						InventoryFocusHansel = pPlayer.Inventory.ItemFocus;
					else
						InventoryFocusGretel = pPlayer.Inventory.ItemFocus;
					++pPlayer.mCurrentState;
					break;
				case 1:
					//Map IFocus to Player
					int InventoryFocusPlayer = InventoryFocusGretel;
					if (pPlayer.GetType() == typeof(Hansel))
						InventoryFocusPlayer = InventoryFocusHansel;

					//Navigate Inventory
					if (pPlayer.Input.ItemLeftJustPressed)
						--InventoryFocusPlayer;
					if (pPlayer.Input.ItemRightJustPressed)
						++InventoryFocusPlayer;
					InventoryFocusPlayer = (int)MathHelper.Clamp(InventoryFocusPlayer, 0, 2);

					//UseItem
					if (pPlayer.Input.UseItemJustPressed && pPlayer.Inventory.ItemSlots[InventoryFocusPlayer].Item != null)
					{
						pPlayer.Inventory.ItemFocus = InventoryFocusPlayer;
						++pPlayer.mCurrentState;
						return;
					}
					if (pPlayer.Input.SwitchItemJustPressed || pPlayer.Input.BackJustPressed)
					{
						++pPlayer.mCurrentState;
						return;
					}

					//SwapItem
					if (pPlayer.Input.ActionJustPressed && ((pPlayer.SkeletonPosition - pOtherPlayer.SkeletonPosition).Length() <= MaxSwapDistance))
						SwapItem(pPlayer, pOtherPlayer);
					break;
				case 2:
					Sequences.SetPlayerToIdle(pPlayer); //In nächsten Framedurchlauf gesetzt damit der ItemHandler das Inventory beim Schließen durch Y nicht instant wieder öffnet (JustPressed)
					break;
			}
		}

		#endregion

		#region Methods

		protected void SwapItem(Player pPlayer, Player pOtherPlayer)
		{
			//Map IFocus to Player
			int InventoryFocusPlayer;
			if (pPlayer.GetType() == typeof(Hansel))
				InventoryFocusPlayer = InventoryFocusHansel;
			else
				InventoryFocusPlayer = InventoryFocusGretel;
			//Swap Item
			if (pPlayer.Inventory.ItemSlots[InventoryFocusPlayer].Item == null)
				return;
			if (pOtherPlayer.Inventory.TryToStore(pPlayer.Inventory.ItemSlots[InventoryFocusPlayer].Item))
				pPlayer.Inventory.ItemSlots[InventoryFocusPlayer].Item = null;
		}

		#endregion

	}
}
