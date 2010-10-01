using System.Diagnostics;
using System;
using Microsoft.Win32;
namespace BrowserOverride {
	class Program {
		static void Main(string[] args) {
			var verboseMode = false;

			if (args[0] == "-i") {
				if (args.Length < 2) {
					throw new ArgumentException("Path to default browser is required");
				}
				Properties.Settings.Default.BrowserPath = args[1];
				Properties.Settings.Default.Save();
				Register();
			}

			if (args[0] == "-u") {
				Unregister();
			}

			if (args[0] == "-b")
			{
				Console.WriteLine(Properties.Settings.Default.BrowserPath);
				return;
			}

			if (args[0] == "-p")
			{
				//list plugins
				var urlWrangler = new UrlWrangler(true);
				Console.WriteLine();
				Console.WriteLine("Available Plugins:");
				foreach (var plugin in urlWrangler.Plugins)
				{
					Console.WriteLine(plugin.Print());
				}
				return;
			}

			if (args.Length > 1 && args[1] == "-v")
			{
				verboseMode = true;
			}

			if (String.IsNullOrEmpty(Properties.Settings.Default.BrowserPath)) {
				throw new ArgumentException("Application must first be run with -i argument");
			}


			var wrangler = new UrlWrangler(verboseMode);
			if (verboseMode)
			{
				Console.WriteLine("Available Plugins:");
				foreach (var plugin in wrangler.Plugins) {
					Console.WriteLine(plugin.Print());
				}
			}
			var url = wrangler.RewriteUrl(args[0]);
			Process.Start(Properties.Settings.Default.BrowserPath, url);
		}

		static void Register() {

			try {
				Registry.LocalMachine.CreateSubKey("SOFTWARE\\RegisteredApplications").SetValue("Fixly", "Software\\Fixly\\Capabilities", RegistryValueKind.String);

				Registry.LocalMachine.CreateSubKey("SOFTWARE\\Fixly\\Capabilities").SetValue("ApplicationName", "Fixly", RegistryValueKind.String);
				Registry.LocalMachine.CreateSubKey("SOFTWARE\\Fixly\\Capabilities").SetValue("ApplicationIcon", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace(@"file:\", "") + "\\BrowserOverride.exe,0", RegistryValueKind.String);
				Registry.LocalMachine.CreateSubKey("SOFTWARE\\Fixly\\Capabilities").SetValue("ApplicationDescription", "Small app that allows for URL rewriting before passing to the browser", RegistryValueKind.String);

				Registry.LocalMachine.CreateSubKey("SOFTWARE\\Fixly\\Capabilities\\FileAssociations").SetValue(".xhtml", "FixlyHTML", RegistryValueKind.String);
				Registry.LocalMachine.CreateSubKey("SOFTWARE\\Fixly\\Capabilities\\FileAssociations").SetValue(".xht", "FixlyHTML", RegistryValueKind.String);
				Registry.LocalMachine.CreateSubKey("SOFTWARE\\Fixly\\Capabilities\\FileAssociations").SetValue(".shtml", "FixlyHTML", RegistryValueKind.String);
				Registry.LocalMachine.CreateSubKey("SOFTWARE\\Fixly\\Capabilities\\FileAssociations").SetValue(".html", "FixlyHTML", RegistryValueKind.String);
				Registry.LocalMachine.CreateSubKey("SOFTWARE\\Fixly\\Capabilities\\FileAssociations").SetValue(".htm", "FixlyHTML", RegistryValueKind.String);

				Registry.LocalMachine.CreateSubKey("SOFTWARE\\Fixly\\Capabilities\\StartMenu").SetValue("StartMenuInternet", "Fixly.exe", RegistryValueKind.String);

				Registry.LocalMachine.CreateSubKey("SOFTWARE\\Fixly\\Capabilities\\URLAssociations").SetValue("https", "FixlyHTML", RegistryValueKind.String);
				Registry.LocalMachine.CreateSubKey("SOFTWARE\\Fixly\\Capabilities\\URLAssociations").SetValue("http", "FixlyHTML", RegistryValueKind.String);
				Registry.LocalMachine.CreateSubKey("SOFTWARE\\Fixly\\Capabilities\\URLAssociations").SetValue("ftp", "FixlyHTML", RegistryValueKind.String);

				Registry.ClassesRoot.CreateSubKey("FixlyHTML").SetValue("", "Fixly HTML", RegistryValueKind.String);
				Registry.ClassesRoot.CreateSubKey("FixlyHTML").SetValue("URL Protocol", "", RegistryValueKind.String);

				Registry.ClassesRoot.CreateSubKey("FixlyHTML\\DefaultIcon").SetValue("", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace(@"file:\", "") + "\\BrowserOverride.exe,0", RegistryValueKind.String);

				Registry.ClassesRoot.CreateSubKey("FixlyHTML\\shell\\open\\command").SetValue("", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace(@"file:\", "") + "\\BrowserOverride.exe %1", RegistryValueKind.String);

				Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\http\\UserChoice").SetValue("Progid", "FixlyHTML", RegistryValueKind.String);
				Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\https\\UserChoice").SetValue("Progid", "FixlyHTML", RegistryValueKind.String);
				Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\ftp\\UserChoice").SetValue("Progid", "FixlyHTML", RegistryValueKind.String);

				try {
					Registry.CurrentUser.DeleteSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.htm\\UserChoice", false);
					Registry.CurrentUser.DeleteSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.html\\UserChoice", false);
					Registry.CurrentUser.DeleteSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.shtml\\UserChoice", false);
					Registry.CurrentUser.DeleteSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.xht\\UserChoice", false);
					Registry.CurrentUser.DeleteSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.xhtml\\UserChoice", false);

					Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.htm\\UserChoice").SetValue("Progid", "FixlyHTML", RegistryValueKind.String);
					Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.html\\UserChoice").SetValue("Progid", "FixlyHTML", RegistryValueKind.String);
					Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.shtml\\UserChoice").SetValue("Progid", "FixlyHTML", RegistryValueKind.String);
					Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.xht\\UserChoice").SetValue("Progid", "FixlyHTML", RegistryValueKind.String);
					Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.xhtml\\UserChoice").SetValue("Progid", "FixlyHTML", RegistryValueKind.String);
				} catch (Exception ex) {
					Console.WriteLine("An error may have occured registering the file extensions. You may want to check in the 'Default Programs' option in your start menu to confirm this worked." + ex.Message);
					throw;
				}

			} catch (Exception ex) {
				Console.WriteLine("Problem writing or reading Registry: " + ex.Message);
				throw;
			}
			Environment.Exit(0);
		}

		static void Unregister() {
			try {
				Registry.LocalMachine.DeleteSubKeyTree("SOFTWARE\\RegisteredApplications\\Fixly");
				Registry.LocalMachine.DeleteSubKeyTree("SOFTWARE\\Fixly");
			} catch (Exception ex) {
				Console.WriteLine("Problem writing or reading Registry: " + ex.Message);
				throw;
			}
			Environment.Exit(0);
		}
	}

}
