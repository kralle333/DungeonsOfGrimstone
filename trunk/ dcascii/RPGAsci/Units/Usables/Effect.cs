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
		Poison,
		CurePoison,
		CureConfusion,
		CureSleep,
		CureAll,
		None
	}
	public class Effect
	{

		public int hp = 0;
		public int damage = 0;
		public int attack = 0;
		public int defense = 0;
		public int speed = 0;
		public int talent = 0;
		public int duration = 0;
		public StatusEffect statusEffect = StatusEffect.None;
		public bool perm = false;

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
		public Effect(StatusEffect statusEffect,int damage)
		{
			this.damage = damage;
			this.statusEffect = statusEffect;
		}
		public Effect(int hp, StatusEffect statusEffect)
		{
			this.hp = hp;
			this.statusEffect = statusEffect;
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
					if (unit.currentHp + hp > unit.hp)
					{
						hp = unit.hp - unit.currentHp;
					}
					unit.currentHp += hp;
					ConsoleHelper.GameWriteLine(unit.name + " was healed " + hp + " hitpoints", ConsoleColor.DarkGreen);
				}
				if (attack > 0)
				{
					unit.currentAttack += attack;
					ConsoleHelper.GameWriteLine(unit.name + "'s attack was raised by " + attack, ConsoleColor.DarkGreen);
				}
				if (defense > 0)
				{
					unit.currentDefense += defense;
					ConsoleHelper.WriteLine(unit.name + "'s defense was raised by " + defense, ConsoleColor.DarkGreen);
				}
				if (speed > 0)
				{
					unit.currentSpeed += speed;
					ConsoleHelper.GameWriteLine(unit.name + "'s speed was raised by " + speed, ConsoleColor.DarkGreen);
				}
				if (talent > 0)
				{
					unit.currentTalent += talent;
					ConsoleHelper.GameWriteLine(unit.name + "'s talent was raised by " + talent, ConsoleColor.DarkGreen);
				}
			}
			if (damage > 0)
			{
				unit.GetDamage(damage);
			}
			if (statusEffect == StatusEffect.CureAll ||
				statusEffect == StatusEffect.CureConfusion && unit.currentStatus == StatusEffect.Confusion ||
				statusEffect == StatusEffect.CureSleep && unit.currentStatus == StatusEffect.Sleep)
			{
				if (unit.currentStatus == StatusEffect.Confusion)
				{
					ConsoleHelper.GameWriteLine(unit.name + " got out of confusion");
				}
				else if (unit.currentStatus == StatusEffect.Sleep)
				{
					ConsoleHelper.GameWriteLine(unit.name + " woke up");
				}
				unit.currentStatus = StatusEffect.None;
			}
			else if (statusEffect != unit.currentStatus)
			{
				if (statusEffect == StatusEffect.Confusion)
				{
					ConsoleHelper.GameWriteLine(unit.name + " got confused");
				}
				else if (statusEffect == StatusEffect.Sleep)
				{
					ConsoleHelper.GameWriteLine(unit.name + " fell asleep");
				}
				unit.currentStatus = statusEffect;
			}
			if (duration > 0)
			{
				unit.currentEffect = this;
			}
			ConsoleHelper.GameWriteLine();
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
