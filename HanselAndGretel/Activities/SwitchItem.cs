using HanselAndGretel.Data;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	class SwitchItem : ActivityState
	{
		#region Properties

		protected const float MaxSwapDistance = 200f;
		public int InventoryFocusHansel;
		public int InventoryFocusGretel;

		#endregion

		#region Constructor

		public SwitchItem(Hansel pHansel, Gretel pGretel)
			: base(pHansel, pGretel)
		{
			Initialize();
		}

		#endregion

		#region OverrideMethods

		public override void Initialize()
		{
			InventoryFocusHansel = 1;
			InventoryFocusGretel = 1;
		}

		public override void PrepareAction(Player pPlayer)
		{
			if (pPlayer.GetType() == typeof(Hansel))
			{
				InventoryFocusHansel = pPlayer.Inventory.ItemFocus;
				mStateHansel = State.Running;
			}
			else if (pPlayer.GetType() == typeof(Gretel))
			{
				InventoryFocusGretel = pPlayer.Inventory.ItemFocus;
				mStateGretel = State.Running;
			}
		}

		public override void StartAction(Player pPlayer)
		{
			//Nothing to do here!
		}

		public override void UpdateAction(Player pPlayer)
		{
			if (pPlayer.GetType() == typeof(Hansel))
			{
				//Navigate Inventory
				if (pPlayer.Input.ItemLeftJustPressed)
					--InventoryFocusHansel;
				if (pPlayer.Input.ItemRightJustPressed)
					++InventoryFocusHansel;
				InventoryFocusHansel = (int)MathHelper.Clamp(InventoryFocusHansel, 0, 2);

				//UseItem
				if (pPlayer.Input.UseItemJustPressed && pPlayer.Inventory.ItemSlots[InventoryFocusHansel].Item != null)
				{
					pPlayer.Inventory.ItemFocus = InventoryFocusHansel;
					pPlayer.mCurrentActivity = new None();
					mStateHansel = State.Idle;
				}
				//SwapItem
				if (pPlayer.Input.ActionJustPressed && ((rHansel.Position - rGretel.Position).Length() <= MaxSwapDistance))
					SwapItem(pPlayer);
			}
			else if (pPlayer.GetType() == typeof(Gretel))
			{
				//Navigate Inventory
				if (pPlayer.Input.ItemLeftJustPressed)
					--InventoryFocusGretel;
				if (pPlayer.Input.ItemRightJustPressed)
					++InventoryFocusGretel;
				InventoryFocusGretel = (int)MathHelper.Clamp(InventoryFocusGretel, 0, 2);

				//UseItem
				if (pPlayer.Input.UseItemJustPressed && pPlayer.Inventory.ItemSlots[InventoryFocusGretel].Item != null)
				{
					pPlayer.Inventory.ItemFocus = InventoryFocusGretel;
					pPlayer.mCurrentActivity = new None();
					mStateGretel = State.Idle;
				}
				//SwapItem
				if (pPlayer.Input.ActionJustPressed && ((rHansel.Position - rGretel.Position).Length() <= MaxSwapDistance))
					SwapItem(pPlayer);
			}
		}

		protected void SwapItem(Player pPlayer)
		{
			if (pPlayer.GetType() == typeof(Hansel))
			{
				if (rHansel.Inventory.ItemSlots[InventoryFocusHansel].Item == null)
					return;
				if (rGretel.Inventory.TryToStore(rHansel.Inventory.ItemSlots[InventoryFocusHansel].Item))
					rHansel.Inventory.ItemSlots[InventoryFocusHansel].Item = null;
			}
			else if (pPlayer.GetType() == typeof(Gretel))
			{
				if (rGretel.Inventory.ItemSlots[InventoryFocusGretel].Item == null)
					return;
				if (rHansel.Inventory.TryToStore(rGretel.Inventory.ItemSlots[InventoryFocusGretel].Item))
					rGretel.Inventory.ItemSlots[InventoryFocusGretel].Item = null;
			}
		}

		#endregion
	}
}
