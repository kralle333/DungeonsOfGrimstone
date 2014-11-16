using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Border
	{
		static public int MainFrameHeight = 39;
		static public int MainFrameWidth = 79;
		static public int GameConsoleHeight = 9;
		static public int GameConsoleWidth = 79;
		static public void Draw(ConsoleColor color)
		{
			Console.SetCursorPosition(0, 0);
			Console.BackgroundColor = color;

			for (int y = 0; y < Console.BufferHeight; y++)
			{
				for (int x = 0; x < Console.BufferWidth; x++)
				{
					if (x == Console.BufferWidth-1 && y == Console.BufferHeight - 1)
					{
						break;
					}
					if (x == 0 || y == 0 || x == Console.BufferWidth - 1 || y == 50 - 1)
					{
						Console.SetCursorPosition(x, y);
						Console.Write(' ');
					}
					else if (x == Console.BufferWidth - 20 || ((y + 11) % 15 == 0 && x > Console.BufferWidth - 20 && y != 49) || y == 40 && x < 80)
					{
						Console.SetCursorPosition(x, y);
						Console.Write(' ');
					}
				}
			}

			Console.SetCursorPosition(0,0);
			Console.ResetColor();
		}
		static public void UpdateDungeonLevel(int level)
		{
			Console.BackgroundColor = ConsoleColor.DarkGray;
			Console.SetCursorPosition(81, 2);
			ConsoleHelper.PaddedWriteLine(18, "Dungeon Level: " + level, ' ');
		}
		static public void DrawStats(Party party, int level)
		{
			Console.BackgroundColor = ConsoleColor.DarkGray;
			Console.SetCursorPosition(81, 1);
			ConsoleHelper.PaddedWriteLine(18, "Party: " + party.name, ' ');
			Console.SetCursorPosition(81, 2);
			ConsoleHelper.PaddedWriteLine(18, "Dungeon Level: " + level, ' ');
			Console.SetCursorPosition(81, 3);
			ConsoleHelper.WriteBlanks(18);
			for (int i = 0; i < party.characters.Count(); i++)
			{
				
				for (int j = 1; j < 15; j++)
				{
					Console.SetCursorPosition(81, j + 15 * i + 4);
					switch (j)
					{
						case 1: ConsoleHelper.PaddedWriteLine(18, party.characters[i].name+ " the", ' '); break;
						case 2: ConsoleHelper.PaddedWriteLine(18, party.characters[i].classType, ' '); break;
						case 3: ConsoleHelper.PaddedWriteLine(18, "Level: " + party.characters[i].level, ' '); break;
						case 5: if (party.characters[i].currentHp > 0) { ConsoleHelper.PaddedWriteLine(18, party.characters[i].image, ' '); } else { ConsoleHelper.PaddedWriteLine(18, "X_x", ' '); } break;
						case 10: ConsoleHelper.PaddedWriteLine(18, "Hp: " + party.characters[i].currentHp+"/"+party.characters[i].hp, ' '); break;
						case 11: ConsoleHelper.PaddedWriteLine(18, "Attack: " + party.characters[i].attack, ' '); break;
						case 12: ConsoleHelper.PaddedWriteLine(18, "Defense: " + party.characters[i].defense, ' '); break;
						case 13: ConsoleHelper.PaddedWriteLine(18, "Talent: " + party.characters[i].talent, ' '); break;
						case 14: ConsoleHelper.PaddedWriteLine(18, "Speed: " + party.characters[i].speed, ' '); break;
						default: ConsoleHelper.WriteBlanks(18); break;
					}

				}

			}
			Console.ResetColor();
		}
	}
}
