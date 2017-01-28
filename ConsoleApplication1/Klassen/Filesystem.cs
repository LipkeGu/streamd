using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace streamd
{
	public static class Filesystem
	{
		public static Dictionary<string, User> Users = new Dictionary<string, User>();

		public static int Execute(string filename, string args = "")
		{
			if (string.IsNullOrEmpty(filename))
				return 1;

			var retval = 0;

			using (var prc = new Process())
			{
				prc.StartInfo.FileName = filename;

				if (!string.IsNullOrEmpty(args))
					prc.StartInfo.Arguments = args;

				prc.Start();
				prc.WaitForExit();

				retval = prc.ExitCode;
			}

			return retval;
		}

		public static void ReadUserList(string filename, bool detect_encoding = true)
		{
			using (var sr = new StreamReader(filename, detect_encoding))
			{
				var line = string.Empty;
				var parts = line.Split(':');

				while (!sr.EndOfStream)
				{
					line = sr.ReadLine().Trim();
					if (!string.IsNullOrEmpty(line))
					{
						parts = line.Split(':');

						if (parts.Length > 2 && parts[0].Length > 3)
						{
							if (!Users.ContainsKey(parts[0].ToLowerInvariant()))
							{
								var user = new User(parts[0].ToLowerInvariant(), parts[1], byte.Parse(parts[2]));
								Console.WriteLine(string.Format("\tDJ\t{0}", user.Username));

								Users.Add(user.Username, user);
							}
						}
					}
				}
			}
		}

		public static void WriteUserList(string filename, Encoding encoding)
		{
			var i = 1;

			using (var sw = new StreamWriter(filename, false, encoding))
			{
				sw.AutoFlush = true;
				sw.NewLine = Environment.NewLine;

				foreach (var user in Users.Values)
				{
					if (user == null)
						continue;

					sw.WriteLine(string.Format("djlogin_{3}={0}\ndjpassword_{3}={1}\ndjpriority_{3}={2}\n\n",
						user.Username, user.Password, user.Level, i));

					i++;
				}
			}

			Console.WriteLine("{0} DJs written to: {1}", Users.Keys.Count, filename);
		}
	}
}
