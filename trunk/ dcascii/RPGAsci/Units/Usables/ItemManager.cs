using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RPGAsci
{
	static class ItemManager
	{
		static private Dictionary<string, Item> items = new Dictionary<string, Item>();
		static private Random random = new Random();

		static public void Init()
		{
			items.Add("Herb", new Item("Herb", new Effect(10, false), "Single", false, 0, 0));
			items.Add("Potion", new Item("Potion", new Effect(30, false), "Single", false, 0, 0));
			items.Add("Bell", new Item("Bell", new Effect(0, 0, 0, 0, 0, 0, StatusEffect.CureSleep, false, 0),"Single",false,0,0));
			items.Add("FluffySheep", new Item("FluffySheep", new Effect(0, StatusEffect.Sleep), "Single", true, 0, 0));
		}
		static public Item GetItem(string name)
		{
			Regex r = new Regex("[0-9]");
			Match match = r.Match(name);
			if (match.Success)
			{
				return items[name.Remove(0, 3)];
			}
			else
			{
				return items[name];
			}
		}
		static public Item GetRandomItem(int level)
		{

			while (true)
			{
				string randomKey = items.Keys.ElementAt(random.Next(items.Keys.Count()));
				int power = items[randomKey].GetPower();
				if (power < level * 100)
				{
					return items[randomKey];
				}
			}
		}
	}
}
