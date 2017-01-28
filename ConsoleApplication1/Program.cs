using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace streamd
{
	class Program
	{
		static void Main(string[] args)
		{
			Filesystem.ReadUserList(args[1], true);
			Filesystem.WriteUserList(args[2], Encoding.ASCII);
		}
	}
}
