using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Tile
	{
		public char image = '-';
		public string type = "";
		public int light = 0;
		public MonsterUnit monster;
		public bool room;
		public Item item;
		public Corridor corridor = new Corridor();
		public int x;
		public int y;

		public Tile(int x,int y)
		{
			this.x = x;
			this.y = y;
		}
		public bool IsWalkable()
		{
			return type == "Walkable" || type == "Stairway" || type == "Door";
		}
	}
}
