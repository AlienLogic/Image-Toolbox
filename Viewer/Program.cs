using Microsoft.Win32;

using System;
using System.Windows.Forms;

namespace Viewer
{
	internal static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			SettingsHelper settings = new SettingsHelper("AlienLogic", "Image Viewer", "0.0_WinForms");
			RegistryKey key = settings.GetUserRegistryKey();

			Form mainForm = new MainForm(settings);

			try // read settings value
			{
				mainForm.WindowState = (FormWindowState)key.GetValue(nameof(mainForm.WindowState), FormWindowState.Normal);

				mainForm.Height = (int)key.GetValue(nameof(mainForm.Height));
				mainForm.Width = (int)key.GetValue(nameof(mainForm.Width));

				mainForm.Left = (int)key.GetValue(nameof(mainForm.Left));
				mainForm.Top = (int)key.GetValue(nameof(mainForm.Top));
			}
			catch (Exception) // write default value
			{
				mainForm.Height = 720;
				mainForm.Width = 1280;
				mainForm.StartPosition = FormStartPosition.CenterScreen;

				key.SetValue(nameof(mainForm.Height), mainForm.Height);
				key.SetValue(nameof(mainForm.Width), mainForm.Width);
			}

			Application.Run(mainForm);
		}
	}
}
