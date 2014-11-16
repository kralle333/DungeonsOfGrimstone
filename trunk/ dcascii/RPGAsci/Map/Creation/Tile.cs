using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Tile
	{
		public enum TileType {Unwalkable,Wall, Dirt, Grass, TallGrass, Door, Stairway }
		public char image = '-';
		public TileType type;
		public int light = 0;
		public MonsterUnit monster;
		public bool hasHero = false;
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
			return type == TileType.Dirt || type == TileType.Grass || type == TileType.TallGrass || 
				type == TileType.Stairway || type == TileType.Door;
		}

		public void Draw()
		{
			if (Console.CursorLeft != x + 1 || Console.CursorTop != y + 1)
			{
				Console.SetCursorPosition(x + 1, y + 1);
			}
			//Console.ResetColor();
			if (light > 0)
			{
				Console.ResetColor();
				Console.BackgroundColor = light == 2 ? ConsoleColor.Gray : ConsoleColor.DarkGray;
				Console.ForegroundColor = ConsoleColor.Black;
				if (monster != null && light == 2)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Write('Ö');
				}
				else if (hasHero)
				{
					Console.ForegroundColor = ConsoleColor.Blue;
					Console.Write('♀');
				}
				else if (item != null)
				{
					Console.BackgroundColor = ConsoleColor.Yellow;
					Console.Write("#");
				}
				else
				{
					switch (type)
					{
						case TileType.Dirt: Console.Write(" "); break;
						case TileType.Grass: Console.Write("▒"); break;
						case TileType.TallGrass: Console.Write("▓"); break;
						case TileType.Wall: ConsoleHelper.Write(" ", ConsoleColor.DarkGreen); break;
						case TileType.Door: ConsoleHelper.Write(" ", ConsoleColor.Green); break;
						case TileType.Stairway: Console.Write("↓"); break;

					}
				}
			}
			Console.ResetColor();
		}
	}
}
