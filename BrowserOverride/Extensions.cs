using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrowserOverride {
	public static class Extensions {
		public static string Print(this IPlugin plugin)
		{
			return String.Format("{0} - {1}", plugin, plugin.HandledDomains.JoinList());
		}

		public static string JoinList(this IEnumerable<string> strings) {
			var sb = new StringBuilder();
			strings.ToList().ForEach(s => sb.Append(s).Append(","));
			return sb.ToString().TrimEnd(",".ToCharArray());
		}
	}
}
