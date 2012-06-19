using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	public enum StatusEffect
	{
		Confusion,
		Sleep,
		None
	}
	public class Effect
	{
		
		int hp;
		int damage;
		int attack;
		int defense;
		int speed;
		int talent;
		int duration;
		StatusEffect statusEffect;
		bool sleep = false;
		bool perm = false;

		public Effect(int hp, int damage, int attack, int defense, int speed, int talent, StatusEffect statusEffect, bool perm, int duration)
		{
			this.hp = hp;
			this.damage = damage;
			this.attack = attack;
			this.defense = defense;
			this.speed = speed;
			this.talent = talent;
			this.statusEffect = statusEffect;
			this.perm = perm;
			this.duration = duration;
		}
		public Effect(int hp,bool perm)
		{
			this.hp = hp;
			this.perm = perm;
		}
		public Effect(int damage)
		{
			this.damage = damage;
		}

		public void Apply(Unit unit)
		{
			if (perm)
			{
				unit.hp += hp;
				unit.attack += attack;
				unit.defense+= defense;
				unit.speed += speed;
				unit.talent += talent;
			}
			else
			{
				if (hp > 0)
				{
					unit.currentHp += hp;
					Console.WriteLine(unit.name + " was healed " + hp);
				}
				if (attack > 0)
				{
					unit.currentAttack += attack;
					Console.WriteLine(unit.name + "'s attack was raised by " + attack);
				}
				if (defense > 0)
				{
					unit.currentDefense += defense;
					Console.WriteLine(unit.name + "'s defense was raised by " + defense);
				}
				if (speed > 0)
				{
					unit.currentSpeed += speed;
					Console.WriteLine(unit.name + "'s speed was raised by " + speed);
				}
				if (talent > 0)
				{
					unit.currentTalent += talent;
					Console.WriteLine(unit.name + "'s talent was raised by " + talent);
				}
			}
			unit.GetDamage(damage);

			if (statusEffect != unit.currentStatus)
			{
				if (statusEffect == StatusEffect.None)
				{
					if (unit.currentStatus == StatusEffect.Confusion)
					{
						Console.WriteLine(unit.name + " got out of confusion");
					}
					else if (unit.currentStatus == StatusEffect.Sleep)
					{
						Console.WriteLine(unit.name + " woke up");
					}
					
				}
				else if (statusEffect == StatusEffect.Confusion)
				{
					Console.WriteLine(unit.name + " got confused");
				}
				else if (statusEffect == StatusEffect.Sleep)
				{
					Console.WriteLine(unit.name + " fell asleep");
				}
				unit.currentStatus = statusEffect;
			}
			if (duration > 0)
			{
				unit.currentEffect = this;
			}
		}
		public void Apply(List<Unit> units)
		{
			foreach (Unit unit in units)
			{
				Apply(unit);
			}
		}
	}
}
