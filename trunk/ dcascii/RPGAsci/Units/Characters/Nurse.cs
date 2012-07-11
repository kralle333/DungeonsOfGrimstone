using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Nurse:Character
	{
		public Nurse(string image, string name):base(15, 2, 2, image, name, 3, 7)
		{
			classType = "Nurse";
			growthRate = new CharacterGrowthRate(0.3f, 0.05f, 0.2f, 0.2f, 0.2f);
			skillTree.FinalizeTree(this);
		}
	}
}
