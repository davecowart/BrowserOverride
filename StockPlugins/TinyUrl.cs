using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace StockPlugins {
	[Export(typeof(IPlugin))]
	public class TinyUrl : IPlugin {
		#region IPlugin Members

		public IEnumerable<string> HandledDomains {
			get { return new string[] {"tinyurl.com"}; }
		}

		public string Rewrite(string url) {
			using (var wc = new WebClient()) {
				var stream = wc.OpenRead(new Uri(String.Format("http://pipes.yahoo.com/pipes/pipe.run?_id=xk9igG833BGaf_dgmLokhQ&_render=json&urlinput1={0}", url)));
				string json;
				using (var sr = new StreamReader(stream)) {
					json = sr.ReadToEnd();
				}
				JObject o = JObject.Parse(json);
				return String.Concat(o["value"]["items"][0]["title"].Value<String>(), o["value"]["items"][1]["title"].Value<String>());
			}
		}

		#endregion
	}
}
