using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RPGAsci
{
	class MapCreator
	{
		Map map;
		Random random = new Random();
		bool debug = false;
		int debugColorIndex = 0;
		ConsoleColor[] debugColors = new ConsoleColor[] { ConsoleColor.DarkBlue, 
															ConsoleColor.Green, 
															ConsoleColor.DarkMagenta, 
															ConsoleColor.DarkGray, 
															ConsoleColor.Cyan, 
															ConsoleColor.Blue, 
															ConsoleColor.Yellow,
															ConsoleColor.Gray,
															ConsoleColor.DarkCyan,
															ConsoleColor.DarkYellow,
															ConsoleColor.Magenta};
		int sleepyTime = 100;
		int level = 0;
		char connections = 'A';
		public MapCreator()
		{

		}
		public Map CreateMap(int width, int height, int level)
		{
			map = new Map(width, height);
			this.level = level;
			int numberOfRooms = random.Next(width / 2, (int)width);
			int tries = 0;

			while (numberOfRooms > 0)
			{
				if (PlaceRoom())
				{
					numberOfRooms--;
					if (debug)
					{
						ConsoleHelper.GameClearLine();
						ConsoleHelper.GameWrite("Placing rooms - Rooms left:" + numberOfRooms + "  ");
						Thread.Sleep(sleepyTime);
					}
				}
				tries++;
				if (tries > 10000)
				{
					numberOfRooms--;
					tries--;
					if (debug)
					{
						ConsoleHelper.GameClearLine();
						ConsoleHelper.GameWrite("Placing rooms - Rooms left:" + numberOfRooms + "  ");
						Thread.Sleep(sleepyTime);
					}
				}

			}
			PlaceDoors();
			if (debug)
			{
				ConsoleHelper.GameClearLine();
				ConsoleHelper.GameWriteLine("Setting Corridors");
			}
			MakeCorridors();
			ConnectRooms();
			while (true)
			{
				int randX = random.Next(1, map.width);
				int randY = random.Next(1, map.height);
				if (map.tiles[randX, randY].type == "Walkable")
				{
					map.tiles[randX, randY].type = "Stairway";
					break;
				}
			}
			PlaceMonsters();
			PlaceItems();
			if (debug)
			{
				ConsoleHelper.ClearConsole();
			}
			return map;
		}
		private bool PlaceRoom()
		{
			Room room = new Room(random.Next(3, map.width / (map.width / 10)), random.Next(3, map.width / (map.width / 10)), random.Next(map.width - 1), random.Next(map.height - 1));
			if (room.xPos + room.width >= map.width || room.yPos + room.height >= map.height || room.xPos <0 || room.yPos <0)
			{
				return false;
			}
			for (int x = room.xPos - 1; x < room.xPos + room.width + 1; x++)
			{
				for (int y = room.yPos - 1; y < room.yPos + room.height + 1; y++)
				{
					if (x>0 && y>0 && y<map.height && x<map.width && map.tiles[x, y].room)
					{
						//Console.WriteLine("Couldnt Place Room at:"+x+","+y);
						return false;
					}
				}
			}
			map.AddRoom(room);
			if (debug)
			{
				for (int x = room.xPos; x < room.width + room.xPos; x++)
				{
					for (int y = room.yPos; y < room.height + room.yPos; y++)
					{
						map.DrawTile(x, y);
						if (debug)
						{
							Thread.Sleep(5);
						}
					}
				}
			}
			return true;
		}
		private void PlaceDoors()
		{
			if (map.rooms.Count == 1)
			{
				return;
			}
			foreach (Room room in map.rooms)
			{
				int maxDoors = (int)Math.Sqrt(room.width * room.height);
				int numberOfDoors = random.Next(1, maxDoors);
				int tries = 0;
				while (numberOfDoors > 0)
				{
					if (PlaceDoor(room))
					{
						numberOfDoors--;
					}
					else
					{
						tries++;
					}
					if (tries == 10000)
					{
						numberOfDoors--;
					}
				}
			}
			if (!HasEnoughDoors())
			{
				Room randomRoom = map.rooms[random.Next(map.rooms.Count)];
				while (!PlaceDoor(randomRoom)) ;
			}
		}
		private bool PlaceDoor(Room room)
		{
			int randX;
			int randY;
			if (random.Next(1, 3) == 2)
			{
				randX = random.Next(room.xPos + 1, room.xPos + room.width - 1);
				List<int> availableY = new List<int>();
				if (room.yPos - 2 > 0)
				{
					availableY.Add(room.yPos);
				}
				if (room.yPos + room.height < map.height)
				{
					availableY.Add(room.yPos + room.height - 1);
				}
				randY = availableY[random.Next(availableY.Count())];
			}
			else
			{
				List<int> availableX = new List<int>();
				randY = random.Next(room.yPos + 1, room.yPos + room.height - 1);
				if (room.xPos - 2 > 0)
				{
					availableX.Add(room.xPos);
				}
				if (room.xPos + room.width< map.width)
				{
					availableX.Add(room.xPos + room.width - 1);
				}
				randX = availableX[random.Next(availableX.Count())];
			}
			if (map.tiles[randX, randY] is Door ||
				map.tiles[randX + 1, randY] is Door ||
				map.tiles[randX - 1, randY] is Door ||
				map.tiles[randX, randY + 1] is Door ||
				map.tiles[randX, randY - 1] is Door)
			{
				//Console.WriteLine("Already a door here at " + randX + "," + randY);
				return false;
			}
			Door door = new Door(randX, randY);
			room.AddDoor(door);
			map.tiles[randX, randY] = door;
			map.DrawTile(randX, randY);
			if (debug)
			{
				Thread.Sleep(1);
			}
			return true;
		}
		private bool HasEnoughDoors()
		{
			int doors = 0;
			foreach (Room room in map.rooms)
			{
				doors += room.doors.Count;
			}
			if (doors % 2 == 0)
			{
				return true;
			}
			return false;
		}
		private void MakeCorridors()
		{
			if (map.rooms.Count == 1)
			{
				return;
			}
			List<Door> doorsToSet = new List<Door>();
			foreach (Room room in map.rooms)
			{
				doorsToSet.Clear();
				doorsToSet.AddRange(room.doors);
				for (int i = 0; i < doorsToSet.Count; i++)
				{
					doorsToSet[i].doorCorridor.AddRoom(room);
					doorsToSet[i].doorCorridor.startDoor = doorsToSet[i];
					int currentPositionX = doorsToSet[i].x;
					int currentPositionY = doorsToSet[i].y;
					string direction = "Left";
					if (debug)
					{
						Console.BackgroundColor = debugColors[debugColorIndex];
					}
					//Door is in left Side
					if (currentPositionX == room.xPos)
					{
						currentPositionX--;
						direction = "Left";
					}
					//Door is in right Side
					if (currentPositionX == room.xPos + room.width - 1)
					{
						currentPositionX++;
						direction = "Right";
					}
					//Door is in bottom
					if (currentPositionY == room.yPos + room.height - 1)
					{
						currentPositionY++;
						direction = "Down";
					}
					//Door is in top
					if (currentPositionY == room.yPos)
					{
						currentPositionY--;
						direction = "Up";
					}
					if (map.tiles[currentPositionX, currentPositionY].type == "UnWalkable")
					{
						map.tiles[currentPositionX, currentPositionY].type = "Walkable";
						map.tiles[currentPositionX, currentPositionY].corridor = doorsToSet[i].doorCorridor;
						if (debug)
						{
							map.tiles[currentPositionX, currentPositionY].light = 3;
							map.DrawTile(currentPositionX, currentPositionY);
						}
					}

					List<Corridor> adjCorridors = TileHitOtherCorridors(currentPositionX, currentPositionY);
					if (adjCorridors.Count() > 0)
					{
						adjCorridors.Add(doorsToSet[i].doorCorridor);
						foreach (Corridor corridor in adjCorridors)
						{
							foreach (Corridor c in adjCorridors)
							{
								if (corridor != c)
								{
									corridor.AddRooms(c.roomsConnected);
								}
							}
						}
						continue;
					}

					List<string> triedDirections = new List<string>();
					double chance = 0.9;
					double chanceNeeded = 0.75;
					int sameDirection = 0;
					bool enoughChance = true;
					bool drewTile = false;

					while (true)
					{
						//Console.WriteLine(sameDirection);
						chance *= chance;

						if (direction == "Right")
						{
							if (IsOkayForCorridor(currentPositionX + 1, currentPositionY, doorsToSet[i].doorCorridor))
							{
								if (random.NextDouble() + chance > chanceNeeded)
								{
									currentPositionX++;
									drewTile = true;
								}
								else
								{
									enoughChance = false;
								}
							}
							else if (!triedDirections.Contains(direction))
							{
								triedDirections.Add(direction);
							}
						}
						else if (direction == "Left")
						{
							if (IsOkayForCorridor(currentPositionX - 1, currentPositionY, doorsToSet[i].doorCorridor))
							{
								if (random.NextDouble() + chance > chanceNeeded)
								{
									currentPositionX--;
									drewTile = true;
								}
								else
								{
									enoughChance = false;
								}
							}
							else if (!triedDirections.Contains(direction))
							{
								triedDirections.Add(direction);
							}
						}
						else if (direction == "Down")
						{
							if (IsOkayForCorridor(currentPositionX, currentPositionY + 1, doorsToSet[i].doorCorridor))
							{
								if (random.NextDouble() + chance > chanceNeeded)
								{
									currentPositionY++;
									drewTile = true;
								}
								else
								{
									enoughChance = false;
								}
							}
							else if (!triedDirections.Contains(direction))
							{
								triedDirections.Add(direction);
							}
						}
						else if (direction == "Up")
						{
							if (IsOkayForCorridor(currentPositionX, currentPositionY - 1, doorsToSet[i].doorCorridor))
							{
								if (random.NextDouble() + chance > chanceNeeded)
								{
									currentPositionY--;
									drewTile = true;
								}
								else
								{
									enoughChance = false;
								}
							}
							else if (!triedDirections.Contains(direction))
							{
								triedDirections.Add(direction);
							}
						}
						if (drewTile)
						{
							map.tiles[currentPositionX, currentPositionY].type = "Walkable";
							map.tiles[currentPositionX, currentPositionY].corridor = doorsToSet[i].doorCorridor;
							if (debug)
							{
								Console.BackgroundColor = debugColors[debugColorIndex];
								map.tiles[currentPositionX, currentPositionY].light = 3;
								map.DrawTile(currentPositionX, currentPositionY);
								Thread.Sleep(sleepyTime);
							}
							List<Tile> adjTiles = TileHelper.GetAdjacentTiles(currentPositionX, currentPositionY, map);
							foreach (Tile tile in adjTiles)
							{
								if (tile is Door && map.tiles[currentPositionX, currentPositionY].corridor.startDoor != tile)
								{
									map.tiles[currentPositionX, currentPositionY].corridor.roomsConnected.AddRange(tile.corridor.roomsConnected);
									tile.corridor.roomsConnected.AddRange(map.tiles[currentPositionX, currentPositionY].corridor.roomsConnected);
								}
							}
							doorsToSet[i].doorCorridor.tiles.Add(map.tiles[currentPositionX, currentPositionY]);
							sameDirection++;
							triedDirections.Clear();
							drewTile = false;
						}
						if (triedDirections.Count() > 0 || sameDirection >= 3 || !enoughChance)
						{
							if (triedDirections.Count() == 4)
							{
								triedDirections.Clear();
								sameDirection = 0;
								chance = 0.9;
								enoughChance = true;
								//Console.WriteLine("Tried all directions");
								if (debug)
								{
									debugColorIndex++;
									if (debugColorIndex >= debugColors.Count())
									{
										debugColorIndex = 0;
									}
								}
								break;
							}
							string directionSame = "";
							if (sameDirection >= 3)
							{
								directionSame = direction;
								sameDirection = 0;
							}
							while (triedDirections.Contains(direction) || direction == directionSame)
							{
								switch (random.Next(1, 5))
								{
									case 1: direction = "Right"; break;
									case 2: direction = "Left"; break;
									case 3: direction = "Up"; break;
									case 4: direction = "Down"; break;
								}
							}
							chance = 0.9;
							enoughChance = true;
						}
					}
					//Console.WriteLine("NOT HERE");
				}
			}
		}
		private void ConnectRooms()
		{
			if (debug)
			{
				Console.ResetColor();
				ConsoleHelper.GameClearLine();
				ConsoleHelper.GameWriteLine("Connecting unconnected rooms");
			}
			if (map.rooms.Count() == 1)
			{
				return;
			}
			foreach (Room room in map.rooms)
			{
				foreach (Door door in room.doors)
				{
					room.roomsConnectedTo.AddRange(door.doorCorridor.roomsConnected);
				}
				room.roomsConnectedTo = room.roomsConnectedTo.Distinct<Room>().ToList();
			}
			List<Room> allRooms = new List<Room>();
			for (int i = 0; i < map.rooms.Count(); i++)
			{
				foreach (Room room in map.rooms)
				{
					foreach (Room otherRoom in room.roomsConnectedTo)
					{
						if (room != otherRoom)
						{
							allRooms.AddRange(otherRoom.roomsConnectedTo);
						}
					}
					room.roomsConnectedTo.AddRange(allRooms);
					room.roomsConnectedTo = room.roomsConnectedTo.Distinct<Room>().ToList();
					allRooms.Clear();
				}
			}
			List<Room> roomsOkay = new List<Room>();
			bool restart = false;
			while (roomsOkay.Count() < map.rooms.Count())
			{
				restart = false;
				foreach (Room room in map.rooms.Where(x => !roomsOkay.Contains(x)))
				{
					if (debug)
					{
						//Console.SetCursorPosition(room.xPos + 1, room.yPos + 1);
						//Console.Write(room.roomsConnectedTo.Count());
						ConsoleHelper.GameGotoLine(3);
						ConsoleHelper.GameClearLine();
						ConsoleHelper.GameWrite("Rooms okay are " + roomsOkay.Count());
					}
					if (room.roomsConnectedTo.Count < map.rooms.Count())
					{
						foreach (Room otherRoom in map.rooms)
						{
							if (otherRoom != room && !room.roomsConnectedTo.Contains(otherRoom))
							{
								if (ConnectTwoRooms(room, otherRoom))
								{
									roomsOkay.Add(room);
									roomsOkay.Add(otherRoom);
									roomsOkay.AddRange(room.roomsConnectedTo);
									roomsOkay.AddRange(otherRoom.roomsConnectedTo);
									roomsOkay = roomsOkay.Distinct().ToList();
									restart = true;
									break;
								}
							}
						}
					}
					else
					{
						roomsOkay.Add(room);
					}
					if (restart)
					{
						break;
					}
				}
			}

			if (debug)
			{
				ConsoleHelper.GameGotoLine(5);
				ConsoleHelper.GameWriteLine("Rooms connected");
			}
		}
		private bool ConnectTwoRooms(Room room1, Room room2)
		{
			#region Finding the closest 2 tiles
			double closest = Math.Sqrt(Math.Pow((room1.xPos - room2.xPos), 2) + Math.Pow((room1.yPos - room2.yPos), 2));
			Tile closestTile1 = room1.doors[0];
			Tile closestTile2 = room2.doors[0];
			foreach (Room room in room1.roomsConnectedTo)
			{
				foreach (Room otherRoom in room2.roomsConnectedTo)
				{
					if (room == otherRoom)
					{
						return true;
					}
					foreach (Door door1 in room.doors)
					{
						foreach (Tile tile1 in door1.doorCorridor.tiles)
						{
							foreach (Door door2 in otherRoom.doors)
							{
								foreach (Tile tile2 in door2.doorCorridor.tiles)
								{
									double currentDistance = TileHelper.GetDistance2Points(tile1.x, tile1.y, tile2.x, tile2.y);
									if (currentDistance < closest && !(tile1 is Door) && !(tile2 is Door))
									{
										closestTile1 = tile1;
										closestTile2 = tile2;
										closest = currentDistance;
									}
								}
							}
						}
					}
				}
			}
			if (closest > map.width / 5)
			{
				return false;
			}
			#endregion

			#region trying to connect the rooms
			int currentX = closestTile2.x;
			int currentY = closestTile2.y;
			if (debug)
			{
				Console.ResetColor();
				Console.BackgroundColor = ConsoleColor.Blue;
				Console.SetCursorPosition(closestTile1.x+1, closestTile1.y+1);
				if (closestTile1.x < 0 || closestTile1.y < 0)
				{
					return false;
				}
				Console.Write(connections);
				Console.SetCursorPosition(closestTile2.x+1, closestTile2.y+1);
				Console.Write(connections);
				Console.ResetColor();
				ConsoleHelper.GameGotoLine(4);
				ConsoleHelper.GameClearLine();
				ConsoleHelper.GameWrite("TRYING TO GET FROM " + closestTile2.x + "," + closestTile2.y + " TO " + closestTile1.x + "," + closestTile1.y);
				Thread.Sleep(sleepyTime*10);
			}
			Tile lastTile = null;
			int trying = 0;
			List<Tile> tilesMade = new List<Tile>();
			Dictionary<Tile, double> costs = new Dictionary<Tile, double>();
			while (true)
			{
				costs.Clear();
				trying++;

				//Console.WriteLine("Still trying..." + trying);
				if (currentX + 1 < map.width && !tilesMade.Contains(map.tiles[currentX + 1, currentY]) && !map.tiles[currentX + 1, currentY].room && !(map.tiles[currentX + 1, currentY] is Door))
				{
					costs[map.tiles[currentX + 1, currentY]] = TileHelper.GetDistance2Points(currentX + 1, currentY, closestTile1.x, closestTile1.y);
				}
				if (currentY + 1 < map.height && !tilesMade.Contains(map.tiles[currentX, currentY + 1]) && !map.tiles[currentX, currentY + 1].room && !(map.tiles[currentX, currentY + 1] is Door))
				{
					costs[map.tiles[currentX, currentY + 1]] = TileHelper.GetDistance2Points(currentX, currentY + 1, closestTile1.x, closestTile1.y);
				}
				if (currentY > 0 && !tilesMade.Contains(map.tiles[currentX, currentY - 1]) && !map.tiles[currentX, currentY - 1].room && !(map.tiles[currentX, currentY - 1] is Door))
				{
					costs[map.tiles[currentX, currentY - 1]] = TileHelper.GetDistance2Points(currentX, currentY - 1, closestTile1.x, closestTile1.y);
				}
				if (currentX > 0 && !tilesMade.Contains(map.tiles[currentX - 1, currentY]) && !map.tiles[currentX - 1, currentY].room && !(map.tiles[currentX - 1, currentY] is Door))
				{
					costs[map.tiles[currentX - 1, currentY]] = TileHelper.GetDistance2Points(currentX - 1, currentY, closestTile1.x, closestTile1.y);
				}
				double cheapest = 4000000;
				Tile bestTile = map.tiles[currentX, currentY];
				foreach (KeyValuePair<Tile, double> pair in costs)
				{
					if (pair.Value < cheapest)
					{
						cheapest = pair.Value;
						bestTile = pair.Key;
					}
				}
				currentX = bestTile.x;
				currentY = bestTile.y;
				bestTile.type = "Walkable";
				tilesMade.Add(bestTile);
				if (lastTile != null && lastTile == bestTile)
				{
					return false;
				}
				lastTile = bestTile;
				trying = 0;
				if (debug)
				{
					
					map.tiles[currentX, currentY].light = 2;
					Console.BackgroundColor = ConsoleColor.Red;
					map.DrawTile(currentX, currentY);
					Console.SetCursorPosition(currentX + 1, currentY + 1);
					Console.Write(connections);
					Thread.Sleep(sleepyTime * 10);
					Console.ResetColor();
				}
				if (TileHitOtherCorridor(bestTile.x, bestTile.y, closestTile1.corridor))
				{
					connections++;
					room1.roomsConnectedTo.AddRange(room2.roomsConnectedTo.Distinct().ToList().Where(x => !room1.roomsConnectedTo.Contains(x)));
					room1.roomsConnectedTo.Add(room2);
					room2.roomsConnectedTo.AddRange(room1.roomsConnectedTo.Distinct().ToList().Where(x => !room2.roomsConnectedTo.Contains(x)));
					room2.roomsConnectedTo.Add(room1);
					foreach (Room r1 in room1.roomsConnectedTo)
					{
						foreach (Room r2 in room2.roomsConnectedTo)
						{
							if (r1 != room1 && r2 != room2 && r1 != room2 && r1 != r2)
							{
								r1.roomsConnectedTo.Add(r2);
							}
						}
					}
					foreach (Room r2 in room2.roomsConnectedTo)
					{
						foreach (Room r1 in room1.roomsConnectedTo)
						{
							if (r2 != room2 && r1 != room1 && r2 != room1 && r1 != r2)
							{
								r2.roomsConnectedTo.Add(r1);
							}
						}
					}
					return true;
				}
			}
			#endregion
		}
		private List<Corridor> TileHitOtherCorridors(int x, int y)
		{
			List<Tile> adjTiles = TileHelper.GetAdjacentTiles(x, y, map);
			List<Corridor> corridors = new List<Corridor>();
			for (int i = 0; i < adjTiles.Count; i++)
			{
				if ((adjTiles[i].type == "Walkable" && adjTiles[i].corridor.tiles.Count() > 0 && adjTiles[i].corridor != map.tiles[x, y].corridor) ||
					adjTiles[i].type == "Door" && map.tiles[x, y].corridor.startDoor != adjTiles[i])
				{

					corridors.Add(adjTiles[i].corridor);
				}
			}
			return corridors;
		}
		private bool TileHitOtherCorridor(int x, int y, Corridor corridor)
		{
			List<Tile> adjTiles = TileHelper.GetAdjacentTiles(x, y, map);
			List<Corridor> corridors = new List<Corridor>();
			for (int i = 0; i < adjTiles.Count; i++)
			{
				if (adjTiles[i].corridor == corridor)
				{

					return true;
				}
			}
			return false;
		}
		private bool IsOkayForCorridor(int x, int y, Corridor corridor)
		{
			if (x >= map.width || y >= map.height || x < 0 || y < 0 || map.tiles[x, y].room)
			{
				return false;
			}
			else if (map.tiles[x, y].type == "Walkable" ||
					map.tiles[x, y].type == "Door" || map.tiles[x, y].type == "Wall")
			{
				return false;
			}
			List<Tile> adjTiles = TileHelper.GetAdjacentTiles(x, y, map);
			int adjCorridors = 0;
			for (int i = 0; i < adjTiles.Count; i++)
			{
				if (adjTiles[i].type == "Walkable")
				{
					adjCorridors++;
				}
			}
			if (adjCorridors > 1)
			{
				return false;
			}
			return true;
		}
		private void PlaceMonsters()
		{
			int numberOfMonsters = (int)Math.Sqrt(map.width * map.height) / 4;
			while (numberOfMonsters > 0)
			{
				Tile tile = TileHelper.GetRandomTileOfType(map, "Walkable");
				tile.monster = new MonsterUnit(tile.x,tile.y);
				map.monsters.Add(tile.monster);
				numberOfMonsters--;
			}
		}
		private void PlaceItems()
		{
			int numberOfItems = (int)Math.Sqrt(map.width * map.height) / 12;
			while (numberOfItems > 0)
			{
				Tile tile = TileHelper.GetRandomTileOfType(map, "Walkable");
				if (tile.monster == null)
				{
					tile.item = ItemManager.GetRandomItem(level);
				}
				numberOfItems--;
			}
		}
		private void TryCombiningRooms()
		{
			foreach (Room room in map.rooms)
			{
				foreach (Room otherRoom in map.rooms)
				{
					if (room == otherRoom || room.width * room.height > map.width)
					{
						continue;
					}
					if (otherRoom.yPos - (room.yPos + room.height) <= 4 && otherRoom.xPos + 2 < room.xPos + room.width && otherRoom.xPos >= room.xPos)
					{
						CombineRooms(room, otherRoom, "Up");
						TryCombiningRooms();
					}
					else if (otherRoom.xPos - (room.xPos + room.width) <= 4 && otherRoom.yPos + 2 < room.yPos + room.height && otherRoom.yPos >= room.yPos)
					{
						CombineRooms(room, otherRoom, "Left");
						TryCombiningRooms();
					}
				}
			}
		}
		private void CombineRooms(Room room1, Room room2, string place)
		{
			if (place == "Left")
			{
				int randY = random.Next((room1.yPos + room1.height) - (room2.yPos + 4));
				int length = room2.xPos - (room1.xPos + room1.width);
				int startX = room1.xPos + room1.width - 1;
				int startY = room2.yPos + randY;
				Tile[,] tiles = new Tile[length, 5];
				for (int x = startX; x < length; x++)
				{
					for (int y = startY; y < 5; y++)
					{
						if (y == startY || y == 4)
						{
							tiles[x, y].type = "Wall";
						}
						else
						{
							tiles[x, y].type = "Walkable";
						}
					}
				}
			}
		}
	}
}
