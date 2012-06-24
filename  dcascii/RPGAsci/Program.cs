using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace RPGAsci
{
	class Program
	{
		static public Party party = new Party();
		static public int level = 1;
		static private Random random = new Random();
		static private MapCreator mc = new MapCreator();
		static private HeroStats hs = new HeroStats();
		static private BattleCreator bc = new BattleCreator();
		static private Map currentMap;
		static void Main(string[] args)
		{
			int level = 1;
			
			Console.SetBufferSize(100, 50);
			Console.SetWindowSize(100, 50);
			Console.CursorVisible = false;
			Border.Draw(ConsoleColor.DarkBlue);
			CharacterManager.Init();
			EquipmentManager.Init();
			ItemManager.Init();
			
			CreateParty();
			currentMap = mc.CreateMap(random.Next(12, 12 * (level + 1)), random.Next(12, 12 * (level + 1)), level);
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
					ConsoleHelper.ClearConsole();
					ConsoleHelper.ClearMainFrame();
					currentMap = mc.CreateMap(Clamp(12,Border.MainFrameWidth,random.Next(12, 12 * (level+1))), Clamp(12,Border.MainFrameHeight,random.Next(12, 12 * (level+1))), level);
					currentMap.PlaceHero();
					currentMap.Draw();
				}

			}

			Console.ReadKey(true);
		}
		static void CreateParty()
		{
			MenuItem classSelectionMenu = new MenuItem("Select a class:",0,0);
			classSelectionMenu.AddChild(new MenuItem("Fighter","A powerful warrior, that uses psychical attacks to kill the enemy"));
			classSelectionMenu.AddChild(new MenuItem("Wizard","Uses spells to heal the party and to damage foes"));
			classSelectionMenu.AddChild(new MenuItem("Ranger","Fast and able to inflict status effect and support the party"));

			MenuItem portraitSelection = new MenuItem("Choose your portrait", 0, 0);
			portraitSelection.AddChild(new MenuItem("Y_Y"));
			portraitSelection.AddChild(new MenuItem("Ø_Ø"));
			portraitSelection.AddChild(new MenuItem("^_^"));
			portraitSelection.AddChild(new MenuItem("ô_ô"));
			portraitSelection.AddChild(new MenuItem("Draw portrait"));

			MenuItem levelUpChoice = new MenuItem("Choose First Bonus", 0, 0);
			Console.ResetColor();
			DrawLogo();
			
			ConsoleHelper.GameWriteLine("Welcome To Dungeon Master Bro");
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
					image = portraitSelection.GetSelectedItem(1);
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
					string classType = classSelectionMenu.GetSelectedItem(1);
					if (classType != "")
					{
						party.characters.Add(CharacterManager.CreateCharacter(classType, name, image));
						break;
					}
				}
				ConsoleHelper.ClearConsole();
				levelUpChoice.children.Clear();
				List<string> choices = CharacterManager.GetLevelChoices(party.characters[i]);
				for (int j = 0; j < choices.Count(); j++)
				{
					levelUpChoice.AddChild(new MenuItem(choices[j]));
				}
				levelUpChoice.Draw();
				while (true)
				{
					levelUpChoice.ReadInput(Console.ReadKey(true));
					string choice = levelUpChoice.GetSelectedItem(1);
					if (choice != "")
					{
						CharacterManager.SetChoice(party.characters[i], choice);
						break;
					}
				}
			}
			ConsoleHelper.ClearConsole();
			ConsoleHelper.ClearMainFrame();
		}
		static void DrawLogo()
		{
			string line;
			int currentLine =5;
			using (StreamReader reader = new StreamReader("IntroLogo.txt"))
			{
				while ((line = reader.ReadLine()) != null)
				{
					Console.SetCursorPosition(18,currentLine);
					Console.WriteLine(line);
					currentLine++;
				}
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
