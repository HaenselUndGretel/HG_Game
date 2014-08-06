using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.AI;
using KryptonEngine.Entities;
using KryptonEngine.FModAudio;
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
		public float Fading { get { return (float)(FadingProgress / FadingDuration); } }

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

		public void Update(Savegame pSavegame, ref SceneData pScene, Hansel pHansel, Gretel pGretel, Camera pCamera, TwoDRenderer pRenderer, ref GameScene.GameState pGameState)
		{
			switch (CurrentState)
			{
				case State.Idle:
					TestForSwitch(pScene, pHansel, pGretel, pSavegame.Scenes);
					break;
				case State.Switching:
					Switch(pSavegame, ref pScene, pHansel, pGretel, pCamera, pRenderer, ref pGameState);
					break;
				case State.Entering:
					Enter(pScene, pHansel, pGretel, pSavegame);
					break;
				default:
					throw new Exception("SwitchScene.CurrentState not set!");
			}	
		}

		public void TestForSwitch(SceneData pScene, Hansel pHansel, Gretel pGretel, SceneData[] pSceneLookup) //Testet ob geswitched werden soll. Gibt die SceneId zurück zu der geswitched werden soll. Wenn nicht dann =-1.
		{
			foreach (Waypoint wp in pScene.Waypoints)
			{
				if (wp.CollisionBox.Contains(new Point((int)pHansel.SkeletonPosition.X, (int)pHansel.SkeletonPosition.Y))) //Hänsel steht in diesem Waypoint
				{
					if (wp.CollisionBox.Contains(new Point((int)pGretel.SkeletonPosition.X, (int)pGretel.SkeletonPosition.Y))) //Gretel steht auch in diesem Waypoint
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
								if (otherWp.CollisionBox.Contains(new Point((int)pGretel.SkeletonPosition.X, (int)pGretel.SkeletonPosition.Y))) //Gretel steht in diesem Waypoint
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
			if (ActivityHandler.AmuletBlocksWaypoints || ActivityHandler.DoorBlocksWaypoint)
				return;
			LeaveHansel = -pWpHansel.MovementOnEnter;
			LeaveGretel = -pWpGretel.MovementOnEnter;
			//Destination auf 0,0 setzen für ErrorTest
			DestinationHansel = Vector2.Zero;
			DestinationGretel = Vector2.Zero;
			foreach(Waypoint wp in pSceneLookup[pWpHansel.DestinationScene].Waypoints) //Wegpunkte in der Zielscene durchgehen
			{
				Vector2 wpOrigin = new Vector2(wp.CollisionBox.Width / 2, wp.CollisionBox.Height / 2);
					bool direction = (wpOrigin.X > wpOrigin.Y) ? true : false;
				if (wp.ObjectId == pWpHansel.DestinationWaypoint) //Hansels Zielwegpunkt=
				{
					// Berechnung welche position
					DestinationHansel = (direction) ? wpOrigin + wp.Position + new Vector2(-50,0) : wpOrigin + wp.Position + new Vector2(0,-50);
				}
				if (wp.ObjectId == pWpGretel.DestinationWaypoint) //Gretels Zielwegpunkt?
				{
					// Berechnung welche position
					DestinationGretel = (direction) ? wpOrigin + wp.Position + new Vector2(50, 0) : wpOrigin + wp.Position + new Vector2(0, 50);
				}
					//DestinationGretel = wp.Position + (pGretel.SkeletonPosition - pWpGretel.Position);
			}
			if (DestinationHansel == Vector2.Zero || DestinationGretel == Vector2.Zero) //ErrorTest
				throw new Exception("Zielwegpunkt für Hansel oder Gretel nicht gefunden!");

			DestinationScene = pWpHansel.DestinationScene;
			//Switching initialisieren
			pHansel.SetAnimation();
			pGretel.SetAnimation();
			FadingProgress = 0d;
			CurrentState = State.Switching;
			FmodMediaPlayer.Instance.FadeBackgroundOut();
			FmodMediaPlayer.Instance.StopAllSongs();
			GameReferenzes.IsSceneSwitching = true;
		}

		/// <summary>
		/// Aktualisiert den FadingProgress des Switchings.
		/// </summary>
		public void Switch(Savegame pSavegame, ref SceneData pScene, Hansel pHansel, Gretel pGretel, Camera pCamera, TwoDRenderer pRenderer, ref GameScene.GameState pGameState) 
		{
			AIManager.Instance.ClearAgents();
			FadingProgress += EngineSettings.Time.ElapsedGameTime.Milliseconds;
			pHansel.MoveManually(LeaveHansel, 1f, pScene);
			pGretel.MoveManually(LeaveGretel, 1f, pScene);
			if (FadingProgress >= FadingDuration)
			{
				if (DestinationScene == Hardcoded.Scene_End)
				{
					pGameState = GameScene.GameState.EndScene;
					FmodMediaPlayer.Instance.SetBackgroundSong(GameReferenzes.GetBackgroundMusic());
					GameReferenzes.IsSceneSwitching = false;
					return;
				}
				//Switch
				pHansel.MoveInteractiveObject(DestinationHansel - pHansel.SkeletonPosition);
				pGretel.MoveInteractiveObject(DestinationGretel - pGretel.SkeletonPosition);
				pSavegame.SceneId = DestinationScene;
				pScene = pSavegame.Scenes[DestinationScene];
				SceneData.BackgroundTexture.LoadBackgroundTextures(Savegame.LevelNameFromId(DestinationScene));
				pCamera.GameScreen = pScene.GamePlane;
				pScene.SetupRenderList(pHansel, pGretel);
				pRenderer.AmbientLight = pScene.SceneAmbientLight;
				//Show on new Scene
				FadingProgress = 0;
				CurrentState = State.Entering;

				GameReferenzes.Level = pScene;
				AIManager.Instance.ChangeMap(pCamera.GameScreen, pScene.MoveArea);
				
				// Für alle weiteren Felder die nicht betreten werden können.
				AIManager.Instance.SetInterActiveObjects(pScene.InteractiveObjects);
				AIManager.Instance.SetAgents(pScene.Enemies);

				GameReferenzes.SceneID = DestinationScene;

				SoundHandler.Instance.ResetTime();
				FmodMediaPlayer.Instance.SetBackgroundSong(GameReferenzes.GetBackgroundMusic());
				GameReferenzes.IsSceneSwitching = false;
			}
		}

		public void Enter(SceneData pScene, Hansel pHansel, Gretel pGretel, Savegame pSavegame)
		{
			bool TmpEnterFinished = true;
			foreach(Waypoint wp in pScene.Waypoints)
			{
				if (wp.CollisionBox.Intersects(pHansel.CollisionBox) || wp.CollisionBox.Intersects(pGretel.CollisionBox))
				{
					TmpEnterFinished = false;
					pHansel.MoveManually(wp.MovementOnEnter, 1f, pScene, false, false, true);
					pGretel.MoveManually(wp.MovementOnEnter, 1f, pScene, false, false, true);
				}
			}
			if (TmpEnterFinished)
			{

				CurrentState = State.Idle;
				//Spiel speichern wenn Spieler in der Scene angekommen sind und ein Kreidefelsen in ihr steht.
				foreach (int i in Hardcoded.Scene_Waystone)
					if (GameReferenzes.SceneID == i)
						Savegame.Save(pSavegame, pHansel, pGretel);
			}
		}

		#endregion
	}
}
