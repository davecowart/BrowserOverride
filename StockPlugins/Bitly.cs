using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace StockPlugins {
	[Export(typeof(IPlugin))]
	public class Bitly : IPlugin {
		#region IPlugin Members

		public IEnumerable<string> HandledDomains {
			get { return new string[] {"bit.ly", "digs.by"}; }
		}

		public string Rewrite(string url) {
			var qsPosition = url.IndexOf('?');
			if (qsPosition > 0) {
				url = url.Remove(qsPosition);
			}
			using (var wc = new WebClient()) {
				var stream = wc.OpenRead(new Uri(String.Format("http://pipes.yahoo.com/pipes/pipe.run?ShortLink={0}&_id=b2d12662c616a850d70cb8b5fb9dc4b6&_render=json", url)));
				string json;
				using (var sr = new StreamReader(stream)) {
					json = sr.ReadToEnd();
				}
				JObject o = JObject.Parse(json);
				return o["value"]["items"][0]["data"]["expand"][0]["long_url"].Value<String>().Replace("&#92;", "");
			}
		}

		#endregion
	}
}
