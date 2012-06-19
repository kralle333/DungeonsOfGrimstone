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
		int sleepyTime = 10;

		public MapCreator()
		{

		}
		public Map CreateMap(int width, int height, int level)
		{
			map = new Map(width, height);
			int numberOfRooms = random.Next((int)Math.Sqrt(height*width)/4, (int)width);
			int tries = 0;
			while (numberOfRooms > 0)
			{
				if (PlaceRoom())
				{
					numberOfRooms--;
				}
				tries++;
				if (tries > 100000)
				{
					numberOfRooms--;
					tries--;
				}
			}
			map.Draw();
			PlaceDoors();
			map.Draw();
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
			//Console.WriteLine("Put stairway");
			return map;
		}
		private bool PlaceRoom()
		{
			Room room = new Room(random.Next(3, map.width / 4), random.Next(3, map.width / 4), random.Next(map.width - 1), random.Next(map.height - 1));
			if (room.xPos + room.width > map.width || room.yPos + room.height > map.height)
			{
				return false;
			}
			for (int x = room.xPos - 1; x < room.xPos + room.width + 1; x++)
			{
				for (int y = room.yPos - 1; y < room.yPos + room.height + 1; y++)
				{
					if ((x < room.xPos + room.width && x >= map.width) || (y < room.yPos + room.height && y >= map.height) ||
						(x < map.width && y < map.height && y >= 0 && x >= 0 && (map.tiles[x, y].room || map.tiles[x, y].type == "OuterWall")))
					{
						//Console.WriteLine("Couldnt Place Room at:"+x+","+y);
						return false;
					}
				}
			}
			map.AddRoom(room);
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
				if (random.Next(1, 3) == 2 || (room.yPos - 1 > 0 && (room.yPos + room.height - 1 - map.height) >= 2))
				{
					randY = room.yPos;
				}
				else
				{
					randY = room.yPos + room.height - 1;
				}
			}
			else
			{
				randY = random.Next(room.yPos + 1, room.yPos + room.height - 1);
				if (random.Next(1, 3) == 2 || (room.xPos - 1 > 0 && (room.xPos + room.width - 1 - map.width) >= 2))
				{
					randX = room.xPos;
				}
				else
				{
					randX = room.xPos + room.width - 1;
				}
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
					doorsToSet[i].corridor.AddRoom(room);
					doorsToSet[i].corridor.startDoor = doorsToSet[i];
					int currentPositionX = doorsToSet[i].x;
					int currentPositionY = doorsToSet[i].y;
					string direction = "Left";

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
					map.tiles[currentPositionX, currentPositionY].type = "Walkable";
					map.tiles[currentPositionX, currentPositionY].corridor = doorsToSet[i].corridor;
					if (debug)
					{
						map.DrawTile(currentPositionX, currentPositionY);
					}
					List<Corridor> adjCorridors = TileHitOtherCorridors(currentPositionX, currentPositionY);
					if (adjCorridors.Count() > 0)
					{
						adjCorridors.Add(doorsToSet[i].corridor);
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
							if (IsOkayForCorridor(currentPositionX + 1, currentPositionY, doorsToSet[i].corridor))
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
							if (IsOkayForCorridor(currentPositionX - 1, currentPositionY, doorsToSet[i].corridor))
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
							if (IsOkayForCorridor(currentPositionX, currentPositionY + 1, doorsToSet[i].corridor))
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
							if (IsOkayForCorridor(currentPositionX, currentPositionY - 1, doorsToSet[i].corridor))
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
							map.tiles[currentPositionX, currentPositionY].corridor = doorsToSet[i].corridor;
							if (debug)
							{
								map.DrawTile(currentPositionX, currentPositionY);
								Thread.Sleep(sleepyTime);
							}
							doorsToSet[i].corridor.tiles.Add(map.tiles[currentPositionX, currentPositionY]);
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
			foreach (Room room in map.rooms)
			{
				foreach (Door door in room.doors)
				{
					room.roomsConnectedTo.AddRange(door.corridor.roomsConnected);
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
			foreach (Room room in map.rooms)
			{
				if (debug)
				{
					Console.SetCursorPosition(room.xPos + 1, room.yPos + 1);
					Console.Write(room.roomsConnectedTo.Count());
				}
				if (room.roomsConnectedTo.Count < map.rooms.Count())
				{
					foreach (Room otherRoom in map.rooms)
					{
						if (otherRoom != room && !room.roomsConnectedTo.Contains(otherRoom))
						{
							ConnectTwoRooms(room, otherRoom);
							
						}
					}
				}
			}
			if (debug)
			{
				Console.SetCursorPosition(0, map.height + 2);
				Console.WriteLine(map.rooms.Count());
			}
		}
		private void ConnectTwoRooms(Room room1, Room room2)
		{
			double closest = Math.Sqrt(Math.Pow((room1.xPos - room2.xPos), 2) + Math.Pow((room1.yPos - room2.yPos), 2));
			Room closestRoom = room2;
			foreach (Room room in room1.roomsConnectedTo)
			{
				foreach (Room otherRoom in room2.roomsConnectedTo)
				{
					double currentDistance = Math.Sqrt(Math.Pow((room.xPos - otherRoom.xPos), 2) + Math.Pow((room.yPos - otherRoom.yPos), 2));
					if (currentDistance < closest)
					{
						closest = currentDistance;
						closestRoom = otherRoom;
					}
				}
			}
			List<Tile> adjTiles = new List<Tile>();
			bool corridorFoundSimple = false;
			foreach (Door door in closestRoom.doors)
			{
				foreach (Tile tile in door.corridor.tiles)
				{
					adjTiles.Clear();
					adjTiles = TileHelper.GetAdjacentTiles(tile.x, tile.y, map);
					for (int i = 0; i < adjTiles.Count(); i++)
					{
						if (adjTiles[i].type == "UnWalkable")
						{
							List<Corridor> corridors = TileHitOtherCorridors(adjTiles[i].x, adjTiles[i].y,door.corridor);
							if (corridors.Count() > 0)
							{
								if (debug)
								{
									map.DrawTile(adjTiles[i].x, adjTiles[i].y);
									Thread.Sleep(10);
								}
								foreach (Corridor corridor in corridors)
								{
									if (!corridor.roomsConnected.Contains(room2))
									{
										adjTiles[i].type = "Walkable";
										room2.roomsConnectedTo.AddRange(corridor.roomsConnected);
										if (corridor.roomsConnected.Contains(room1))
										{
											corridorFoundSimple = true;
										}
									}
								}
								if (corridorFoundSimple)
								{
									room1.roomsConnectedTo.AddRange(room2.roomsConnectedTo);
									return;
								}
							}
						}
					}
				}
			}
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
		private List<Corridor> TileHitOtherCorridors(int x, int y,Corridor corridor)
		{
			List<Tile> adjTiles = TileHelper.GetAdjacentTiles(x, y, map);
			List<Corridor> corridors = new List<Corridor>();
			for (int i = 0; i < adjTiles.Count; i++)
			{
				if ((adjTiles[i].type == "Walkable" && adjTiles[i].corridor.tiles.Count() > 0 && adjTiles[i].corridor != corridor) ||
					adjTiles[i].type == "Door" && map.tiles[x, y].corridor.startDoor != adjTiles[i])
				{

					corridors.Add(adjTiles[i].corridor);
				}
			}
			return corridors;
		}
		private bool IsOkayForCorridor(int x, int y, Corridor corridor)
		{
			if (x >= map.width || y >= map.height || x < 0 || y < 0 || map.tiles[x, y].room)
			{
				return false;
			}
			else if (map.tiles[x, y].type == "Walkable" || map.tiles[x, y].type == "OuterWall" ||
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
			int numberOfMonsters =(int) Math.Sqrt(map.width * map.height)/4;
			while (numberOfMonsters > 0)
			{
				Tile tile = TileHelper.GetRandomTileOfType(map,"Walkable");
				tile.monster = true;
				numberOfMonsters--;
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
