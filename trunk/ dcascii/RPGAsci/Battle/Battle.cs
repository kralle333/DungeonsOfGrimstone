using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RPGAsci
{
	class Battle
	{
		List<Monster> monsters = new List<Monster>();
		Party party;
		Random random = new Random();
		List<Unit> remainingUnits = new List<Unit>();
		List<Character> remainingCharacters = new List<Character>();
		int round = 1;
		Random rand = new Random();
		public bool over = false;
		public bool won = false;
		private bool newTurn = false;
		public MenuItem battleMenu = new MenuItem("Menu", 0, 0);
		public MenuItem targetMenu = new MenuItem("Target:", 0, 0);
		public MenuItem levelUpChoice = new MenuItem("Choose Bonus", 0, 0);
		private int experience = 0;

		public Battle(List<Monster> monsters, Party party)
		{
			this.monsters = monsters;
			this.party = party;
			remainingUnits.AddRange(monsters);
			remainingUnits.AddRange(party.characters);
			remainingUnits.OrderBy(x => x.speed);
			remainingUnits.Reverse();
			remainingCharacters.AddRange(party.characters);
			InitMenu();
			foreach (Character character in remainingCharacters)
			{
				character.skillsLeft = new Dictionary<Skill, int>(character.skills);
			}
		}
		private void InitMenu()
		{
			MenuItem a = new MenuItem("Attack", "Use your weapon to damage the enemy");
			MenuItem s = new MenuItem("Skills", "Available skills to use");
			MenuItem i = new MenuItem("Items", "Available items to use");
			MenuItem d = new MenuItem("Defend", "Defend for this round, gives a temporary bonus to defense");
			MenuItem f = new MenuItem("Flee", "Try to flee the battle, you will not get any experience,gold or items");
			battleMenu.AddChild(a);
			battleMenu.AddChild(s);
			battleMenu.AddChild(i);
			battleMenu.AddChild(d);
			battleMenu.AddChild(f);
			foreach (Monster monster in monsters)
			{
				a.AddChild(new MenuItem(monster.name));
			}
			battleMenu.currentlySelected = true;
		}
		public void UpdateTurns()
		{
			newTurn = true;
			foreach (Unit unit in remainingUnits)
			{
				if (!unit.usedTurn && unit.currentHp > 0)
				{
					ConsoleHelper.ClearConsole();
					if (unit is Monster)
					{
						HandleMonsterTurn(unit);
						if (remainingCharacters.Count() == 0)
						{
							return;
						}
					}
					else if (unit is Character)
					{
						if (HandlePlayerTurn(unit))
						{
							return;
						}
					}
				}
				if (newTurn)
				{
					foreach (Monster m in monsters)
					{
						m.usedTurn = false;
					}
					foreach (Character character in party.characters)
					{
						character.usedTurn = false;
					}
					round++;
					ConsoleHelper.ClearConsole();
					ConsoleHelper.GameWriteLine(round + ". round started");
					Console.ReadKey(true);
				}
				if (monsters.Count() == 0)
				{
					BattleWon();
					return;
				}
			}
			Border.DrawStats(party, Program.level);
		}

		private void HandleMonsterTurn(Unit unit)
		{
			ConsoleHelper.ClearConsole();
			Character randCharacter = remainingCharacters[rand.Next(remainingCharacters.Count())];
			ConsoleHelper.GameWrite(unit.name + " attacked " + randCharacter.name);
			unit.Attack(randCharacter);
			if (randCharacter.currentHp <= 0)
			{
				ConsoleHelper.GameWriteLine(randCharacter.name + " died!");
				randCharacter.currentHp = 0;
				remainingCharacters.Remove(randCharacter);
				if (remainingCharacters.Count == 0)
				{
					ConsoleHelper.GameWriteLine("Party Lost Battle!");
					over = true;
					won = false;
					return;
				}
			}
			ConsoleHelper.GameWriteLine();
			Console.ReadKey(true);
			newTurn = false;
		}
		private bool HandlePlayerTurn(Unit unit)
		{
			bool usedTurn = false;
			battleMenu.Reset();
			battleMenu.currentlySelected = true;
			if (unit.defending)
			{
				unit.defending = false;
			}
			battleMenu.text = unit.name + "'s turn: What do you want to do?";
			battleMenu.Draw();
			while (!usedTurn)
			{
				battleMenu.ReadInput(Console.ReadKey(true));
				if (battleMenu.IsSelected("Attack"))
				{
					usedTurn = HandleAttack(unit);
				}
				else if (battleMenu.IsSelected("Defend"))
				{
					usedTurn = HandleDefend(unit);
				}
				else if (unit.skillsLeft.Count() > 0 && battleMenu.IsSelected("Skills"))
				{
					usedTurn = HandleSkillsUse(unit);
				}
				else if (party.items.Count() > 0 && battleMenu.IsSelected("Items"))
				{
					usedTurn = HandleItemUse(unit);
				}
				else if (battleMenu.IsSelected("Flee"))
				{
					if (HandleFlee())
					{
						return true;
					}
					else
					{
						usedTurn = true;
					}
				}
			}
			if (won)
			{
				return true;
			}
			battleMenu.GetItem("Skills").children.Clear();
			battleMenu.GetItem("Items").children.Clear();
			return false;
		}

		private bool HandleAttack(Unit unit)
		{
			int monsterIndex = battleMenu.IsChildrenPressed("Attack");
			if (monsterIndex == -1)
			{
				return false;
			}
			ConsoleHelper.GameWriteLine();
			Monster monster = monsters[monsterIndex];
			ConsoleHelper.GameWrite(unit.name + " attacked " + monster.name);
			unit.Attack(monster);
			Console.ReadKey(true);
			if (monster.currentHp <= 0)
			{
				MonsterKilled(monster);
			}
			ConsoleHelper.GameWriteLine();
			newTurn = false;
			return true;
		}
		private bool HandleSkillsUse(Unit unit)
		{
			MenuItem skills = battleMenu.GetItem("Skills");
			if (skills.children.Count() > 0)
			{
				int skillIndex = battleMenu.IsChildrenPressed("Skills");
				if (skillIndex == -1)
				{
					return false;
				}
				else
				{
					Skill currentSkill = CharacterManager.GetSkill(battleMenu.GetSelectedItem(2));
					if (currentSkill.target == "Self")
					{
						unit.skillsLeft[currentSkill]--;
						currentSkill.Use(unit, unit);
						newTurn = false;
						targetMenu.Reset();
						targetMenu.children.Clear();
						Console.ReadKey(true);
						return true;
					}
					else if (currentSkill.target == "All")
					{
						if (currentSkill.offensive)
						{
							foreach (Monster m in monsters)
							{
								ConsoleHelper.ClearConsole();
								currentSkill.Use(unit, m);
								Console.ReadKey(true);
								if (m.currentHp <= 0)
								{
									if (MonsterKilled(m))
									{
										return true;
									}
								}
							}
						}
						else
						{
							foreach (Unit c in remainingCharacters)
							{
								ConsoleHelper.ClearConsole();
								currentSkill.Use(unit, c);
								Console.ReadKey(true);
							}
						}
						unit.skillsLeft[currentSkill]--;
						newTurn = false;
						targetMenu.Reset();
						targetMenu.children.Clear();
						Console.ReadKey(true);
						return true;
					}
					else if (currentSkill.target == "Single")
					{
						ConsoleHelper.ClearConsole();
						targetMenu.text = "Target " + currentSkill.name + " on who?";
						if (currentSkill.offensive)
						{
							foreach (Monster m in monsters)
							{
								targetMenu.AddChild(new MenuItem(m.name));
							}
						}
						else
						{
							foreach (Character c in remainingCharacters)
							{
								targetMenu.AddChild(new MenuItem(c.name));
							}
						}
						targetMenu.Draw();
						targetMenu.currentlySelected = true;
						while (true)
						{
							targetMenu.ReadInput(Console.ReadKey(true));
							MenuItem item = targetMenu.childSelected;
							if (item != null)
							{
								if (currentSkill.offensive)
								{
									Monster target = monsters.Find(x => x.name == item.text);
									ConsoleHelper.ClearConsole();
									currentSkill.Use(unit, target);
									Console.ReadKey(true);
									unit.skillsLeft[currentSkill]--;
									if (target.currentHp <= 0)
									{
										return MonsterKilled(target);
									}
								}
								else
								{
									Character target = remainingCharacters.Find(x => x.name == item.text);
									ConsoleHelper.ClearConsole();
									currentSkill.Use(unit, target);
									Console.ReadKey(true);
									unit.skillsLeft[currentSkill]--;
								}

								targetMenu.Reset();
								targetMenu.children.Clear();
								Console.ReadKey(true);
								return true;
							}
						}
					}
				}
			}
			else
			{
				foreach (KeyValuePair<Skill, int> pair in unit.skillsLeft)
				{
					MenuItem item = new MenuItem(pair.Value + "x " + pair.Key.name);
					if (pair.Value == 0)
					{
						item.locked = true;
					}
					skills.AddChild(item);
				}
				skills.DrawChildren();
			}
			return false;
		}
		private bool HandleDefend(Unit unit)
		{
			unit.defending = true;
			unit.usedTurn = true;
			ConsoleHelper.GameWriteLine();
			ConsoleHelper.GameWriteLine(unit.name + " is defending                  ");
			newTurn = false;
			Console.ReadKey(true);
			return true;
		}
		private bool HandleItemUse(Unit unit)
		{
			MenuItem items = battleMenu.GetItem("Items");
			if (items.children.Count() > 0)
			{
				int itemIndex = battleMenu.IsChildrenPressed("Items");
				if (itemIndex == -1)
				{
					return false;
				}
				else
				{
					Item currentItem = ItemManager.GetItem(battleMenu.GetSelectedItem(2));
					if (currentItem.target == "Self")
					{
						currentItem.Use(unit);
						party.items.Remove(currentItem);
						newTurn = false;
						Console.ReadKey(true);
						targetMenu.Reset();
						targetMenu.children.Clear();
						return true;
					}
					else if (currentItem.target == "All")
					{
						if (currentItem.offensive)
						{
							foreach (Monster m in monsters)
							{
								ConsoleHelper.ClearConsole();
								currentItem.Use(m);
								Console.ReadKey(true);
								if (MonsterKilled(m))
								{
									return true;
								}
							}
						}
						else
						{
							foreach (Unit c in remainingCharacters)
							{
								ConsoleHelper.ClearConsole();
								currentItem.Use(c);
								Console.ReadKey(true);
							}
						}
						party.items.Remove(currentItem);
						items.children.Clear();
						newTurn = false;

						targetMenu.children.Clear();
						targetMenu.Reset();
						Console.ReadKey(true);
						return true;
					}
					else if (currentItem.target == "Single")
					{
						ConsoleHelper.ClearConsole();
						targetMenu.text = "Target " + currentItem.name + " on who?";
						if (currentItem.offensive)
						{
							foreach (Monster m in monsters)
							{
								targetMenu.AddChild(new MenuItem(m.name));
							}
						}
						else
						{
							foreach (Character c in remainingCharacters)
							{
								targetMenu.AddChild(new MenuItem(c.name));
							}
						}
						targetMenu.Draw();
						targetMenu.currentlySelected = true;
						while (true)
						{
							targetMenu.ReadInput(Console.ReadKey(true));
							MenuItem item = targetMenu.childSelected;
							if (item != null)
							{
								if (currentItem.offensive)
								{
									Monster target = monsters.Find(x => x.name == item.text);
									ConsoleHelper.ClearConsole();
									currentItem.Use(target);
									Console.ReadKey(true);
									party.items.Remove(currentItem);
									targetMenu.Reset();
									targetMenu.children.Clear();

									if (target.currentHp <= 0)
									{

										return MonsterKilled(target);
									}
								}
								else
								{
									Character target = remainingCharacters.Find(x => x.name == item.text);
									ConsoleHelper.ClearConsole();
									currentItem.Use(target);
									Console.ReadKey(true);
									party.items.Remove(currentItem);
								}
								targetMenu.Reset();
								targetMenu.children.Clear();
								return true;
							}
						}
					}
				}
			}
			else
			{
				foreach (KeyValuePair<Item, int> pair in party.items)
				{
					MenuItem item = new MenuItem(pair.Value + "x " + pair.Key.name);
					if (pair.Value == 0)
					{
						item.locked = true;
					}
					items.AddChild(item);
				}
				items.DrawChildren();
			}
			return false;
		}
		private bool HandleFlee()
		{
			int monsterPower = 0;
			foreach (Monster monster in monsters)
			{
				monsterPower += monster.GetPower();
			}
			if (random.Next(0, (party.power / monsterPower) * 100) >= 50)
			{
				ConsoleHelper.GameWriteLine(party.name + " tried to flee the battle....");
				Console.ReadKey(true);
				ConsoleHelper.GameWriteLine(party.name + " succeded fleeing!");
				Console.ReadKey(true);
				over = true;
				won = true;
				return true;
			}
			else
			{
				ConsoleHelper.GameWriteLine(party.name + " tried to flee the battle....");
				Console.ReadKey(true);
				ConsoleHelper.GameWriteLine("but " + party.name + " was caught by the enemy!");
				Console.ReadKey(true);
				return false;
			}
		}
		private void BattleWon()
		{
			ConsoleHelper.ClearConsole();
			ConsoleHelper.GameWriteLine("Party Won Battle!");
			ConsoleHelper.GameWriteLine("Party got " + experience + " experience");
			Console.ReadKey(true);
			experience /= remainingCharacters.Count();
			foreach (Character character in remainingCharacters)
			{
				character.experience += experience;
				if ((character.experience / 1000) * character.level >= 1)
				{
					ConsoleHelper.ClearConsole();
					levelUpChoice.Reset();
					character.level++;
					character.experience = (character.experience % 1000);
					ConsoleHelper.GameWriteLine(character.name + " reached level " + character.level);
					Console.ReadKey(true);
					CharacterManager.SetLevelChanges(character);
					List<string> choices = CharacterManager.GetLevelChoices(character);
					if (choices.Count() == 0)
					{
						continue;
					}
					levelUpChoice.children.Clear();
					for (int i = 0; i < choices.Count(); i++)
					{
						levelUpChoice.AddChild(new MenuItem(choices[i]));
					}
					levelUpChoice.Draw();
					while (true)
					{
						levelUpChoice.ReadInput(Console.ReadKey(true));
						string choice = levelUpChoice.GetSelectedItem(1);
						if (choice != "")
						{
							CharacterManager.SetChoice(character, choice);
							break;
						}
					}
				}

			}
			foreach (Character c in party.characters)
			{
				if (c.currentHp == 0)
				{
					c.currentHp = 1;
				}
				c.usedTurn = false;
			}
			over = true;
			won = true;
		}
		private bool MonsterKilled(Monster m)
		{
			ConsoleHelper.GameWriteLine(m.name + " died!");
			Console.ReadKey(true);
			experience += m.power;
			monsters.Remove(m);
			m.usedTurn = true;
			battleMenu.RemoveChild(m.name);
			DrawBattle();
			if (monsters.Count == 0)
			{
				BattleWon();
				return true;
			}
			return false;
		}
		public void DrawBattle()
		{

			int cursorSize = Console.CursorSize;
			Console.SetCursorPosition(10, 10);
			/*foreach(Monster monster in monsters)
			{
				
				ConsoleHelper.PaddedWriteLine((4-(monsters.Count() /3)) *6, monster.name, ' ');
			}*/
			ConsoleHelper.ClearMainFrame();
			if (monsters.Count() > 0)
			{
				int drawXIndex = 10;
				int drawXAdd = ((Border.MainFrameWidth - 20) / monsters.Count()) - 5 * monsters.Count();
				foreach (Monster monster in monsters)
				{
					string file;
					if (File.Exists("Art//Monsters//" + monster.image))
					{
						file = "Art//Monsters//" + monster.image;
						string line;
						int currentLine = 10;
						int width = 0;
						using (StreamReader reader = new StreamReader(file))
						{
							while ((line = reader.ReadLine()) != null)
							{
								Console.SetCursorPosition(drawXIndex, currentLine);
								foreach (Char c in line.ToCharArray())
								{
									if (c == '0')
									{
										Console.BackgroundColor = ConsoleColor.Black;
										Console.Write(' ');
									}
									else if (c == '1')
									{
										Console.BackgroundColor = ConsoleColor.White;
										Console.Write(' ');
									}
									else if (c == '2')
									{
										Console.BackgroundColor = ConsoleColor.Gray;
										Console.Write(' ');
									}
									else if (c == '3')
									{
										Console.BackgroundColor = ConsoleColor.Yellow;
										Console.Write(' ');
									}
									else if (c == '4')
									{
										Console.BackgroundColor = ConsoleColor.Green;
										Console.Write(' ');
									}
									width = line.ToCharArray().Length;
								}
								currentLine++;
							}
						}
						drawXIndex += drawXAdd + width + (15 - width);
						Console.ResetColor();
					}
				}
			}
		}
	}
}
