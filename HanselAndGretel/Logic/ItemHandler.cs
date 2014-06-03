using HanselAndGretel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	public class ItemHandler
	{
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

		#endregion
	}
}
