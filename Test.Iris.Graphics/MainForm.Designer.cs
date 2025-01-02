using Iris.Graphics.RenderControl;

namespace Test.Iris.Graphics
{
	partial class MainForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._irisRenderWindow = new IrisRenderControl();
			this.SuspendLayout();
			// 
			// _irisRenderWindow
			// 
			this._irisRenderWindow.BackColor = Color.FromArgb(44, 44, 44);
			this._irisRenderWindow.Dock = DockStyle.Fill;
			this._irisRenderWindow.Location = new Point(0, 0);
			this._irisRenderWindow.Margin = new Padding(1);
			this._irisRenderWindow.Name = "_irisRenderWindow";
			this._irisRenderWindow.Size = new Size(944, 501);
			this._irisRenderWindow.TabIndex = 0;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(944, 501);
			Controls.Add(this._irisRenderWindow);
			Margin = new Padding(2);
			Name = "MainForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Test Iris Graphics";
			this.ResumeLayout(false);
		}

		#endregion

		private IrisRenderControl _irisRenderWindow;
	}
}
