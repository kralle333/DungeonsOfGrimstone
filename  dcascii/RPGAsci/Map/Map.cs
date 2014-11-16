using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Map
	{
		struct Point
		{
			public float x;
			public float y;
			public Point(float x, float y)
			{
				this.x = x;
				this.y = y;
			}
		}
		private enum YesNoPrompt
		{
			None,
			GoDown,
			PickupItem

		}
		private YesNoPrompt currentPrompt = YesNoPrompt.None;
		public Tile[,] tiles;
		public int width = 0;
		public int height = 0;
		int heroX = -1;
		int heroY = -1;
		public List<Room> rooms = new List<Room>();
		public bool inBattle = false;
		public bool stairwayFound = false;
		public List<MonsterUnit> monsters = new List<MonsterUnit>();
		private Dictionary<ConsoleKey, Point> movementMap = new Dictionary<ConsoleKey, Point>();
		Random random = new Random();

		MenuItem YesNoMenu;
		MenuItem skillMenu;
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
				}
			}

			movementMap[ConsoleKey.LeftArrow] = new Point(-1, 0);
			movementMap[ConsoleKey.RightArrow] = new Point(1, 0);
			movementMap[ConsoleKey.UpArrow] = new Point(0, -1);
			movementMap[ConsoleKey.DownArrow] = new Point(0, 1);

			YesNoMenu = new MenuItem("Menu", 0, 0);
			YesNoMenu.AddChild(new MenuItem("Yes"));
			YesNoMenu.AddChild(new MenuItem("No"));
			skillMenu = new MenuItem("Select skill tree", 0, 0);
			foreach (Character c in Program.party.characters)
			{
				skillMenu.AddChild(new MenuItem(c.name));
			}
		}
		public void PlaceHero()
		{
			while (true)
			{
				Room randomRoom = rooms[random.Next(rooms.Count)];
				int randX = random.Next(randomRoom.xPos, randomRoom.xPos + randomRoom.width - 1);
				int randY = random.Next(randomRoom.yPos, randomRoom.yPos + randomRoom.height - 1);
				if (tiles[randX, randY].IsWalkable() && tiles[randX, randY].monster == null)
				{
					heroX = randX;
					heroY = randY;
					break;
				}
			}
			tiles[heroX, heroY].hasHero = true;
			DrawLight(3, heroX, heroY);
			tiles[heroX, heroY].light = 2;
			tiles[heroX, heroY].Draw();
		}
		public void Draw()
		{
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					tiles[x, y].Draw();
				}
			}
			//Console.WriteLine("\n" + rooms.Count);
		}

		public void ReadInput()
		{
			ConsoleKeyInfo kb = Console.ReadKey(true);
			//Movement
			if (movementMap.ContainsKey(kb.Key))
			{
				int newX = (int)movementMap[kb.Key].x + heroX;
				int newY = (int)movementMap[kb.Key].y + heroY;
				if (newX >= 0 && newX < width && newY >= 0 && newY < height && tiles[newX, newY].IsWalkable())
				{
					tiles[heroX, heroY].hasHero = false;
					tiles[newX, newY].hasHero = true;
					heroX = newX;
					heroY = newY;			
					DrawShadow();
					DrawLight(3, heroX, heroY);
					tiles[heroX, heroY].Draw();
					if (tiles[heroX, heroY].monster != null)
					{
						inBattle = true;
						return;
					}

					//Move monsters
					foreach (MonsterUnit monster in monsters)
					{
						monster.Move();
						monster.currentTile.Draw();
					}
					if (tiles[heroX, heroY].monster != null)
					{
						inBattle = true;
					}
				}

			}

			//Menus
			if (kb.Key == ConsoleKey.X)
			{
				if (tiles[heroX, heroY].type == Tile.TileType.Stairway)
				{
					YesNoMenu.text = "Go Down?";
					YesNoMenu.Draw();
					currentPrompt = YesNoPrompt.GoDown;
				}
				else if (tiles[heroX, heroY].item != null)
				{
					YesNoMenu.text = "Take Item?";
					YesNoMenu.Draw();
					currentPrompt = YesNoPrompt.PickupItem;
				}
				if (currentPrompt != YesNoPrompt.None)
				{
					HandleYesNoMenu();
				}
			}
			else if (kb.Key == ConsoleKey.I)
			{
				HandleInventoryMenu();
			}
			else if (kb.Key == ConsoleKey.S)
			{
				HandleSkillTreeMenu();
			}

		}
		private void HandleYesNoMenu()
		{
			while (true)
			{
				YesNoMenu.ReadInput(Console.ReadKey(true));
				bool yesSelected = YesNoMenu.IsSelected("Yes");
				bool noSelected = YesNoMenu.IsSelected("No");
				if (yesSelected)
				{
					switch (currentPrompt)
					{
						case YesNoPrompt.GoDown:
							stairwayFound = true;
							break;
						case YesNoPrompt.PickupItem:
							Program.party.GetItem(tiles[heroX, heroY].item);
							ConsoleHelper.ClearConsole();
							ConsoleHelper.GameWriteLine(Program.party.name + " picked up item " + tiles[heroX, heroY].item.name);
							Console.ReadKey(true);
							tiles[heroX, heroY].item = null;
							break;
					}
					ConsoleHelper.ClearConsole();
					break;
				}
				else if (noSelected)
				{
					ConsoleHelper.ClearConsole();
					break;
				}
			}
			currentPrompt = YesNoPrompt.None;
		}
		private void HandleInventoryMenu()
		{
			MenuItem items = new MenuItem("Inventory:", 0, 0);
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
			Item currentItem = ItemManager.GetItem(items.GetSelectedItemText(1));
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
					else if (input.Key == ConsoleKey.Z)
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
		private void HandleSkillTreeMenu()
		{
			skillMenu.Draw();
			while (true)
			{
				ConsoleKeyInfo skillInput = Console.ReadKey(true);
				skillMenu.ReadInput(skillInput);
				if (skillMenu.childSelected != null)
				{
					SkillTree currentTree = Program.party.characters[skillMenu.index].skillTree;
					ConsoleHelper.ClearConsole();
					currentTree.isHidden = false;
					currentTree.Draw();
					while (true)
					{
						ConsoleKeyInfo input = Console.ReadKey(true);
						if (currentTree.HandleInput(input))
						{
							ConsoleHelper.ClearConsole();
							currentTree.Draw();
						}
						else if (currentTree.isHidden)
						{
							ConsoleHelper.ClearConsole();
							currentTree.treeItem.Reset();
							skillMenu.Reset();
							skillMenu.Draw();
							break;
						}
					}
				}
				if (skillMenu.childSelected == null && skillInput.Key == ConsoleKey.Z)
				{
					ConsoleHelper.ClearConsole();
					break;
				}
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
						tiles[x, y].Draw();
					}
				}
			}
		}
		public void RemoveMonsterAfterBattle()
		{
			//Needs to be a function to avoid access of private variable
			tiles[heroX, heroY].monster = null;
		}
		public void DrawLight(int range, int xPos, int yPos)
		{
			if (!tiles[xPos, yPos].IsWalkable())
			{
				return;
			}
			List<Tile> adjTiles = TileHelper.GetAdjacentTiles(xPos, yPos, this);
			//Draw shadows around the light
			if (range <= 0)
			{
				
				foreach (Tile tile in adjTiles)
				{
					if (tile.light == 0)
					{
						tile.light = 1;
						tile.Draw();
					}
				}
				return;
			}
			foreach (Tile tile in adjTiles)
			{
				if (tile.light != 2)
				{
					if (tile.type != Tile.TileType.TallGrass)
					{
						tile.light = 2;
					}
					
					tile.Draw();
				}
				switch (tile.type)
				{
					case Tile.TileType.Dirt: DrawLight(range - 1, tile.x, tile.y); break;
					case Tile.TileType.TallGrass: DrawLight(Math.Min(range-1,0), tile.x, tile.y); break;
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
						tiles[x, y].type = Tile.TileType.Wall;
					}
					else
					{
						tiles[x, y].type = Tile.TileType.Dirt;
					}
					tiles[x, y].room = true;
				}
			}
			AddGrassPatch((room.width + room.height) / 4, tiles[random.Next(room.xPos + 1, room.xPos + room.width - 1), random.Next(room.yPos + 1, room.yPos + room.width - 1)]);
			rooms.Add(room);
		}
		public void AddGrassPatch(int range, Tile tile)
		{
			if (tile.type == Tile.TileType.Dirt)
			{
				//First tile
				tile.type = Tile.TileType.TallGrass;
			}
			if (range <= 0)
			{
				tile.type = Tile.TileType.TallGrass;
			}
			else
			{
				List<Tile> adjTiles = TileHelper.GetAdjacentTilesOfType(tile.x, tile.y, this, Tile.TileType.Dirt);
				foreach (Tile adjTile in adjTiles)
				{
					adjTile.type = Tile.TileType.TallGrass;
					AddGrassPatch(range - random.Next(1,3), adjTile);
				}
			}
		}
	}
}
