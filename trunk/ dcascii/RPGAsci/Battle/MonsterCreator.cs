using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

			public MonsterTemplate(string image, string name, ConsoleColor color, int hpMultiplier,int attackMultiplier, int defenseMultiplier, int speedMultiplier,int talentMultiplier)
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
		public Monster GetMonster(int level,int maxPower)
		{
			int monsterPower = maxPower;
			Monster monster;
			MonsterTemplate template;
			while (monsterPower >= maxPower)
			{
				template = monsterTypes[random.Next(monsterTypes.Count())];
				monster = new Monster(random.Next(10 * level, 20 * level) * template.hpMultiplier,
					random.Next(2 * level, 5 * level) * template.attackMultiplier,
					random.Next(2 * level, 4 * level) * template.defenseMultiplier,
					template.image,
					template.name,
					template.speedMultiplier * 4,template.talentMultiplier*3);
				monster.color = template.color;
				monsterPower = monster.GetPower();
				if (monsterPower < maxPower)
				{
					return monster;
				}
			}
			return null;
		}
		private void InitMonsterTypes()
		{
			monsterTypes.Add(new MonsterTemplate(">(o.o)<", "Bat", ConsoleColor.DarkGreen, 1,1, 1, 1,1));
			monsterTypes.Add(new MonsterTemplate(">*_*<", "Butterfly", ConsoleColor.Blue, 1,1, 1, 2,1));
			monsterTypes.Add(new MonsterTemplate("^(0v0)^", "Owl", ConsoleColor.Gray, 2,2, 1, 2,1));
			monsterTypes.Add(new MonsterTemplate(".,.^_^.,.", "Cat", ConsoleColor.Gray,1,3, 1, 2,1));
		}
	}

}
