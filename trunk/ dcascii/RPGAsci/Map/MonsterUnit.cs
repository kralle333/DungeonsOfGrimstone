using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class MonsterUnit
	{
		public int x;
		public int y;
		public Tile currentTile;
		private Map currentMap;
		private Random random;

		public MonsterUnit(Tile tile, Map map)
		{
			this.x = tile.x;
			this.y = tile.y;
			currentTile = tile;
			currentMap = map;
			random = new Random();
		}


		public void Move()
		{
			List<Tile> adjTiles = TileHelper.GetAdjacentWalkableTiles(currentTile.x, currentTile.y, currentMap);


			if (adjTiles.Any())
			{
				if (currentTile.light == 3)//Debug 3 is not possible
				{

					Tile bestTile = adjTiles[0];
					double distance = 50000;
					foreach (Tile tile in adjTiles)
					{
						double currentDistance = TileHelper.GetDistance2Tiles(tile, currentTile);
						if (currentDistance < distance)
						{
							bestTile = tile;
							distance = currentDistance;
						}
					}
					currentTile.monster = null;
					bestTile.monster = this;
					currentTile = bestTile;
				}
				else
				{
					Tile randomTile = adjTiles[random.Next(adjTiles.Count())];
					currentTile.monster = null;
					randomTile.monster = this;
					currentTile = randomTile;
				}
			}

		}
	}
}
