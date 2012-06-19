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
		public Effect effect = new Effect(0);
		public bool passive;

		public Skill(bool offensive,string target ,string name, Effect effect)
		{
			this.offensive = offensive;
			this.target = target;
			this.name = name;
			this.effect = effect;
		}
		public void Use(Unit user,Unit target)
		{
			Console.WriteLine(user.name + " used " + name + " on " + target.name);
			effect.Apply(target);
		}
	}
}
