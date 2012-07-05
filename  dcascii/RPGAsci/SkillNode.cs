using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	public class SkillNode
	{
		public Dictionary<Skill,int> requiredSkills = new Dictionary<Skill,int>();
		public List<SkillNode> children = new List<SkillNode>();
		public Skill skill;
		int level = 0;
		bool available = false;

		public SkillNode(Skill skill)
		{
			this.skill = skill;
		}
	}
}
