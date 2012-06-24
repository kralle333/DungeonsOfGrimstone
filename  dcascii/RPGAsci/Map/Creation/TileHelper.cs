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
		}
		static public double GetDistance2Points(int x1, int y1, int x2, int y2)
		{
			return Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));

		}
		static public double GetDistance2Tiles(Tile tile1, Tile tile2)
		{
			return GetDistance2Points(tile1.x, tile1.y, tile2.x, tile2.y);
		}
	}
}
