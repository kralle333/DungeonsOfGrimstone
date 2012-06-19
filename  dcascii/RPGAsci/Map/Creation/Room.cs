using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Room
	{
		public int width;
		public int height;

		public int xPos;
		public int yPos;
		public List<Room> roomsConnectedTo = new List<Room>();
		public List<Door> doors = new List<Door>();

		public Room(int width, int height,int xPos,int yPos)
		{
			this.width = width;
			this.height = height;
			this.xPos = xPos;
			this.yPos = yPos;
		}
		public void AddDoor(Door door)
		{
			doors.Add(door);
		}
	}
}
