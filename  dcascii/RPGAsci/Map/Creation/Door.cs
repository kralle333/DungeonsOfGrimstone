using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Door:Tile
	{
		public bool connected = false;
		public Corridor doorCorridor = new Corridor();

		public Door(int xPos, int yPos):base(xPos,yPos)
		{
			type = "Door";
		}
	}
}
