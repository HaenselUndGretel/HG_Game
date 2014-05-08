using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel
{
	public class Savegame
	{
		#region Properties

		public List<Artefact> Artefacts;
		public List<Toy> Toys;
		public List<DiaryEntry> Diary;

		public Inventory InventoryHansel;
		public Inventory InventoryGretel;
		public int Chalk;

		public Waypoint WaypointHansel;
		public Waypoint WaypointGretel;

		public SceneData[] Scenes;

		#endregion

		#region Constructor

		#endregion

		#region Override Methods

		#endregion

		#region Methods

		public void Load()
		{
			//Savegame aus File laden
		}

		public void Save()
		{
			//Savegame in File schreiben
		}

		public void Reset()
		{
			//SaveGame von SceneData Files neu mit default Werten erstellen.
		}

		#endregion
	}
}
