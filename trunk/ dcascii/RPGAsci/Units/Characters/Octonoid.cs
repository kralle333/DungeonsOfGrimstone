using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Octonoid:Character
	{
		public Octonoid(string image, string name):base(16, 4, 8, image, name, 1, 1)
		{
			classType = "Octonoid";
			growthRate = new CharacterGrowthRate(0.3f, 0.2f, 0.2f, 0.1f, 0.2f);
			skillTree.FinalizeTree(this);
		}
	}
}
