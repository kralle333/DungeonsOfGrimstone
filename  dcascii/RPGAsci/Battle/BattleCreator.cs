using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class BattleCreator
	{
		Random random = new Random();
		MonsterCreator mc = new MonsterCreator();
		List<Monster> changeNames = new List<Monster>();
		string[] suffixes = new string[] { "A", "B", "C" };
		Dictionary<string, int> indexes = new Dictionary<string, int>();
		public Battle CreateBattle(Party party,int level)
		{
			List<Monster> monsters = new List<Monster>();
			int monsterPackPower = 0;
			Monster currentMonster;
			int numberOfMonster = random.Next(1, 4);
			indexes.Clear();
			while (numberOfMonster > 0 && monsters.Count()<3)
			{
				currentMonster = mc.GetMonster(level);
				if (!indexes.ContainsKey(currentMonster.name))
				{
					indexes[currentMonster.name] = 0;
				}
				Monster containMonster = monsters.Find(x => x.name == currentMonster.name);
				if (containMonster != null)
				{
					indexes[containMonster.name]++;
					currentMonster.name += "("+suffixes[indexes[containMonster.name]]+")";
					if(!changeNames.Contains(containMonster))
					{
						changeNames.Add(containMonster);
					}
				}
				monsters.Add(currentMonster);
				monsterPackPower += currentMonster.GetPower();
				numberOfMonster--;
			}
			foreach (Monster c in changeNames)
			{
				c.name += "(A)";
			}
			
			return new Battle(monsters, party);
		}
	}
}
