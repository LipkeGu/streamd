using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace streamd
{
	public class Playlist
	{
		string name;
		List<string> tracks;

		public Playlist(string name)
		{
			this.name = name;
			this.tracks = new List<string>();
		}

		public string Add
		{
			set
			{
				if (!this.tracks.Contains(value))
					this.tracks.Add(value);
			}
		}

		public List<string> Tracks
		{
			get
			{
				return this.tracks;
			}
		}
	}
}
