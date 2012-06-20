using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RPGAsci
{
 class Program
 {
  static private Party party = new Party();
  static private int level = 1;
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
   CharacterManager.Init();

   CreateParty();
   hs.UpdateInfo(party);
   hs.Draw();
   currentMap = mc.CreateMap(random.Next(12, 12 * level), random.Next(12, 12 * level), level);
   currentMap.PlaceHero();
   currentMap.Draw();

   while (true)
   {
    if (currentMap.inBattle)
    {
     Console.Clear();
     Battle b = bc.CreateBattle(party, level);
     b.DrawBattle();
     while (!b.over)
     {
      b.UpdateTurns();
      hs.UpdateInfo(party);
      hs.Draw();
     }
     if (!b.won)
     {
      break;
     }
     else
     {
      currentMap.inBattle = false;
      currentMap.RemoveMonsterAfterBattle();
      Console.Clear();
      currentMap.Draw();
      hs.Draw();
     }
    }
    currentMap.ReadInput();
    if (currentMap.stairwayFound)
    {
     level++;
     Console.Clear();
     currentMap = mc.CreateMap(random.Next(12, 12 * level), random.Next(12, 12 * level), level);
     currentMap.PlaceHero();
     currentMap.Draw();
     hs.Draw();
    }

   }

   Console.ReadKey(true);
  }
  static void CreateParty()
  {
   MenuItem classSelectionMenu = new MenuItem("Classes:");
   classSelectionMenu.AddChild(new MenuItem("Fighter"));
   classSelectionMenu.AddChild(new MenuItem("Wizard"));
   classSelectionMenu.AddChild(new MenuItem("Ranger"));
   classSelectionMenu.currentlySelected = true;

   MenuItem levelUpChoice = new MenuItem("Choose First Bonus");
   levelUpChoice.currentlySelected = true;

   Console.WriteLine("Welcome To Dungeon Master Bro");
   Console.WriteLine("Create your party");
   party.name = ConsoleHelper.SimpleRead("Name of Party", 10);
   Console.WriteLine("Create your party! 3 members are allowed");
   Console.ReadKey(true);
   for (int i = 0; i < 3; i++)
   {
    Console.Clear();
    classSelectionMenu.Reset();
    StringBuilder sb = new StringBuilder();
    sb.Append("Name of " + (i + 1).ToString() + ". party member");
    string name = ConsoleHelper.SimpleRead(sb.ToString(), 8);
    string image = ConsoleHelper.SimpleRead("Draw/type the appearance", 4);
    classSelectionMenu.Draw();
    while (true)
    {
     classSelectionMenu.ReadInput();
     classSelectionMenu.Draw();
     string classType = classSelectionMenu.GetSelectedItem(1);
     if (classType != "")
     {
      party.characters.Add(CharacterManager.CreateCharacter(classType, name, image));
      break;
     }
    }

    List<string> choices = CharacterManager.GetLevelChoices(party.characters[i]);
    if (choices.Count() == 0)
    {
     continue;
    }
    levelUpChoice = new MenuItem("Choose First Bonus");
    levelUpChoice.currentlySelected = true;
    for (int j = 0; j < choices.Count(); j++)
    {
     levelUpChoice.AddChild(new MenuItem(choices[j]));
     if (j == 0)
     {
      levelUpChoice.children[j].currentlyMarked = true;
     }
    }
    levelUpChoice.Draw();
    while (true)
    {
     levelUpChoice.ReadInput();
     levelUpChoice.Draw();
     string choice = levelUpChoice.GetSelectedItem(1);
     if (choice != "")
     {
      CharacterManager.SetChoice(party.characters[i], choice);
      break;
     }
    }
   }
   Console.Clear();
  }
 }
}
