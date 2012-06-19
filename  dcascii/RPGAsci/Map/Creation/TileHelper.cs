using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class TileHelper
	{
		static private Random random = new Random();
		static public List<Tile> GetAdjacentTiles(int x, int y, Map map)
		{
			List<Tile> tiles = new List<Tile>();
			if (x - 1 >= 0)
			{
				tiles.Add(map.tiles[x - 1, y]);
			}
			if(x+1<map.width)
			{
				tiles.Add(map.tiles[x+1,y]);
			}
			if (y- 1 >= 0)
			{
				tiles.Add(map.tiles[x, y-1]);
			}
			if (y + 1 < map.height)
			{
				tiles.Add(map.tiles[x,y+1]);
			}
			return tiles;
		}
		static public Tile GetRandomTileOfType(Map map,string type)
		{
			while (true)
			{
				int randX = random.Next(1, map.width-1);
				int randY = random.Next(1, map.height-1);
				if (map.tiles[randX, randY].type == type)
				{
					return map.tiles[randX, randY];
				}
			}
			return null;
		}
	}
}
