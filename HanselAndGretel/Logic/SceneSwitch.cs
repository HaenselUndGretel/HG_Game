using HanselAndGretel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	public class SceneSwitch
	{

		public void Test(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			foreach (Waypoint wp in pScene.Waypoints)
			{
				if (wp.CollisionBox.Intersects(pHansel.CollisionBox)) //Hänsel berührt den Waypoint
				{
					if (wp.TwoPlayerEnter) //Der Waypoint kann von 2 Spielern betreten werden
					{
						if (wp.CollisionBox.Intersects(pGretel.CollisionBox)) //Gretel berührt auch den Waypoint
						{
							Switch();
						}
					}
					else if (wp.CollisionBox.Contains(pHansel.CollisionBox)) //Der Waypoint kann von einem Spieler betreten werden & Hänsel steht im Waypoint
					{
						foreach(Waypoint otherWp in pScene.Waypoints)
						{
							if (otherWp != wp && otherWp.DestinationScene == wp.DestinationScene) //Ein weiterer Waypoint dieser Map führt auf die gleiche DestinationMap
							{
								if (otherWp.CollisionBox.Contains(pGretel.CollisionBox)) //Gretel steht in diesem Waypoint
								{
									Switch();
								}
							}
						}
					}
				}
			}
			

		}

		public void Switch()
		{
 
		}

	}
}
