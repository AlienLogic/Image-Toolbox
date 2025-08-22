using Microsoft.Win32;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Viewer
{
	public partial class MainForm : Form
	{
		readonly private RegistryKey key;
		readonly private uint zoomMin = 0;
		readonly private uint zoomMax = 20;

		private uint zoomFactor = 0;

		private Image image;

		public MainForm(SettingsHelper settings)
		{
			key = settings.GetUserRegistryKey();
			InitializeComponent();

			pictureBox1.MouseWheel += pictureBox1_MouseWheel;
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

		/* PictureBox code */
		private void pictureBox1_DoubleClick(object sender, EventArgs e)
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					image?.Dispose();
					image = Image.FromFile(dialog.FileName);
					pictureBox1.Image = image;
				}
			}
		}

		private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta > 0 && zoomFactor < zoomMax)
				zoomFactor++;

			if (e.Delta < 0 && zoomFactor > zoomMin)
				zoomFactor--;

			Console.WriteLine(Math.Pow(1.2f, zoomFactor));
			pictureBox1.Invalidate();
		}

		private void pictureBox1_Paint(object sender, PaintEventArgs e)
		{
			if (image == null) return;

			Graphics g = e.Graphics;
			g.InterpolationMode = InterpolationMode.NearestNeighbor;
			g.PixelOffsetMode = PixelOffsetMode.None;
			g.SmoothingMode = SmoothingMode.None;
			//g.CompositingQuality = CompositingQuality.AssumeLinear; // ?

			// scale factor for zoom
			float scale = (float)Math.Pow(1.2f, zoomFactor);

			// calc center image in picturebox
			float cx = pictureBox1.ClientSize.Width / 2f;
			float cy = pictureBox1.ClientSize.Height / 2f;

			int w = (int)Math.Round(image.Width * scale);
			int h = (int)Math.Round(image.Height * scale);

			int x = (int)Math.Round(cx - w / 2.0);
			int y = (int)Math.Round(cy - h / 2.0);

			//float imgW = image.Width;
			//float imgH = image.Height;

			// save state and apply transformation: transform to center, scale then center image
			var state = g.Save();
			//g.TranslateTransform(cx, cy);                 // image top left to center of picturebox
			//g.ScaleTransform(scale, scale);		        // apply zoom
			//g.TranslateTransform(-imgW / 2f, -imgH / 2f); // center image in picturebox
			//g.DrawImage(image, 0, 0, imgW, imgH);
			g.DrawImage(image, x, y, w, h);
			g.Restore(state);
		}
	}
}
