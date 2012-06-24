using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RPGAsci
{
	class EquipmentManager
	{
		static private Dictionary<string, Equipment> equipment = new Dictionary<string, Equipment>();

		static public void Init()
		{
			//Armor
			equipment.Add("Common Shirt", new Equipment("Common shirt","Not uncomfortable, not comfortable",0,0,0,0,0));
			equipment.Add("Common Pants", new Equipment("Common pants","Doesn't do much, but they cover your legs",0, 0, 0, 0, 0));
			equipment.Add("Common Cap", new Equipment("Common cap", "Does not protect you, although maybe from the sun", 0, 0, 0, 0, 0));
			equipment.Add("Leather Cap", new Equipment("Leather cap", "Protective cap", 0, 0, 1, 0, 0));
			equipment.Add("Leather Vest", new Equipment("Leather Vest", "Protective Vest", 0, 0, 2, 0, 0));
			equipment.Add("Leather Pants", new Equipment("Leather Greaves", "Protective greaves", 0, 0, 2, 0, 0));
			equipment.Add("Copper Ring", new Equipment("Copper ring", "Ring made of copper", 3, 0, 0, 0, 0));

			//Weapons

			//Fighter
			equipment.Add("Wooden Sword", new Equipment("Wooden Sword", "The sword you used when you were a kid",2));
			
			//Wizard
			equipment.Add("Wooden Staff", new Equipment("Wooden Staff", "Basically a stick, but you keep telling yourself it's a staff", 1));
			
			//Ranger
			equipment.Add("Wooden Bow", new Equipment("Wooden Bow", "Bow made of wood, not able to shoot arrows far", 2));
		}
		static public Equipment GetEquipment(string name)
		{
			Regex r = new Regex("[0-9]");
			Match match = r.Match(name);
			if (match.Success)
			{
				return equipment[name.Remove(0,3)];
			}
			else
			{
				return equipment[name];
			}
		}
	}
}
