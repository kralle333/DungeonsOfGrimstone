using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class HeroStats
	{
		Party party;

		public void UpdateInfo(Party party)
		{
			this.party = party;
		}
		public void Draw()
		{
			for (int i = 0; i < party.characters.Count(); i++)
			{
				Console.SetCursorPosition(80, 10 * i+i*2);
				Console.WriteLine("********************");
				for (int j = 1; j < 10; j++)
				{
					Console.SetCursorPosition(80, j + 10 * i + i*2);
					switch (j)
					{
						case 0: Console.WriteLine("*Level: " + party.characters[i].level.ToString().PadRight(11, ' ') + "*"); break;
						case 2: Console.WriteLine("*       " + party.characters[i].name.PadRight(11, ' ') + "*"); break;
						case 4: Console.WriteLine("*       " + party.characters[i].image.PadRight(11, ' ') + "*"); break;
						case 6: Console.WriteLine("* HP:      " + party.characters[i].hp.ToString().PadRight(8, ' ') + "*"); break;
						case 7: Console.WriteLine("* Attack:  " + party.characters[i].attack.ToString().PadRight(8, ' ') + "*"); break;
						case 8: Console.WriteLine("* Defense: " + party.characters[i].defense.ToString().PadRight(8, ' ') + "*"); break;
						case 9: Console.WriteLine("* Speed:   " + party.characters[i].speed.ToString().PadRight(8, ' ') + "*"); break;
						default: Console.WriteLine("*                  *");break;
					}
					
				}
				Console.SetCursorPosition(80, 10 + 10 * i + i*2);
				Console.WriteLine("********************");
			}
			
		}
	}
}
