using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Corridor
	{
		public List<Tile> tiles = new List<Tile>();
		public List<Room> roomsConnected = new List<Room>();
		public Door startDoor;


		public void AddRoom(Room room)
		{
			if (!roomsConnected.Contains(room))
			{
				roomsConnected.Add(room);
			}
		}
		public void AddRooms(List<Room> rooms)
		{
			foreach (Room room in rooms)
			{
				AddRoom(room);
			}
		}
	}
}
