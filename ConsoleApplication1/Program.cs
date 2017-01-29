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

			if (Filesystem.Exists(args[0], Filesystem.Type.File) &&
				Filesystem.Exists(args[2], Filesystem.Type.Directory))
			{
				Filesystem.GeneratePlaylist(args[2]);
				Filesystem.WritePlaylist("");

				Filesystem.ReadUserList(args[0], true);
				Filesystem.WriteUserList(args[1], Encoding.ASCII);
			}
		}
	}
}
