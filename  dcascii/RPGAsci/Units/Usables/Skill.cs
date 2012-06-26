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
		private Random random;
		public Dictionary<string, int> requiredSkills = new Dictionary<string, int>();

		public Skill(string name)
		{
			this.passive = true;
			this.name = name;
		}
		public Skill(bool offensive,string target ,string name, Effect effect,float hitRatio)
		{
			this.hitRatio = hitRatio;
			this.offensive = offensive;
			this.target = target;
			this.name = name;
			this.effect = effect;
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
	}
}
