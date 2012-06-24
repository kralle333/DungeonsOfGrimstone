using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	public class Unit
	{
		public string name;

		public int currentHp;
		public int hp;

		public int attack;
		public int defense;
		public int speed;
		public int talent;
		public string image;

		public int currentAttack;
		public int currentDefense;
		public int currentSpeed;
		public int currentTalent;

		public Effect currentEffect;
		protected Dictionary<string, Equipment> equipment = new Dictionary<string, Equipment>();
		public bool defending = false;
		public StatusEffect currentStatus = StatusEffect.None;
		public bool usedTurn = false;
		public int power = 0;
		public Dictionary<Skill, int> skills = new Dictionary<Skill, int>();
		public Dictionary<Skill, int> skillsLeft = new Dictionary<Skill, int>();
		public Unit(int hp, int attack, int defense, string image, string name, int speed,int talent)
		{
			this.hp = hp;
			this.attack = attack;
			this.defense = defense;
			this.image = image;
			this.name = name;
			this.speed = speed;
			this.talent = talent;
			currentHp = hp;
		}
		public void Attack(Unit unit)
		{
			int damage = Math.Abs(unit.defense - attack);
			if (unit.defending)
			{
				damage -= 2;
				damage = Math.Abs(damage);
			}
			foreach (KeyValuePair<string, Equipment> pair in equipment)
			{
				if (pair.Value != null)
				{
					damage -= pair.Value.effect.defense;
				}
			}
			damage = Math.Abs(damage);
			if (damage > 0)
			{
				unit.currentHp -= damage;
			}
			ConsoleHelper.GameWriteLine("- Dealt " + damage + " damage");
			if (unit.hp <= 0)
			{
				ConsoleHelper.GameWriteLine(unit.name + " died");
			}
			usedTurn = true;
		}
		public void GetDamage(int damage)
		{
			damage = Math.Abs(defense - damage);
			if (defending)
			{
				damage -= 2;
				damage = Math.Abs(damage);
			}
			if (damage > 0)
			{
				currentHp -= damage;
			}
			ConsoleHelper.GameWriteLine(name + " got " + damage + " damage", ConsoleColor.DarkRed);
			if (hp <= 0)
			{
				ConsoleHelper.GameWriteLine(name + " died",ConsoleColor.DarkGreen);
			}
		}
		public virtual int GetPower()
		{
			power = attack * defense * hp * speed*(skills.Count()+1);
			return power;
		}
	}
}
