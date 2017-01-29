using System;
using System.Collections.Generic;
using System.IO;

namespace streamd
{
	public class Playlist
	{
		string name;
		string path;

		bool shuffle;
		bool loop;

		byte level;

		List<string> tracks;

		public Playlist(string name, string path, byte level = 1, bool loop = true, bool shuffle = true)
		{
			this.loop = loop;
			this.shuffle = shuffle;
			this.level = level;

			this.path = path;
			this.name = name;
			this.tracks = new List<string>();

			AddEntries();
		}

		internal void AddEntries()
		{
			var dirinfo = new DirectoryInfo(this.path);
			foreach (var file in dirinfo.GetFiles("*.mp3", SearchOption.AllDirectories))
			{
				if (file.Length < 1000 && !file.Exists)
					continue;

				this.Add = file.FullName;
			}
		}

		public string Add
		{
			set
			{
				if (!this.tracks.Contains(value))
				{
					var parts = value.Split('.');

					if (parts.Length > 1)
						this.tracks.Add(value);
				}
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public byte Level
		{
			get
			{
				return this.level;
			}
		}

		public List<string> Tracks
		{
			get
			{
				return this.tracks;
			}
		}

		public bool Loop
		{
			get
			{
				return this.loop;
			}
		}

		public bool Shuffle
		{
			get
			{
				return this.shuffle;
			}
		}

		public int Count
		{
			get
			{
				return this.tracks.Count;
			}
		}
	}
}
