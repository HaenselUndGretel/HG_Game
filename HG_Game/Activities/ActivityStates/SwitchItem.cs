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
		public int InventoryFocus;

		public SwitchItem(Hansel pHansel, Gretel pGretel)
			: base(pHansel, pGretel)
		{
			InventoryFocus = 1;
		}

		#region Override Methods

		public override void Update(Player pPlayer, Player pOtherPlayer)
		{
			switch (pPlayer.mCurrentState)
			{
				case 0:
					InventoryFocus = pPlayer.Inventory.ItemFocus;
					++pPlayer.mCurrentState;
					break;
				case 1:
					//Navigate Inventory
					if (pPlayer.Input.ItemLeftJustPressed)
						--InventoryFocus;
					if (pPlayer.Input.ItemRightJustPressed)
						++InventoryFocus;
					InventoryFocus = (int)MathHelper.Clamp(InventoryFocus, 0, 2);

					//UseItem
					if (pPlayer.Input.UseItemJustPressed && pPlayer.Inventory.ItemSlots[InventoryFocus].Item != null)
					{
						pPlayer.Inventory.ItemFocus = InventoryFocus;
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
			if (pPlayer.Inventory.ItemSlots[InventoryFocus].Item == null)
				return;
			if (pOtherPlayer.Inventory.TryToStore(pPlayer.Inventory.ItemSlots[InventoryFocus].Item))
				pPlayer.Inventory.ItemSlots[InventoryFocus].Item = null;
		}

		#endregion

	}
}
