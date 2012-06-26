using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	public class SkillNode
	{
		public List<SkillNode> required = new List<SkillNode>();
		Skill skill;
		int level = 0;
		bool available = false;

		public SkillNode(Skill skill)
		{
			this.skill = skill;
		}
	}
}
