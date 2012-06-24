using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class Party
	{
		public int power = 0;
		public string name = "";
		public List<Character> characters = new List<Character>();
		public Dictionary<Item, int> items = new Dictionary<Item, int>();
		public Dictionary<Equipment, int> equipment = new Dictionary<Equipment, int>();
		public void CalculateAndSavePartyStrength()
		{

		}

		public int GetPowerLevel()
		{
			foreach (Character character in characters)
			{
				power += character.GetPower();
			}
			return power;
		}
	}
}
