using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	public class SkillNode:MenuItem
	{
		public Dictionary<Skill, int> requiredSkills = new Dictionary<Skill, int>();
		public List<SkillNode> canOpen = new List<SkillNode>();
		public Skill skill;
		public int level = 0;
		public bool available = false;

		public SkillNode(Skill skill,string description):base(skill.name+" 0",description)
		{
			this.skill = skill;
			locked = true;
		}
	}
}
