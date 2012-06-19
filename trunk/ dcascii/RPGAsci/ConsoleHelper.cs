using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGAsci
{
	class ConsoleHelper
	{
		static public string SimpleRead(string text, int lengthLimit)
		{
			Console.WriteLine(text + " - Allowed length :" + lengthLimit);
			while (true)
			{
				string read = Console.ReadLine();
				if (read.Count() > 0 && read.Count() < lengthLimit)
				{
					return read;
				}
			}
		}
		static public void WriteLine(string text, ConsoleColor color)
		{
			Console.BackgroundColor = color;
			Console.WriteLine(text);
			Console.ResetColor();
		}
		static public void Write(string text, ConsoleColor color)
		{
			Console.BackgroundColor = color;
			Console.Write(text);
			Console.ResetColor();
		}
	}
}
