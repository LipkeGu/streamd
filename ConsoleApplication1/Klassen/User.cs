using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace streamd
{
	public class User
	{
		string name;
		string password;
		byte level;

		public User(string name, string password, byte level)
		{
			this.name = name;
			this.password = password;
			this.level = level;
		}

		public string Username
		{
			get
			{
				return this.name;
			}
		}

		public string Password
		{
			get
			{
				return this.password;
			}
		}

		public int Level
		{
			get
			{
				return this.level;
			}
		}
	}
}
