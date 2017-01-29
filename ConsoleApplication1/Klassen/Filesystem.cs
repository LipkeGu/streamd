using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace streamd
{
	public static class Filesystem
	{
		public enum Type
		{
			Directory,
			File
		}

		public static Dictionary<string, User> Users = new Dictionary<string, User>();
		public static Dictionary<string, Playlist> Playlists = new Dictionary<string, Playlist>();

		public static bool Exists(string path, Type t = Type.File)
		{
			switch (t)
			{
				case Type.Directory:
					return Directory.Exists(path);

				case Type.File:
					return File.Exists(path);
				default:
					return false;
			}
		}

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

		public static void GeneratePlaylist(string path, string name = "main")
		{
			var playlist = new Playlist(name, path);

			if (!Playlists.ContainsKey(name))
				Playlists.Add(name, playlist);
		}

		public static void WritePlaylist(string path = "")
		{
			if (Playlists.Keys.Count != 0)
			{
				foreach (var playlist in Playlists.Values)
				{
					if (playlist.Tracks.Count > 0)
					{
						using (var sw = new StreamWriter(Path.Combine(path, string.Format("{0}.lst", playlist.Name))))
						{
							Console.WriteLine("\tPLAYLIST\t{0}", playlist.Name);

							sw.AutoFlush = true;
							sw.NewLine = Environment.NewLine;

							foreach (var entry in playlist.Tracks)
							{
								if (!string.IsNullOrEmpty(entry) && !string.IsNullOrWhiteSpace(entry))
								{
									sw.WriteLine(entry);
									Console.WriteLine("\tMP3\t{0}", entry);
								}
							}

							Console.WriteLine("");
						}
					}
					else
						Console.WriteLine("WARN: Skipping empty Playlist \"{0}\"", playlist.Name);
				}
			}
		}

		public static void ReadUserList(string filename, bool detect_encoding = true)
		{
			if (string.IsNullOrEmpty(filename))
				return;

			using (var sr = new StreamReader(filename, detect_encoding))
			{
				var line = string.Empty;
				var parts = line.Split(':');

				while (!sr.EndOfStream)
				{
					line = sr.ReadLine().Trim();

					if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
						continue;

					parts = line.Split(':');

					if (parts.Length > 2 && !Users.ContainsKey(parts[0].ToLowerInvariant()))
					{
						var user = new User(parts[0].ToLowerInvariant(), parts[1], byte.Parse(parts[2]));
						Console.WriteLine(string.Format("\tDJ\t{0}", user.Username));

						Users.Add(user.Username, user);
					}
				}
			}

			try
			{
				File.Delete(filename);
			}
			catch (Exception e)
			{
				Console.WriteLine("ERROR: Cant delete file \"{0}\": {1}", filename, e);
			}
			
		}

		public static void WriteUserList(string filename, Encoding encoding, int pwlen = 8)
		{
			if (Users.Count == 0)
				return;

			using (var sw = new StreamWriter(filename, false, encoding))
			{
				sw.AutoFlush = true;
				sw.NewLine = Environment.NewLine;

				var i = 1U;

				foreach (var user in Users.Values)
				{
					if (user == null)
						continue;

					if (pwlen == 1)
						pwlen = 8;

					if (user.Password.Length <= (pwlen - 1))
						Console.WriteLine("WARN: Password for {0} is LT (lesser than) {1} Characters! (Length: {2})",
							user.Username, pwlen, user.Password.Length);

					if (user.Level < 1)
						Console.WriteLine("WARN: Priority for {0} is LT (lesser than) 1! (Priority: {1})",
							user.Username, user.Level);

					if (user.Level > 9)
						Console.WriteLine("WARN: Priority for {0} is GT (greater than) 9! (Priority: {1})",
							user.Username, user.Level);

					sw.WriteLine(string.Format("djlogin_{3}={0}{4}djpassword_{3}={1}{4}djpriority_{3}={2}{4}",
						user.Username, user.Password, user.Level, i, Environment.NewLine));

					i++;
				}

				Console.WriteLine("{0} DJs written to: {1}", (i - 1), filename);
				Console.WriteLine();

				WriteCalender("calendar.xml");
			}
		}

		public static void WriteCalender(string filename)
		{
			Console.WriteLine(string.Format("Generating calendar..."));
			Console.WriteLine();

			var xw_settings = new XmlWriterSettings();
			xw_settings.Encoding = Encoding.UTF8;
			xw_settings.Indent = true;
			xw_settings.NewLineChars = Environment.NewLine;

			var xw = XmlWriter.Create(filename, xw_settings);

			xw.WriteStartDocument();
			xw.WriteStartElement("eventlist");

			if (Playlists.Keys.Count > 0)
			{
				foreach (var playlist in Playlists.Values)
				{
					if (playlist.Count == 0)
						continue;

					if (playlist.Level < 1)
						Console.WriteLine("WARN: Priority for playlist {0} is LT (lesser than) 1! (Priority: {1})",
							playlist.Name, playlist.Level);

					if (playlist.Level > 1)
						Console.WriteLine("WARN: Priority for playlist {0} is GT (greater than) 9! (Priority: {1})",
							playlist.Name, playlist.Level);

					xw.WriteStartElement("event");
					xw.WriteAttributeString("type", "playlist");

					xw.WriteStartElement("playlist");
					xw.WriteAttributeString("loopatend", playlist.Loop ? "1" : "0");
					xw.WriteAttributeString("shuffle", playlist.Shuffle ? "1" : "0");

					xw.WriteAttributeString("priority", string.Format("{0}", playlist.Level));
					xw.WriteValue(playlist.Name);
					xw.WriteEndElement(); // Playlist

					xw.WriteStartElement("calender");
					xw.WriteAttributeString("starttime", "00:00:00");
					xw.WriteEndElement(); // Calender
					xw.WriteEndElement(); // Event

					Console.WriteLine(string.Format("\tEVENT<PLAYLIST>\t{0}", playlist.Name));
				}

				Console.WriteLine("{0} Playlists written to: {1}", Playlists.Keys.Count, filename);
			}
			else
				Console.WriteLine("Nothing to do for rule \"Playlists\"...");

			if (Users.Keys.Count > 0)
			{
				foreach (var user in Users.Values)
				{
					xw.WriteStartElement("event");
					xw.WriteAttributeString("type", "dj");

					xw.WriteStartElement("dj");
					xw.WriteAttributeString("archive", "0");
					xw.WriteAttributeString("priority", string.Format("{0}", user.Level));
					xw.WriteValue(user.Username);
					xw.WriteEndElement(); // Dj

					xw.WriteStartElement("calender");
					xw.WriteAttributeString("starttime", "00:00:00");
					xw.WriteEndElement(); // Calender
					xw.WriteEndElement(); // Event

					Console.WriteLine(string.Format("\tEVENT<DJ>\t{0}", user.Username));
				}

				Console.WriteLine("{0} DJ Events written to: {1}", Users.Keys.Count, filename);
			}
			else
				Console.WriteLine("Nothing to do for rule \"Users\"...");

			xw.WriteEndElement();
			xw.WriteEndDocument();

			xw.Close();
		}
	}
}
