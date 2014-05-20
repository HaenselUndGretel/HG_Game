using HanselAndGretel.Data;
using KryptonEngine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	public class SceneSwitch
	{
		public enum State
		{
			Idle,
			Switching,
			Entering
		}

		#region Properties

		public double FadingDuration;
		public double FadingProgress;
		protected Vector2 DestinationHansel;
		protected Vector2 DestinationGretel;
		protected int DestinationScene;
		public State CurrentState;

		#endregion

		#region Getter & Setter

		/// <summary>
		/// Fading Progress. Läuft von 1 bis 0.
		/// </summary>
		public float Fading { get { return 1f - (float)(FadingProgress / FadingDuration); } }

		#endregion

		#region Constructor

		public SceneSwitch()
		{
			Initialize();
		}

		#endregion

		#region Methods

		public void Initialize()
		{
			FadingDuration = 1000d;
			FadingProgress = 0d;
			CurrentState = State.Idle;
		}

		public void Update(Savegame pSavegame, ref SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			switch (CurrentState)
			{
				case State.Idle:
					TestForSwitch(pScene, pHansel, pGretel, pSavegame.Scenes);
					break;
				case State.Switching:
					Switch(pSavegame, ref pScene, pHansel, pGretel);
					break;
				case State.Entering:
					Enter(pScene, pHansel, pGretel);
					break;
				default:
					throw new Exception("SwitchScene.CurrentState not set!");
					break;
			}	
		}

		public void TestForSwitch(SceneData pScene, Hansel pHansel, Gretel pGretel, SceneData[] pSceneLookup) //Testet ob geswitched werden soll. Gibt die SceneId zurück zu der geswitched werden soll. Wenn nicht dann =-1.
		{
			foreach (Waypoint wp in pScene.Waypoints)
			{
				if (wp.CollisionBox.Intersects(pHansel.CollisionBox)) //Hänsel berührt den Waypoint
				{
					if (wp.TwoPlayerEnter) //Der Waypoint kann von 2 Spielern betreten werden
					{
						if (wp.CollisionBox.Intersects(pGretel.CollisionBox)) //Gretel berührt auch den Waypoint
						{
							StartSwitching(pHansel, pGretel, wp, wp, pSceneLookup);
							return;
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
									StartSwitching(pHansel, pGretel, wp, otherWp, pSceneLookup);
									return;
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Berechnet Situation nach Switching und Initialisiert das Switchen.
		/// </summary>
		/// <param name="pHansel">Hänsel</param>
		/// <param name="pGretel">Gretel</param>
		/// <param name="pWpHansel">Wegpunkt, der von Hänsel betreten ist.</param>
		/// <param name="pWpGretel">Wegpunkt, der von Gretel betreten ist.</param>
		/// <param name="pSceneLookup">Scenes-Array aus dem Savegame.</param>
		public void StartSwitching(Hansel pHansel, Gretel pGretel, Waypoint pWpHansel, Waypoint pWpGretel, SceneData[] pSceneLookup)
		{
			//Destination auf 0,0 setzen für ErrorTest
			DestinationHansel = Vector2.Zero;
			DestinationGretel = Vector2.Zero;
			foreach(Waypoint wp in pSceneLookup[pWpHansel.DestinationScene].Waypoints) //Wegpunkte in der Zielscene durchgehen
			{
				if (wp.ObjectId == pWpHansel.DestinationWaypoint) //Hansels Zielwegpunkt=
					DestinationHansel = wp.Position + (pHansel.Position - pWpHansel.Position);
				if (wp.ObjectId == pWpGretel.DestinationWaypoint) //Gretels Zielwegpunkt?
					DestinationGretel = wp.Position + (pGretel.Position - pWpGretel.Position);
			}
			if (DestinationHansel == Vector2.Zero || DestinationGretel == Vector2.Zero) //ErrorTest
				throw new Exception("Zielwegpunkt für Hansel oder Gretel nicht gefunden!");
			DestinationScene = pWpHansel.DestinationScene;
			//Switching initialisieren
			pHansel.mModel.SetAnimation();
			pGretel.mModel.SetAnimation();
			FadingProgress = 0d;
			CurrentState = State.Switching;
		}

		/// <summary>
		/// Aktualisiert den FadingProgress des Switchings.
		/// </summary>
		public void Switch(Savegame pSavegame, ref SceneData pScene, Hansel pHansel, Gretel pGretel) 
		{
			FadingProgress += EngineSettings.Time.ElapsedGameTime.Milliseconds;
			if (FadingProgress >= FadingDuration)
			{
				//Switch
				pHansel.Position = DestinationHansel;
				pGretel.Position = DestinationGretel;
				pScene = pSavegame.Scenes[DestinationScene];
				//Show on new Scene
				FadingProgress = 0;
				CurrentState = State.Entering;
			}
		}

		public void Enter(SceneData pScene, Hansel pHansel, Gretel pGretel)
		{
			bool TmpEnterFinished = true;
			foreach(Waypoint wp in pScene.Waypoints)
			{
				if (wp.CollisionBox.Intersects(pHansel.CollisionBox) || wp.CollisionBox.Intersects(pGretel.CollisionBox))
				{
					TmpEnterFinished = false;
					pHansel.MoveManually(wp.MovementOnEnter);
					pGretel.MoveManually(wp.MovementOnEnter);
				}
			}
			if (TmpEnterFinished)
				CurrentState = State.Idle;
		}

		#endregion
	}
}
