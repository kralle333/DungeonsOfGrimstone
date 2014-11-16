using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RPGAsci
{
	class MonsterCreator
	{
		struct MonsterTemplate
		{
			public string image;
			public string name;
			public ConsoleColor color;
			public int hpMultiplier;
			public int attackMultiplier;
			public int defenseMultiplier;
			public int speedMultiplier;
			public int talentMultiplier;

			public MonsterTemplate(string image, string name, ConsoleColor color, int hpMultiplier, int attackMultiplier, int defenseMultiplier, int speedMultiplier, int talentMultiplier)
			{
				this.image = image;
				this.name = name;
				this.color = color;
				this.hpMultiplier = hpMultiplier;
				this.attackMultiplier = attackMultiplier;
				this.defenseMultiplier = defenseMultiplier;
				this.speedMultiplier = speedMultiplier;
				this.talentMultiplier = talentMultiplier;
			}
		}
		Random random = new Random();
		List<MonsterTemplate> monsterTypes = new List<MonsterTemplate>();
		public MonsterCreator()
		{
			InitMonsterTypes();
		}
		public Monster GetMonster(int level)
		{
			string monsterFile = "";
			if (level <= 5)
			{
				monsterFile = "Data/Level5.txt";
			}
			string[] monsterInfo = File.ReadAllLines(monsterFile);
			int currentMonster = random.Next(0,  Math.Min(3,level + 1)) * (level / 10 + 1) * 7;
			string monsterName = monsterInfo[currentMonster];
			string image = "";
			string file;
			int widthOfLine = 0;
			if (File.Exists("Art/Monsters/" + monsterName+".txt"))
			{
				file = "Art/Monsters/" + monsterName + ".txt";
				string line;
				using (StreamReader reader = new StreamReader(file))
				{
					while ((line = reader.ReadLine()) != null)
					{
						image += line;
						image += "\n";
						widthOfLine = line.Length;	
					}
				}
			}
			return new Monster(Int32.Parse(monsterInfo[currentMonster + 1]), Int32.Parse(monsterInfo[currentMonster + 2]), Int32.Parse(monsterInfo[currentMonster + 3]), image, widthOfLine,monsterName, Int32.Parse(monsterInfo[currentMonster + 4]), Int32.Parse(monsterInfo[currentMonster + 5]),Int32.Parse(monsterInfo[currentMonster + 6]));
		}
		private void InitMonsterTypes()
		{
			monsterTypes.Add(new MonsterTemplate("bat.txt", "Bat", ConsoleColor.DarkGreen, 1, 1, 1, 1, 1));
			monsterTypes.Add(new MonsterTemplate("slime.txt", "Slime", ConsoleColor.DarkGreen, 1, 1, 1, 1, 1));
			monsterTypes.Add(new MonsterTemplate("Zombie.txt", "Zombie", ConsoleColor.DarkGreen, 1, 1, 1, 1, 1));
			//monsterTypes.Add(new MonsterTemplate(">*_*<", "Butterfly", ConsoleColor.Blue, 1,1, 1, 2,1));
			//monsterTypes.Add(new MonsterTemplate("^(0v0)^", "Owl", ConsoleColor.Gray, 2,2, 1, 2,1));
			//monsterTypes.Add(new MonsterTemplate(".,.^_^.,.", "Cat", ConsoleColor.Gray,1,3, 1, 2,1));
		}
	
	}

}
