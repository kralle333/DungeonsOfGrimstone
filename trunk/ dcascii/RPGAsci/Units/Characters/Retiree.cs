using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Retiree:Character
	{
		public Retiree(string image, string name)
			: base(12, 8, 5, image, name, 1, 1)
		{
			classType = "Retiree";
			growthRate = new CharacterGrowthRate(0.25f, 0.5f, 0.1f, 0.1f, 0.15f);
			skillTree.FinalizeTree(this);
		}
	}
}
