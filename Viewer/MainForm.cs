using Microsoft.Win32;

using System;
using System.Windows.Forms;

namespace Viewer
{
	public partial class MainForm : Form
	{
		readonly private RegistryKey key;

		public MainForm(SettingsHelper settings)
		{
			key = settings.GetUserRegistryKey();
			InitializeComponent();
		}

		private void mainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
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

		private void mainForm_ResizeEnd(object sender, EventArgs e)
		{
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
			FormWindowState previousState = WindowState;
			base.WndProc(ref m);
			if (WindowState != previousState)
				OnFormWindowStateChanged();
		}

		protected virtual void OnFormWindowStateChanged()
		{
			if (WindowState != FormWindowState.Minimized)
			{
				key.SetValue(nameof(WindowState), (int)WindowState, RegistryValueKind.DWord);
			}
		}
	}
}
