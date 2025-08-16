using Microsoft.Win32;

using System.Diagnostics;

namespace Viewer
{
	public class SettingsHelper
	{
		private readonly string publisher;
		private readonly string appName;
		private readonly string appVersion;

		public SettingsHelper(string publisher, string appName, string appVersion)
		{
			this.publisher = publisher;
			this.appName = appName;
			this.appVersion = Debugger.IsAttached ? appVersion + "_debug" : appVersion;
		}

		public RegistryKey GetUserRegistryKey()
		{
			RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);

			key.CreateSubKey(publisher);
			key = key.OpenSubKey(publisher, true);

			key.CreateSubKey(appName);
			key = key.OpenSubKey(appName, true);

			key.CreateSubKey(appVersion);
			key = key.OpenSubKey(appVersion, true);

			return key;
		}
	}
}
