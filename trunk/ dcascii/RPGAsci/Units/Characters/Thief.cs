using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Thief:Character
	{
		public Thief(string image, string name):base(11, 3, 3, image, name, 6, 10)
		{
			growthRate = new LevelUpGrowthRate(0.2f, 0.2f, 0.1f, 0.4f, 0.3f);
		}
	}
}
