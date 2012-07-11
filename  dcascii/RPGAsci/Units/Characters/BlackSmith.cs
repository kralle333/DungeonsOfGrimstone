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
			classType = "BlackSmith";
			growthRate = new CharacterGrowthRate(0.3f, 0.3f, 0.2f, 0.1f, 0.1f);
			skillTree.AddNode(CharacterManager.GetSkill("Mighty Blow"),"MASSIVE DAMAGE, MASSIVE MISS CHANCE");
			skillTree.AddNode(CharacterManager.GetSkill("Forgery"),"Lets you forge on equipment in town");
			skillTree.AddNode(CharacterManager.GetSkill("SpecialBooze"),"The blacksmith's favourite special brew, raises attack and miss chance");
			skillTree.AddNode(CharacterManager.GetSkill("Scavenger"),"Finds more material from dead foes");
			skillTree.FinalizeTree(this);
		}
	}
}
