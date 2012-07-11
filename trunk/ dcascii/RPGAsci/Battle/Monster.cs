using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Monster:Unit
	{
		public int imageWidth = 0;
		public int experience = 0;
		public Monster(int hp, int attack, int defense, string image,int imageWidth,string name, int speed,int talent,int experience):base(hp,attack,defense,image,name, speed,talent)
		{
			equipment["Weapon"] = null;
			equipment["Armor1"] = null;
			equipment["Armor2"] = null;
			equipment["Armor3"] = null;
			this.imageWidth = imageWidth;
			this.experience = experience;
		}
	}
}
