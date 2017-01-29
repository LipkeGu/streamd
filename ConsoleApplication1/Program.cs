using System;
using System.Text;

namespace streamd
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 3)
			{
				Console.WriteLine("Usage: streamd.exe <userlist_file> <djlogins.conf> <music_root>{0}", Environment.NewLine);
				return;
			}

			if (!Filesystem.Exists(args[0], Filesystem.Type.File))
			{
				Console.WriteLine("File not Found: {0}!", args[0]);
				return;
			}

			if (!Filesystem.Exists(args[2], Filesystem.Type.Directory))
			{
				Console.WriteLine("File not Found: {0}!", args[2]);
				return;
			}

			Filesystem.GeneratePlaylist(args[2]);

			Filesystem.ReadUserList(args[0], true);
			Filesystem.WriteUserList(args[1], Encoding.ASCII);
		}
	}
}
