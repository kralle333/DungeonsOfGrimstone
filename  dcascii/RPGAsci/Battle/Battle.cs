using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RPGAsci.ConsoleDrawing;

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
						if (!remainingCharacters.Any())
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
				remainingUnits.OrderBy(u => u.currentSpeed);
				round++;
				ConsoleHelper.ClearConsole();
				ConsoleHelper.GameWriteLine(round + ". round started");
				Console.ReadKey(true);
			}
			if (!monsters.Any())
			{
				BattleWon();
				return;
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
			ConsoleHelper.ClearConsole();
			battleMenu.Reset();
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
				else if (unit.skillsLeft.Any()&& battleMenu.IsSelected("Skills"))
				{
					usedTurn = HandleSkillsUse(unit);
				}
				else if (party.items.Any()  && battleMenu.IsSelected("Items"))
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
			battleMenu.GetItem("Skills").Reset();
			battleMenu.GetItem("Items").Reset();
			return false;
		}

		private bool HandleAttack(Unit unit)
		{
			int monsterIndex = battleMenu.GetIndexOfSelectedChild("Attack");
			if (monsterIndex == -1)
			{
				return false;
			}
			Monster monster = monsters[monsterIndex];
			ConsoleHelper.ClearConsole();
			ConsoleHelper.GameWrite(unit.name + " attacked " + monster.name);
			unit.Attack(monster);
			if (monster.currentHp <= 0)
			{
				MonsterKilled(monster);
			}
			else
			{
				DrawMonsterHp();
			}
			Console.ReadKey(true);
			ConsoleHelper.GameWriteLine();
			newTurn = false;
			return true;
		}
		private bool HandleSkillsUse(Unit unit)
		{
			MenuItem skills = battleMenu.GetItem("Skills");
			if (skills.children.Count() > 0)
			{
				int skillIndex = battleMenu.GetIndexOfSelectedChild("Skills");
				if (skillIndex == -1)
				{
					return false;
				}
				else
				{
					Skill currentSkill = CharacterManager.GetSkill(battleMenu.GetSelectedItemText(2));
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
							DrawMonsterHp();
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
							DrawMonsterHp();
						}
						else
						{
							foreach (Character c in remainingCharacters)
							{
								targetMenu.AddChild(new MenuItem(c.name));
							}
						}
						targetMenu.Draw();
						while (true)
						{
							targetMenu.ReadInput(Console.ReadKey(true));
							MenuItem item = targetMenu.childSelected;
							if (item != null)
							{
								if (currentSkill.offensive)
								{
									Monster target = monsters[targetMenu.index];
									ConsoleHelper.ClearConsole();
									currentSkill.Use(unit, target);
									Console.ReadKey(true);
									unit.skillsLeft[currentSkill]--;
									targetMenu.Reset();
									targetMenu.children.Clear();
									DrawMonsterHp();
									if (target.currentHp <= 0)
									{
										return MonsterKilled(target);
									}
								}
								else
								{
									Character target = remainingCharacters[targetMenu.index];
									ConsoleHelper.ClearConsole();
									currentSkill.Use(unit, target);
									Console.ReadKey(true);
									unit.skillsLeft[currentSkill]--;
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
				int itemIndex = battleMenu.GetIndexOfSelectedChild("Items");
				if (itemIndex == -1)
				{
					return false;
				}
				else
				{
					Item currentItem = ItemManager.GetItem(battleMenu.GetSelectedItemText(2));
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
						while (true)
						{
							targetMenu.ReadInput(Console.ReadKey(true));
							MenuItem item = targetMenu.childSelected;
							if (item != null)
							{
								if (currentItem.offensive)
								{
									Monster target = monsters[targetMenu.index];
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
									Character target = remainingCharacters[targetMenu.index];
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
				if ((character.experience / 100) * character.level >= 1)
				{
					ConsoleHelper.ClearConsole();
					levelUpChoice.Reset();
					character.level++;
					character.experience = (character.experience % 100);
					ConsoleHelper.GameWriteLine(character.name + " reached level " + character.level);
					Console.ReadKey(true);
					character.SetLevelChanges();
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
			experience += m.experience;
			monsters.Remove(m);
			m.usedTurn = true;
			battleMenu.RemoveChild(m.name);
			ConsoleHelper.ClearConsole();
			DrawBattle();
			if (monsters.Count == 0)
			{
				BattleWon();
				return true;
			}
			return false;
		}

		private void DrawMonsterHp()
		{
			if (monsters.Count > 0)
			{
				int drawXIndex = 10;
				int drawXAdd = ((Border.MainFrameWidth - 20) / monsters.Count()) - 5 * monsters.Count();
				foreach (Monster monster in monsters)
				{
					int currentLine = 10;

					//Health:
					Console.SetCursorPosition(drawXIndex, currentLine - 2);
					int healthLeft = (int)Math.Round((monster.currentHp / (float)monster.hp) * (float)monster.imageWidth);
					Console.BackgroundColor = ConsoleColor.Green;
					Console.Write(new string(' ', healthLeft));
					Console.BackgroundColor = ConsoleColor.Red;
					Console.Write(new string(' ', monster.imageWidth - healthLeft));
					drawXIndex += drawXAdd + monster.imageWidth + (15 - monster.imageWidth);
				}
				Console.ResetColor();
			}
		}
		public void DrawBattle()
		{

			int cursorSize = Console.CursorSize;
			Console.SetCursorPosition(10, 10);
			ConsoleHelper.ClearMainFrame();
			DrawMonsterHp();
			if (monsters.Count() > 0)
			{
				int drawXIndex = 10;
				int drawXAdd = ((Border.MainFrameWidth - 20) / monsters.Count()) - 5 * monsters.Count();
				foreach (Monster monster in monsters)
				{
					int currentLine = 10;
					int width = 0;

					Console.SetCursorPosition(drawXIndex, currentLine);
					foreach (Char c in monster.image.ToCharArray())
					{
						if (c != '\n')
						{
							Console.BackgroundColor = AsciiArtConverter.colorList[(int)Char.GetNumericValue(c)];
							Console.Write(' ');
							width++;
						}
						else
						{
							currentLine++;
							Console.SetCursorPosition(drawXIndex, currentLine);
							width = 0;
						}
					}

					drawXIndex += drawXAdd + monster.imageWidth + (15 - monster.imageWidth);
					Console.ResetColor();

				}
			}
		}
	}
}
