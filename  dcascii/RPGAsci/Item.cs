using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Item
	{
		public string name;
		public Effect effect;
		public string target;
		public int xPos;
		public int yPos;
		
		public Item(string name, Effect effect,string target,int xPos,int yPos)
		{
			this.name = name;
			this.effect = effect;
			this.target = target;
			this.xPos = xPos;
			this.yPos = yPos;
		}
		
		public void Use(Unit target)
		{
			effect.Apply(target)
		}
	}
}
