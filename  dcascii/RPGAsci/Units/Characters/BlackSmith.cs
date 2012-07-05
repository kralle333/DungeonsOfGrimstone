using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class BlackSmith : Character
	{
		public BlackSmith(string image, string name)
			: base(20, 3, 3, image, name, 1, 2)
		{
			growthRate = new LevelUpGrowthRate(0.3f, 0.3f, 0.2f, 0.1f, 0.1f);
			skillTree.AddNode(CharacterManager.GetSkill("Powerful"));
			skillTree.AddNode(CharacterManager.GetSkill("Forgery"));
			skillTree.AddNode(CharacterManager.GetSkill("SpecialBooze"));
			skillTree.AddNode(CharacterManager.GetSkill("Scavenger"));
		}
	}
}
