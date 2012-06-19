using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Map
	{
		public Tile[,] tiles;
		public int width = 0;
		public int height = 0;
		int heroX = 5;
		int heroY = 5;
		public List<Room> rooms = new List<Room>();
		public List<MonsterUnit> monsters = new List<MonsterUnit>();
		public bool inBattle = false;
		public bool stairwayFound = false;
		Random random = new Random();
		MenuItem YesNoMenu;

		public Map(int width, int height)
		{
			this.width = width;
			this.height = height;
			tiles = new Tile[width, height];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					tiles[x, y] = new Tile(x,y);
					if (y == 0 || x == 0 || y == height - 1 || x == width - 1)
					{
						tiles[x, y].type = "OuterWall";
					}
					else
					{
						tiles[x, y].type = "UnWalkable";
					}
				}
			}
			YesNoMenu = new MenuItem("Menu",height+1);
			YesNoMenu.currentlySelected = true;
			YesNoMenu.AddChild(new MenuItem("Yes"));
			YesNoMenu.AddChild(new MenuItem("No"));
		}
		public void PlaceHero()
		{
			
			while (true)
			{
				Room randomRoom = rooms[random.Next(rooms.Count)];
				int randX = random.Next(randomRoom.xPos, randomRoom.xPos + randomRoom.width - 1);
				int randY = random.Next(randomRoom.yPos, randomRoom.yPos + randomRoom.height - 1);
				if (tiles[randX, randY].type == "Walkable")
				{
					heroX = randX;
					heroY = randY;
					break;
				}
			}
			DrawLight(3, heroX, heroY);
			tiles[heroX, heroY].light = 2;
			DrawTile(heroX, heroY);
		}
		public void Draw()
		{
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					DrawTile(x, y);
				}
			}
			//Console.WriteLine("\n" + rooms.Count);
		}
		public void DrawTile(int x, int y)
		{
			if (Console.CursorLeft != x || Console.CursorTop != y)
			{
				Console.SetCursorPosition(x, y);
			}
			Console.ResetColor();
			if (tiles[x, y].light > 0 && tiles[x,y].IsWalkable())
			{
				switch (tiles[x, y].light)
				{
					case 1: Console.BackgroundColor = ConsoleColor.Gray; break;
					case 2: Console.BackgroundColor = ConsoleColor.White; break;
				}
				Console.ForegroundColor = ConsoleColor.Black;
				if (tiles[x, y].light == 2 && tiles[x, y].monster)
				{
					Console.Write('M');
				}
				else if (heroX == x && heroY == y)
				{
					Console.Write('X');
				}
				else
				{
					switch (tiles[x, y].type)
					{
						case "Walkable": Console.Write("-"); break;
						case "Door": Console.Write("D"); break;
						case "Stairway": Console.Write("S"); break;
					}
					Console.ResetColor();
				}
			}
			else
			{
				Console.ResetColor();
				if (tiles[x, y].type == "OuterWall")
				{
					Console.BackgroundColor = ConsoleColor.DarkRed;
				}
				else if (tiles[x, y].type == "Wall" && tiles[x,y].light >0)
				{
					Console.BackgroundColor = ConsoleColor.DarkGreen;
				}
				else
				{
					Console.BackgroundColor = ConsoleColor.Black;
				}
				Console.Write(" ");
				if (x == width - 1 && y == height - 1)
				{
					Console.ResetColor();
				}
			}
			Console.ResetColor();
		}
		public void MoveMonsters()
		{

		}
		public void ReadInput()
		{
			MoveMonsters();
			ConsoleKeyInfo kb = Console.ReadKey(true);
			if (kb.Key == ConsoleKey.LeftArrow)
			{
				if (heroX - 1 >= 0 && tiles[heroX - 1, heroY].IsWalkable())
				{
					heroX -= 1;
					DrawShadow();
					DrawLight(3, heroX, heroY);
					DrawTile(heroX, heroY);

				}
			}
			else if (kb.Key == ConsoleKey.RightArrow)
			{
				if (heroX + 1 < width && tiles[heroX + 1, heroY].IsWalkable())
				{
					heroX += 1;
					DrawShadow();
					DrawLight(3, heroX, heroY);
					DrawTile(heroX, heroY);
				}
			}
			else if (kb.Key == ConsoleKey.UpArrow)
			{
				if (heroY - 1 >= 0 && tiles[heroX, heroY - 1].IsWalkable())
				{
					heroY -= 1;
					DrawShadow();
					DrawLight(3, heroX, heroY);
					DrawTile(heroX, heroY);
				}
			}
			else if (kb.Key == ConsoleKey.DownArrow)
			{
				if (heroY + 1 < height && tiles[heroX, heroY + 1].IsWalkable())
				{
					heroY += 1;
					DrawShadow();
					DrawLight(3, heroX, heroY);
					DrawTile(heroX, heroY);
				}
			}
			else if (kb.Key == ConsoleKey.X)
			{
				if (tiles[heroX, heroY].type == "Stairway")
				{
					Console.SetCursorPosition(0, height + 1);
					YesNoMenu.text = "Go Down?";
					YesNoMenu.Draw();
					while (true)
					{
						YesNoMenu.ReadInput();
						YesNoMenu.Draw();
						if (YesNoMenu.IsSelected("Yes"))
						{
							stairwayFound = true;
							break;
						}
						else if (YesNoMenu.IsSelected("No"))
						{
							break;
						}			
					}
				}
			}
			if (tiles[heroX, heroY].monster)
			{
				inBattle = true;
			}
			
		}
		public void DrawShadow()
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int oldLightValue = tiles[x, y].light;
					if (tiles[x, y].light > 0)
					{
						tiles[x, y].light = 1;
					}
					else
					{
						tiles[x, y].light = 0;
					}
					if (oldLightValue != tiles[x, y].light)
					{
						DrawTile(x, y);
					}
				}
			}
		}
		public void RemoveMonsterAfterBattle()
		{
			tiles[heroX, heroY].monster = false;
		}
		public void DrawLight(int range, int xPos, int yPos)
		{
			if (tiles[xPos, yPos].type == "OuterWall" || tiles[xPos,yPos].type == "UnWalkable")
			{
				return;
			}
			//Draw shadows around the light
			if (range == 0)
			{
				if (tiles[xPos+1, yPos].light == 0)
				{
					tiles[xPos + 1, yPos].light = 1;
					DrawTile(xPos + 1, yPos);
				}
				else if (tiles[xPos - 1, yPos].light == 0)
				{
					tiles[xPos - 1, yPos].light = 1;
					DrawTile(xPos - 1, yPos);
				}
				else if (tiles[xPos, yPos+1].light == 0)
				{
					tiles[xPos, yPos+1].light = 1;
					DrawTile(xPos, yPos+1);
				}
				else if (tiles[xPos, yPos-1].light == 0)
				{
					tiles[xPos, yPos-1].light = 1;
					DrawTile(xPos, yPos-1);
				}
				return;
			}
			if (xPos > 0 && tiles[xPos - 1, yPos].type != "OuterWall")
			{
				if (tiles[xPos - 1, yPos].light != 2)
				{
					tiles[xPos - 1, yPos].light = 2;
					DrawTile(xPos - 1, yPos);
				}

				if (tiles[xPos - 1, yPos].type != "Wall" && tiles[xPos - 1, yPos].type != "Door")
				{
					DrawLight(range - 1, xPos - 1, yPos);
				}
				else
				{
					if (yPos - 1 >= 0 && tiles[xPos - 1, yPos - 1].type == "Wall" && tiles[xPos - 1, yPos - 1].light != 2)
					{
						tiles[xPos - 1, yPos - 1].light = 2;
						DrawTile(xPos - 1, yPos - 1);
					}
					if (yPos + 1 < height && tiles[xPos - 1, yPos + 1].type == "Wall" && tiles[xPos - 1, yPos + 1].light != 2)
					{
						tiles[xPos - 1, yPos + 1].light = 2;
						DrawTile(xPos - 1, yPos + 1);
					}
				}
			}
			if (xPos + 1 < width && tiles[xPos + 1, yPos].type != "OuterWall")
			{
				if (tiles[xPos + 1, yPos].light != 2)
				{
					tiles[xPos + 1, yPos].light = 2;
					DrawTile(xPos + 1, yPos);
				}
				if (tiles[xPos + 1, yPos].type != "Wall" && tiles[xPos + 1, yPos].type != "Door")
				{
					DrawLight(range - 1, xPos + 1, yPos);
				}
				else
				{
					if (yPos - 1 >= 0 && tiles[xPos + 1, yPos - 1].type == "Wall" && tiles[xPos + 1, yPos - 1].light != 2)
					{
						tiles[xPos + 1, yPos - 1].light = 2;
						DrawTile(xPos + 1, yPos - 1);
					}
					if (yPos + 1 < height && tiles[xPos + 1, yPos + 1].type == "Wall" && tiles[xPos + 1, yPos + 1].light != 2)
					{
						tiles[xPos + 1, yPos + 1].light = 2;
						DrawTile(xPos + 1, yPos + 1);
					}
				}
			}
			if (yPos > 0 && tiles[xPos, yPos - 1].type != "OuterWall")
			{
				if (tiles[xPos, yPos - 1].light != 2)
				{
					tiles[xPos, yPos - 1].light = 2;
					DrawTile(xPos, yPos - 1);
				}

				if (tiles[xPos, yPos - 1].type != "Wall" && tiles[xPos, yPos - 1].type != "Door")
				{
					DrawLight(range - 1, xPos, yPos - 1);
				}
				else
				{
					if (xPos - 1 >= 0 && tiles[xPos - 1, yPos - 1].type == "Wall" && tiles[xPos - 1, yPos - 1].light != 2)
					{
						tiles[xPos - 1, yPos - 1].light = 2;
						DrawTile(xPos - 1, yPos - 1);
					}
					if (xPos + 1 < width && tiles[xPos + 1, yPos - 1].type == "Wall" && tiles[xPos + 1, yPos - 1].light != 2)
					{
						tiles[xPos + 1, yPos - 1].light = 2;
						DrawTile(xPos + 1, yPos - 1);
					}
				}
			}
			if (yPos + 1 < height && tiles[xPos, yPos + 1].type != "OuterWall")
			{
				if (tiles[xPos, yPos + 1].light != 2)
				{
					tiles[xPos, yPos + 1].light = 2;
					DrawTile(xPos, yPos + 1);
				}

				if (tiles[xPos, yPos + 1].type != "Wall" && tiles[xPos, yPos + 1].type != "Door")
				{
					DrawLight(range - 1, xPos, yPos + 1);
				}
				else
				{
					if (xPos - 1 >= 0 && tiles[xPos - 1, yPos + 1].type == "Wall" && tiles[xPos - 1, yPos + 1].light != 2)
					{
						tiles[xPos - 1, yPos + 1].light = 2;
						DrawTile(xPos - 1, yPos + 1);
					}
					if (xPos + 1 < width && tiles[xPos + 1, yPos + 1].type == "Wall" && tiles[xPos + 1, yPos + 1].light != 2)
					{
						tiles[xPos + 1, yPos + 1].light = 2;
						DrawTile(xPos + 1, yPos + 1);
					}
				}
			}
		}
		public void AddRoom(Room room)
		{
			for (int x = room.xPos; x < room.xPos + room.width; x++)
			{
				for (int y = room.yPos; y < room.yPos + room.height; y++)
				{
					if (x == room.xPos || y == room.yPos || x == room.xPos + room.width - 1 || y == room.yPos + room.height - 1)
					{
						tiles[x, y].type = "Wall";
					}
					else
					{
						tiles[x, y].type = "Walkable";
					}
					tiles[x, y].room = true;
				}
			}
			rooms.Add(room);
		}
	}
}
