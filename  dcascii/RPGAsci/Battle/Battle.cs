using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		public MenuItem battleMenu = new MenuItem("Menu");
		public MenuItem targetMenu = new MenuItem("Target:");
		public MenuItem levelUpChoice = new MenuItem("Choose Bonus");
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
				character.skillsLeft = new List<Skill>(character.skills);
			}
		}
		private void InitMenu()
		{
			MenuItem a = new MenuItem("Attack");
			MenuItem s = new MenuItem("Skills");
			MenuItem i = new MenuItem("Items");
			MenuItem d = new MenuItem("Defend");
			MenuItem f = new MenuItem("Flee");
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
			a.currentlyMarked = true;
		}
		public void UpdateTurns()
		{
			newTurn = true;
			foreach (Unit unit in remainingUnits)
			{
				if (!unit.usedTurn && unit.currentHp > 0)
				{
					if (unit is Monster)
					{
						Console.Clear();
						DrawBattle();
						Character randCharacter = remainingCharacters[rand.Next(remainingCharacters.Count())];
						Console.Write(unit.name + " attacked " + randCharacter.name);
						unit.Attack(randCharacter);
						if (randCharacter.currentHp <= 0)
						{
							Console.WriteLine("\n" + randCharacter.name + " died!");
							randCharacter.currentHp = 0;
							remainingCharacters.Remove(randCharacter);
							if (remainingCharacters.Count == 0)
							{
								Console.WriteLine("Party Lost Battle!");
								over = true;
								won = false;
								return;
							}
							break;
						}
						Console.WriteLine("\n");
						Console.ReadKey(true);
						newTurn = false;
					}
					else if (unit is Character)
					{
						bool usedTurn = false;
						battleMenu.Reset();
						battleMenu.currentlySelected = true;
						battleMenu.children[0].currentlyMarked = true;
						if (unit.defending)
						{
							unit.defending = false;
						}
						Console.Clear();
						DrawBattle();
						Console.WriteLine(unit.name + "'s turn: What do you want to do?");
						battleMenu.Draw();
						while (!usedTurn)
						{
							battleMenu.ReadInput();
							battleMenu.Draw();
							if (battleMenu.IsSelected("Attack"))
							{
								int monsterIndex = battleMenu.IsChildrenPressed("Attack");
								if (monsterIndex == -1)
								{
									continue;
								}
								Console.WriteLine();
								Monster monster = monsters[monsterIndex];
								Console.Write(unit.name + " attacked " + monster.name);
								unit.Attack(monster);
								if (monster.currentHp <= 0)
								{
									Console.WriteLine("\n" + monster.name + " died!");
									experience += monster.power;
									monsters.Remove(monster);
									monster.usedTurn = true;
									battleMenu.RemoveChild(monster.name);
									if (monsters.Count == 0)
									{
										BattleWon();
										return;
									}
								}
								Console.WriteLine("\n");
								usedTurn = true;
								newTurn = false;
								Console.ReadKey(true);
								break;

							}
							else if (battleMenu.IsSelected("Defend"))
							{
								unit.defending = true;
								unit.usedTurn = true;
								Console.WriteLine();
								Console.WriteLine(unit.name + " is defending                  ");
								newTurn = false;
								usedTurn = true;
								Console.ReadKey(true);
								break;
							}
							else if (unit.skillsLeft.Count()>0 && battleMenu.IsSelected("Skills"))
							{
								MenuItem skills = battleMenu.GetItem("Skills");
								if (skills.children.Count()>0)
								{
									int skillIndex = battleMenu.IsChildrenPressed("Skills");
									if (skillIndex == -1)
									{
										continue;
									}
									else
									{
										if (unit.skillsLeft[skillIndex].target == "Self")
										{
											unit.skillsLeft.RemoveAt(skillIndex);
											newTurn = false;
											usedTurn = true;
											Console.ReadKey(true);
											break;
										}
										else if (unit.skillsLeft[skillIndex].target == "All")
										{
											if (unit.skillsLeft[skillIndex].offensive)
											{
												foreach (Unit m in monsters)
												{
													unit.skillsLeft[skillIndex].Use(unit, m);
												}
											}
											else
											{
												foreach (Unit c in remainingCharacters)
												{
													unit.skillsLeft[skillIndex].Use(unit, c);
												}
											}
											unit.skillsLeft.RemoveAt(skillIndex);
											newTurn = false;
											usedTurn = true;
											Console.ReadKey(true);
										}
										else if (unit.skillsLeft[skillIndex].target == "Single")
										{
											targetMenu = new MenuItem("Target " + unit.skillsLeft[skillIndex].name + " on who?");
											if (unit.skillsLeft[skillIndex].offensive)
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
											battleMenu.Clear();
											targetMenu.Draw();
											targetMenu.currentlySelected = true;
											while (true)
											{
												targetMenu.ReadInput();
												targetMenu.Draw();
												MenuItem item = targetMenu.childSelected;
												if (item != null)
												{
													if (unit.skillsLeft[skillIndex].offensive)
													{
														unit.skillsLeft[skillIndex].Use(unit, monsters.Find(x=>x.name==item.text));
													}
													else
													{
														unit.skillsLeft[skillIndex].Use(unit, remainingCharacters.Find(x => x.name == item.text));
													}
													
													break;
												}
											}
										}
									}
								}
								else
								{
									foreach (Skill skill in unit.skillsLeft)
									{
										skills.AddChild(new MenuItem(skill.name));
									}
									battleMenu.Draw();
								}
							}
							else if (party.items.Count()>0 && battleMenu.IsSelected("Items"))
							{
								MenuItem items = battleMenu.GetItem("Items");
								if (items.children.Count() > 0)
								{
									int itemIndex = battleMenu.IsChildrenPressed("Items");
									if (itemIndex == -1)
									{
										continue;
									}
								}
								else
								{
									foreach (Item item in party.items)
									{
										items.AddChild(new MenuItem(item.name));
									}
								}

							}
							else if (battleMenu.IsSelected("Flee"))
							{
								int monsterPower = 0;
								foreach (Monster monster in monsters)
								{
									monsterPower += monster.GetPower();
								}
								if (random.Next(0, (party.power / monsterPower) * 100) >= 50)
								{
									Console.WriteLine(party.name + " fled the battle....");
									Console.ReadKey(true);
									over = true;
									won = false;
								}
							}
						}
						battleMenu.GetItem("Skills").children.Clear();
						battleMenu.GetItem("Items").children.Clear();
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
					Console.WriteLine(round + ". round started");
				}
			}
		}
		private void BattleWon()
		{
			Console.WriteLine("Party Won Battle!");
			Console.WriteLine("Party got " + experience + " experience");
			Console.ReadKey(true);
			experience /= 3;
			foreach (Character character in remainingCharacters)
			{
				character.experience += experience;
				if ((character.experience / 1000) * character.level >= 1)
				{
					character.level++;
					character.experience = 0;
					Console.WriteLine(character.name + " reached level " + character.level);
					Console.ReadKey(true);
					CharacterManager.SetLevelChanges(character);
					List<string> choices = CharacterManager.GetLevelChoices(character);
					if (choices.Count() == 0)
					{
						continue;
					}
					levelUpChoice = new MenuItem("Choose Bonus");
					levelUpChoice.currentlySelected = true;
					for (int i = 0; i < choices.Count(); i++)
					{
						levelUpChoice.AddChild(new MenuItem(choices[i]));
						if (i == 0)
						{
							levelUpChoice.children[i].currentlyMarked = true;
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
							CharacterManager.SetChoice(character, choice);
							break;
						}
					}
				}

			}
			over = true;
			won = true;
		}
		public void DrawBattle()
		{
			Console.Write("\n \n     ");
			foreach (Monster monster in monsters)
			{
				Console.Write(monster.name);
				Console.Write("		");
			}
			Console.Write("\n     ");
			foreach (Monster monster in monsters)
			{
				Console.Write(monster.image);
				Console.Write("		");
			}
			Console.Write("\n \n \n \n     ");
			foreach (Character character in party.characters)
			{
				Console.Write(character.name);
				Console.Write("	    ");
			}
			Console.Write("\n     ");
			foreach (Character character in party.characters)
			{
				Console.Write(character.image);
				Console.Write("	    ");
			}
			Console.Write("\n     ");
			foreach (Character character in party.characters)
			{
				Console.Write("HP: " + character.currentHp);
				Console.Write("	    ");
			}
			Console.WriteLine("\n");
		}
	}
}
