using Microsoft.Win32;

using System;
using System.Windows.Forms;

namespace Viewer
{
	public partial class MainForm : Form
	{
		readonly private SettingsHelper settings;
		private Tuple<bool, FormWindowState> windowState;

		public MainForm(SettingsHelper settings)
		{
			this.settings = settings;
			InitializeComponent();
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			RegistryKey key = settings.GetUserRegistryKey();

			try // write settings value
			{
				if (WindowState != FormWindowState.Minimized)
				{
					key.SetValue(nameof(WindowState), (int)WindowState, RegistryValueKind.DWord);

					if (WindowState != FormWindowState.Maximized)
					{
						key.SetValue(nameof(Height), Height, RegistryValueKind.DWord);
						key.SetValue(nameof(Width), Width, RegistryValueKind.DWord);

						key.SetValue(nameof(Left), Left, RegistryValueKind.DWord);
						key.SetValue(nameof(Top), Top, RegistryValueKind.DWord);
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void MainForm_ResizeEnd(object sender, EventArgs e)
		{
			RegistryKey key = settings.GetUserRegistryKey();

			try // write settings value (size & position changed)
			{
				key.SetValue(nameof(Height), Height, RegistryValueKind.DWord);
				key.SetValue(nameof(Width), Width, RegistryValueKind.DWord);

				key.SetValue(nameof(Left), Left, RegistryValueKind.DWord);
				key.SetValue(nameof(Top), Top, RegistryValueKind.DWord);
			}
			catch (Exception)
			{
				throw;
			}
		}

		protected override void WndProc(ref Message m)
		{
			FormWindowState org = WindowState;
			base.WndProc(ref m);
			if (WindowState != org)
				OnFormWindowStateChanged(EventArgs.Empty);
		}

		protected virtual void OnFormWindowStateChanged(EventArgs e)
		{
			if (WindowState != FormWindowState.Minimized)
			{
				RegistryKey key = settings.GetUserRegistryKey();
				key.SetValue(nameof(WindowState), (int)WindowState, RegistryValueKind.DWord);
			}
		}
	}
}
