using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Character : Unit
	{
		public int experience = 0;
		public int level = 1;
		public string classType;

		public Character(int hp, int attack, int defense, string image, string name, int speed, int talent, string classType)
			: base(hp, attack, defense, image, name, speed, talent)
		{
			this.classType = classType;
		}
	}
}
