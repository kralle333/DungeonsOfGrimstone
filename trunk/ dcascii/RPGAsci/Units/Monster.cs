using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Monster:Unit
	{
		public ConsoleColor color;
		public Monster(int hp, int attack, int defense, string image,string name, int speed,int talent):base(hp,attack,defense,image,name, speed,talent)
		{

		}
	}
}
