using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	public class Item
	{
		public string name;
		public Effect effect;
		public string target;
		public bool offensive;
		public int xPos;
		public int yPos;

		public Item(string name, Effect effect, string target, bool offensive, int xPos, int yPos)
		{
			this.name = name;
			this.effect = effect;
			this.target = target;
			this.offensive = offensive;
			this.xPos = xPos;
			this.yPos = yPos;
		}
		public Item()
		{
			effect = new Effect(0);
		}
		public void Use(Unit target)
		{
			effect.Apply(target);
		}
		public int GetPower()
		{
			int extra = 0;
			if (effect.statusEffect == StatusEffect.CureAll)
			{
				extra += 500;
			}
			else if (effect.statusEffect != StatusEffect.None)
			{
				extra += 100;
			}
			if (effect.perm)
			{
				extra += 1000;
			}
			return extra+(effect.attack + 1) * (effect.damage + 1) * (effect.defense + 1) * (effect.duration + 1) * (effect.hp + 1) * (effect.speed + 1) * (effect.talent + 1);
		}
	}
}
