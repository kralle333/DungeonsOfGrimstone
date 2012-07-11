using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	public class Skill
	{
		public bool offensive;
		public string target;
		public string name;
		public float hitRatio = 0.9f;
		public Effect effect = null;
		public bool passive = false;
		private Random random = new Random();
		public Dictionary<string, int> requiredSkills = new Dictionary<string, int>();
		public SkillGrowthRate growthRate;
		public struct SkillGrowthRate
		{
			public int numberOfSkills;
			public float hp;
			public float damage;
			public float attack;
			public float defense;
			public float speed;
			public float talent;
			public float duration;
			public float hitRatio;

			public SkillGrowthRate(float hp, float damage, float attack, float defense, float speed, float talent, float duration,float hitRatio, int numberOfSkills)
			{
				this.hp = hp;
				this.damage = damage;
				this.attack = attack;
				this.defense = defense;
				this.speed = speed;
				this.talent = talent;
				this.duration = duration;
				this.hitRatio = hitRatio;
				this.numberOfSkills = numberOfSkills;
				
			}
		}
		public int level = 1;

		public Skill(string name)
		{
			this.passive = true;
			this.name = name;
		}
		public Skill(bool offensive,string target ,string name, Effect effect,float hitRatio,SkillGrowthRate growthRate)
		{
			this.hitRatio = hitRatio;
			this.offensive = offensive;
			this.target = target;
			this.name = name;
			this.effect = effect;
			this.growthRate = growthRate;
		}

		public void Use(Unit user,Unit target)
		{
			if (hitRatio == 1f)
			{
				ConsoleHelper.GameWriteLine(user.name + " used " + name + " on " + target.name);
				effect.Apply(target);
			}
			else if (random.NextDouble() > hitRatio)
			{
				ConsoleHelper.GameWriteLine(user.name + " used " + name + " on " + target.name);
				effect.Apply(target);
			}
			else
			{
				ConsoleHelper.GameWriteLine(user.name + " failed using " + name+"...");
			}
		}
		public void LevelUp()
		{
			effect.hp = (int)(effect.hp * growthRate.hp);
			effect.attack = (int)(effect.attack * growthRate.attack);
			effect.damage = (int)(effect.damage * growthRate.damage);
			effect.defense = (int)(effect.defense * growthRate.defense);
			effect.duration = (int)(effect.duration * growthRate.duration);
			effect.speed = (int)(effect.speed * growthRate.speed);
			effect.talent = (int)(effect.talent * growthRate.talent);
			hitRatio = hitRatio*growthRate.hitRatio;
			level++;
		}
	}
}
