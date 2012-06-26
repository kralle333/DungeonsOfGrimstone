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
		int heroX = -1;
		int heroY = -1;
		public List<Room> rooms = new List<Room>();
		public bool inBattle = false;
		public bool stairwayFound = false;
		public List<MonsterUnit> monsters = new List<MonsterUnit>();
		Random random = new Random();
		MenuItem YesNoMenu;
		MenuItem targetMenu = new MenuItem("Target:", 0, 0);

		public Map(int width, int height)
		{
			this.width = width;
			this.height = height;
			tiles = new Tile[width, height];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					tiles[x, y] = new Tile(x, y);
					tiles[x, y].type = "UnWalkable";
				}
			}
			YesNoMenu = new MenuItem("Menu", 0, 0);
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
				if (tiles[randX, randY].type == "Walkable" && tiles[randX, randY].monster == null)
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
			if (Console.CursorLeft != x + 1 || Console.CursorTop != y + 1)
			{
				Console.SetCursorPosition(x + 1, y + 1);
			}
			//Console.ResetColor();
			if (tiles[x, y].light > 0 && tiles[x, y].IsWalkable())
			{
				switch (tiles[x, y].light)
				{
					case 1: Console.BackgroundColor = ConsoleColor.DarkGray; break;
					case 2: Console.BackgroundColor = ConsoleColor.Gray; break;
				}
				Console.ForegroundColor = ConsoleColor.Black;
				if (tiles[x, y].light == 2 && tiles[x, y].monster != null)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Write('Ö');
				}
				else if (heroX == x && heroY == y)
				{
					Console.ForegroundColor = ConsoleColor.Blue;
					Console.Write('♀');
				}
				else
				{
					if (tiles[x, y].item != null)
					{
						Console.BackgroundColor = ConsoleColor.Yellow;
						Console.Write("#");
					}
					else
					{

						switch (tiles[x, y].type)
						{
							case "Walkable": Console.Write("░"); break;
							case "Door": ConsoleHelper.Write(" ", ConsoleColor.Green); break;
							case "Stairway": Console.Write("↓"); break;
						}
					}
					Console.ResetColor();
				}
			}
			else
			{
				Console.ResetColor();
				if (tiles[x, y].type == "Wall" && tiles[x, y].light > 0)
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
			foreach (MonsterUnit unit in monsters)
			{
				if (tiles[unit.x, unit.y].light == 2)
				{
					List<Tile> adjTiles = TileHelper.GetAdjacentTiles(unit.x, unit.y, this);
					Tile bestTile = adjTiles[0];
					double distance = 50000;
					foreach (Tile tile in adjTiles)
					{
						double currentDistance = TileHelper.GetDistance2Tiles(tile, tiles[unit.x, unit.y]);
						if (currentDistance < distance)
						{
							bestTile = tile;
							distance = currentDistance;
						}
					}
					tiles[unit.x, unit.y].monster = null;
					bestTile.monster = unit;
					unit.x = bestTile.x;
					unit.y = bestTile.y;
				}
				else
				{
					List<Tile> adjTiles = TileHelper.GetAdjacentTiles(unit.x, unit.y, this);
					if (adjTiles.Count() > 0)
					{
						Tile randomTile = adjTiles[random.Next(adjTiles.Count())];
						tiles[unit.x, unit.y].monster = null;
						randomTile.monster = unit;
						unit.x = randomTile.x;
						unit.y = randomTile.y;
					}
				}
			}
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
					YesNoMenu.text = "Go Down?";
					YesNoMenu.Draw();
					while (true)
					{
						YesNoMenu.ReadInput(Console.ReadKey(true));
						if (YesNoMenu.IsSelected("Yes"))
						{
							stairwayFound = true;
							ConsoleHelper.ClearConsole();
							break;
						}
						else if (YesNoMenu.IsSelected("No"))
						{
							ConsoleHelper.ClearConsole();
							break;
						}
					}
				}
				else if (tiles[heroX, heroY].item != null)
				{
					YesNoMenu.text = "Take Item?";
					YesNoMenu.Draw();
					while (true)
					{
						YesNoMenu.ReadInput(Console.ReadKey(true));
						if (YesNoMenu.IsSelected("Yes"))
						{
							if (Program.party.items.ContainsKey(tiles[heroX, heroY].item))
							{
								Program.party.items[tiles[heroX, heroY].item]++;
							}
							else
							{
								Program.party.items.Add(tiles[heroX, heroY].item, 1);
							}

							ConsoleHelper.ClearConsole();
							ConsoleHelper.GameWriteLine(Program.party.name + " picked up item " + tiles[heroX, heroY].item.name);
							Console.ReadKey(true);
							ConsoleHelper.ClearConsole();
							tiles[heroX, heroY].item = null;
							break;
						}
						else if (YesNoMenu.IsSelected("No"))
						{
							ConsoleHelper.ClearConsole();
							break;
						}
					}
				}
			}
			else if (kb.Key == ConsoleKey.I)
			{
				MenuItem items = new MenuItem("Items:",0,0);
				foreach (KeyValuePair<Item, int> pair in Program.party.items)
				{
					MenuItem item = new MenuItem(pair.Value + "x " + pair.Key.name);
					if (pair.Value == 0)
					{
						item.locked = true;
					}
					items.AddChild(item);
				}
				items.Draw();
				
				while (true)
				{
					ConsoleKeyInfo input = Console.ReadKey(true);
					items.ReadInput(input);
					
					if (items.childSelected != null)
					{
						break;
					}
					else if (input.Key == ConsoleKey.Z)
					{
						ConsoleHelper.ClearConsole();
						return;
					}
				}
				Item currentItem = ItemManager.GetItem(items.GetSelectedItem(1));
				if (currentItem.target == "All")
				{
					foreach (Unit c in Program.party.characters)
					{
						ConsoleHelper.ClearConsole();
						currentItem.Use(c);
						Console.ReadKey(true);
					}

					Program.party.items.Remove(currentItem);
					items.children.Clear();
					targetMenu.children.Clear();
					targetMenu.Reset();
					items.children.Clear();
					Console.ReadKey(true);
				}
				else if (currentItem.target == "Single")
				{
					ConsoleHelper.ClearConsole();
					targetMenu.text = "Target " + currentItem.name + " on who?";
					foreach (Character c in Program.party.characters)
					{
						targetMenu.AddChild(new MenuItem(c.name));
					}
					targetMenu.Draw();
					targetMenu.currentlySelected = true;
					while (true)
					{
						ConsoleKeyInfo input = Console.ReadKey(true);
						targetMenu.ReadInput(input);
						MenuItem item = targetMenu.childSelected;
						if (item != null)
						{
							Character target = Program.party.characters.Find(x => x.name == item.text);
							ConsoleHelper.ClearConsole();
							currentItem.Use(target);
							Console.ReadKey(true);
							Program.party.items.Remove(currentItem);
							targetMenu.Reset();
							targetMenu.children.Clear();
							break;
						}
						else if(input.Key == ConsoleKey.Z)
						{
							ConsoleHelper.ClearConsole();
							items.children.Clear();
							targetMenu.children.Clear();
							return;
						}
					}
				}
				ConsoleHelper.ClearConsole();
			}

			if (tiles[heroX, heroY].monster != null)
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
			tiles[heroX, heroY].monster = null;
		}
		public void DrawLight(int range, int xPos, int yPos)
		{
			if (tiles[xPos, yPos].type == "UnWalkable")
			{
				return;
			}
			//Draw shadows around the light
			if (range == 0)
			{
				if (xPos + 1 < width && tiles[xPos + 1, yPos].light == 0)
				{
					tiles[xPos + 1, yPos].light = 1;
					DrawTile(xPos + 1, yPos);
				}
				else if (xPos > 0 && tiles[xPos - 1, yPos].light == 0)
				{
					tiles[xPos - 1, yPos].light = 1;
					DrawTile(xPos - 1, yPos);
				}
				else if (yPos + 1 < height && tiles[xPos, yPos + 1].light == 0)
				{
					tiles[xPos, yPos + 1].light = 1;
					DrawTile(xPos, yPos + 1);
				}
				else if (yPos > 0 && tiles[xPos, yPos - 1].light == 0)
				{
					tiles[xPos, yPos - 1].light = 1;
					DrawTile(xPos, yPos - 1);
				}
				return;
			}
			if (xPos > 0)
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
			if (xPos + 1 < width)
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
			if (yPos > 0)
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
			if (yPos + 1 < height)
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
