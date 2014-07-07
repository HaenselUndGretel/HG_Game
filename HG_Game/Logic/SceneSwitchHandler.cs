using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Entities;
using KryptonEngine.Rendering;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HG_Game
{
	public class SceneSwitchHandler
	{
		public enum State
		{
			Idle,
			Switching,
			Entering
		};

		#region Properties

		public double FadingDuration;
		public double FadingProgress;
		protected Vector2 LeaveHansel;
		protected Vector2 LeaveGretel;
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

		public SceneSwitchHandler()
		{
			Initialize();
		}

		#endregion

		#region Methods

		public void Initialize()
		{
			FadingDuration = 1500d;
			FadingProgress = 0d;
			CurrentState = State.Idle;
		}

		public void Update(Savegame pSavegame, ref SceneData pScene, Hansel pHansel, Gretel pGretel, Camera pCamera, TwoDRenderer pRenderer)
		{
			switch (CurrentState)
			{
				case State.Idle:
					TestForSwitch(pScene, pHansel, pGretel, pSavegame.Scenes);
					break;
				case State.Switching:
					Switch(pSavegame, ref pScene, pHansel, pGretel, pCamera, pRenderer);
					break;
				case State.Entering:
					Enter(pScene, pHansel, pGretel);
					break;
				default:
					throw new Exception("SwitchScene.CurrentState not set!");
			}	
		}

		public void TestForSwitch(SceneData pScene, Hansel pHansel, Gretel pGretel, SceneData[] pSceneLookup) //Testet ob geswitched werden soll. Gibt die SceneId zurück zu der geswitched werden soll. Wenn nicht dann =-1.
		{
			foreach (Waypoint wp in pScene.Waypoints)
			{
				if (wp.CollisionBox.Contains(pHansel.CollisionBox)) //Hänsel steht in diesem Waypoint
				{
					if (wp.CollisionBox.Contains(pGretel.CollisionBox)) //Gretel steht auch in diesem Waypoint
					{
						if (!wp.OneWay)
							StartSwitching(pHansel, pGretel, wp, wp, pSceneLookup);
						return;
					}
					else
					{
						foreach(Waypoint otherWp in pScene.Waypoints)
						{
							if (otherWp != wp && otherWp.DestinationScene == wp.DestinationScene) //Ein weiterer Waypoint dieser Map führt auf die gleiche DestinationMap
							{
								if (otherWp.CollisionBox.Contains(pGretel.CollisionBox)) //Gretel steht in diesem Waypoint
								{
									if (!wp.OneWay && !otherWp.OneWay)
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
			LeaveHansel = -pWpHansel.MovementOnEnter;
			LeaveGretel = -pWpGretel.MovementOnEnter;
			//Destination auf 0,0 setzen für ErrorTest
			DestinationHansel = Vector2.Zero;
			DestinationGretel = Vector2.Zero;
			foreach(Waypoint wp in pSceneLookup[pWpHansel.DestinationScene].Waypoints) //Wegpunkte in der Zielscene durchgehen
			{
				if (wp.ObjectId == pWpHansel.DestinationWaypoint) //Hansels Zielwegpunkt=
					DestinationHansel = wp.Position + (pHansel.SkeletonPosition - pWpHansel.Position);
				if (wp.ObjectId == pWpGretel.DestinationWaypoint) //Gretels Zielwegpunkt?
					DestinationGretel = wp.Position + (pGretel.SkeletonPosition - pWpGretel.Position);
			}
			if (DestinationHansel == Vector2.Zero || DestinationGretel == Vector2.Zero) //ErrorTest
				throw new Exception("Zielwegpunkt für Hansel oder Gretel nicht gefunden!");
			DestinationScene = pWpHansel.DestinationScene;
			//Switching initialisieren
			pHansel.SetAnimation();
			pGretel.SetAnimation();
			FadingProgress = 0d;
			CurrentState = State.Switching;
		}

		/// <summary>
		/// Aktualisiert den FadingProgress des Switchings.
		/// </summary>
		public void Switch(Savegame pSavegame, ref SceneData pScene, Hansel pHansel, Gretel pGretel, Camera pCamera, TwoDRenderer pRenderer) 
		{
			FadingProgress += EngineSettings.Time.ElapsedGameTime.Milliseconds;
			pHansel.MoveManually(LeaveHansel, 1f, pScene);
			pGretel.MoveManually(LeaveGretel, 1f, pScene);
			if (FadingProgress >= FadingDuration)
			{
				//Switch
				pHansel.MoveInteractiveObject(DestinationHansel);
				pGretel.MoveInteractiveObject(DestinationGretel);
				pScene = pSavegame.Scenes[DestinationScene];
				pCamera.GameScreen = pScene.GamePlane;
				pScene.SetupRenderList(pHansel, pGretel);
				pRenderer.AmbientLight = pScene.SceneAmbientLight;
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
					pHansel.MoveManually(wp.MovementOnEnter, 1f, pScene, false);
					pGretel.MoveManually(wp.MovementOnEnter, 1f, pScene, false);
				}
			}
			if (TmpEnterFinished)
				CurrentState = State.Idle;
		}

		#endregion
	}
}
