using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Reflection;

namespace BrowserOverride {
	public class UrlWrangler {
		[ImportMany]
		public IEnumerable<IPlugin> Plugins { get; set; }

		private bool _verbose;

		public UrlWrangler(bool verbose) {
			_verbose = verbose;
			var catalogs = new List<ComposablePartCatalog> { new AssemblyCatalog(Assembly.GetExecutingAssembly()) };
			var path = Assembly.GetExecutingAssembly().GetName().CodeBase.Substring(8);
			path = path.Remove(path.LastIndexOf('/')).Replace("/", "\\") + "\\Extensions";
			if (_verbose) {
				Console.WriteLine("Extensions path: {0}", path);
			}
			if (Directory.Exists(path)) {
				catalogs.Add(new DirectoryCatalog("Extensions"));
			}

			var catalog = new AggregateCatalog(catalogs);
			var container = new CompositionContainer(catalog);
			container.ComposeParts(this);
		}

		public string RewriteUrl(string url) {
			var plugin = Plugins.FirstOrDefault(p => p.HandledDomains.Any(d => url.Contains(d)));
			if (plugin != null) {
				if (_verbose) {
					Console.WriteLine("Selected plugin: {0}", plugin.Print());
				}
				if (_verbose) {
					Console.WriteLine("Old URL: {0}", url);
				}
				url = plugin.Rewrite(url);
				if (_verbose) {
					Console.WriteLine("New URL: {0}", url);
				}
			} else {
				if (_verbose) {
					Console.WriteLine("No available plugin to handle this url");
				}
			}
			return url;
		}
	}
}
