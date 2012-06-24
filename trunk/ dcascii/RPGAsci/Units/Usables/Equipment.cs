using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	public class Equipment:Item
	{
		public Equipment(string name,string description,int damage)
		{
			effect.damage = damage;
		}
		public Equipment(string name, string description, int hp, int attack, int defense, int talent, int speed)
		{
			effect.hp = hp;
			effect.defense = defense;
			effect.attack = attack;
			effect.defense = defense;
			effect.talent = talent;
			effect.speed = speed;
		}
	}
}
