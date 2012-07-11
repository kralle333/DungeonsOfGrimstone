using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Ranger:Character
	{
		public Ranger(string image, string name):base(13, 5, 4, image, name, 10, 5)
		{
			classType = "Ranger";
			growthRate = new CharacterGrowthRate(0.1f, 0.15f, 0.4f, 0.2f, 0.4f);
			skillTree.FinalizeTree(this);
		}
	}
}
