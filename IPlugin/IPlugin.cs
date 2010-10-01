using System.Collections.Generic;

public interface IPlugin {
	IEnumerable<string> HandledDomains { get; }
	string Rewrite(string url);
}
