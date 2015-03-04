using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using RPGAsci.ConsoleDrawing;

namespace RPGAsci
{
	class Program
	{
		static public Party party = new Party();
		static public int level = 1;
		static private Random random = new Random();
		static private MapCreator mc = new MapCreator();
		static private BattleCreator bc = new BattleCreator();
		static private Map currentMap;
		static void Main(string[] args)
		{
			level = 1;
			Console.SetBufferSize(100, 50);
			Console.SetWindowSize(100, 50);
            Console.Title = "Dungeons of Grimstone";
			Console.CursorVisible = false;
			Border.Draw(ConsoleColor.DarkBlue);
			CharacterManager.Init();
			EquipmentManager.Init();
			ItemManager.Init();
			
			CreateParty();
			currentMap = mc.CreateMapUsingBSP(random.Next(15, 15 * (level + 1)), random.Next(15, 15 * (level + 1)), level);
			currentMap.PlaceHero();
			Border.DrawStats(party, level);
			
			while (true)
			{
				if (currentMap.inBattle)
				{
					ConsoleHelper.ClearConsole();
					ConsoleHelper.ClearMainFrame();
					Border.Draw(ConsoleColor.Red);
					Battle b = bc.CreateBattle(party, level);
					b.DrawBattle();
					
					while (!b.over)
					{
						b.UpdateTurns();
					}
					if (!b.won)
					{
						break;
					}
					else
					{
						currentMap.inBattle = false;
						currentMap.RemoveMonsterAfterBattle();
						ConsoleHelper.ClearConsole();
						ConsoleHelper.ClearMainFrame();
						Border.Draw(ConsoleColor.DarkBlue);
						Border.DrawStats(party, level);
						currentMap.Draw();
					}
				}
				currentMap.ReadInput();
				if (currentMap.stairwayFound)
				{
					level++;
					Border.UpdateDungeonLevel(level);
					ConsoleHelper.ClearConsole();
					ConsoleHelper.ClearMainFrame();
					currentMap = mc.CreateMapUsingBSP(Clamp(12,Border.MainFrameWidth,random.Next(12, 12 * (level+1))), Clamp(12,Border.MainFrameHeight,random.Next(12, 12 * (level+1))), level);
					currentMap.PlaceHero();
					currentMap.Draw();
				}

			}

			Console.ReadKey(true);
		}
		static void CreateParty()
		{
			MenuItem classSelectionMenu = new MenuItem("Select a class:",0,0);
			classSelectionMenu.AddChild(new MenuItem("Adventurer","A powerful fighter, that uses psychical attacks to kill the enemy"));
			classSelectionMenu.AddChild(new MenuItem("Blacksmith", "Very strong, but also misses a lot, especially if he has been drinking"));
			classSelectionMenu.AddChild(new MenuItem("Ranger","An expert in dungeons. Uses status effecting attacks and eats strange mushrooms"));
			classSelectionMenu.AddChild(new MenuItem("Octonoid","By praying to the octopuss god Catulu he have gained the appearance of an octopuss"));
			classSelectionMenu.AddChild(new MenuItem("Retiree", "Old grumpy person that spends all that extra freetime on fighting in dungeons"));
			classSelectionMenu.AddChild(new MenuItem("Nurse", "Uses spells to heal the party and to damage foes"));
			classSelectionMenu.AddChild(new MenuItem("Thief", "Fast and able to bring your party some extra gold"));

			MenuItem portraitSelection = new MenuItem("Choose your portrait", 0, 0);
			portraitSelection.AddChild(new MenuItem("Y_Y"));
			portraitSelection.AddChild(new MenuItem("Ø_Ø"));
			portraitSelection.AddChild(new MenuItem("^_^"));
			portraitSelection.AddChild(new MenuItem("ô_ô"));
			portraitSelection.AddChild(new MenuItem("Draw portrait"));

			MenuItem levelUpChoice = new MenuItem("Choose First Bonus", 0, 0);
			Console.ResetColor();
			DrawLogo();

            ConsoleHelper.GameWriteLine("Welcome To Dungeons of Grimstone");
			ConsoleHelper.GameWriteLine("Create your party to start the game!\n");	
			Console.ReadKey(true);
			ConsoleHelper.ClearConsole();
			party.name = ConsoleHelper.GameSimpleRead("Name of Party", 10, ConsoleColor.DarkBlue);
			ConsoleHelper.ClearConsole();

			for (int i = 0; i < 3; i++)
			{
				ConsoleHelper.ClearConsole();
				DrawLogo();
				classSelectionMenu.Reset();
				portraitSelection.Reset();
				levelUpChoice.Reset();
				string name = ConsoleHelper.GameSimpleRead("Name of " + (i + 1).ToString() + ". party member", 8, ConsoleColor.DarkBlue);
				portraitSelection.Draw();
				string image = "";
				while (true)
				{
					portraitSelection.ReadInput(Console.ReadKey(true));
					image = portraitSelection.GetSelectedItemText(1);
					if (image.StartsWith("Draw"))
					{
						image = ConsoleHelper.SimpleRead("Draw/type the appearance", 4);
						break;
					}
					else if (image != "") 
					{
						break;
					}
				}
				ConsoleHelper.ClearConsole();
				classSelectionMenu.Draw();
				while (true)
				{
					classSelectionMenu.ReadInput(Console.ReadKey(true));
					string classType = classSelectionMenu.GetSelectedItemText(1);
					if (classType != "")
					{
						switch (classType)
						{
							case "Adventurer": party.characters.Add(new Adventurer(image, name)); break;
							case "Retiree": party.characters.Add(new Retiree(image, name)); break;
							case "Ranger": party.characters.Add(new Ranger(image, name)); break;
							case "Thief": party.characters.Add(new Thief(image, name)); break;
							case "Octonoid": party.characters.Add(new Octonoid(image, name)); break;
							case "Nurse": party.characters.Add(new Nurse(image, name)); break;
							case "Blacksmith": party.characters.Add(new BlackSmith(image, name)); break;
							default: Console.WriteLine("Class Not Found!!!"); break;
						}
						break;
					}
				}
			}
			ConsoleHelper.ClearConsole();
			ConsoleHelper.ClearMainFrame();
		}
		static void DrawLogo()
		{
			int currentLine =5;
			string[] logoLines = System.IO.File.ReadAllLines("IntroLogo.txt");
			
			foreach(string line in logoLines)
			{
				Console.SetCursorPosition(5,currentLine);
				Console.WriteLine(line);
				currentLine++;
			}
		}
		static int Clamp(int smallest, int highest, int value)
		{
			if (value < smallest)
			{
				return smallest;
			}
			else if(value > highest)
			{
				return highest;
			}
			return value;
		}
	}
}
