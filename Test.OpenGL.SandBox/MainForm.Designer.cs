namespace Test.OpenGL.SandBox
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
			_renderControl = new RenderControl();
			this.SuspendLayout();
			// 
			// _renderControl
			// 
			_renderControl.Dock = DockStyle.Fill;
			_renderControl.Location = new Point(0, 0);
			_renderControl.Name = "_renderControl";
			_renderControl.Size = new Size(1898, 1024);
			_renderControl.TabIndex = 0;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(10F, 25F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1898, 1024);
			Controls.Add(_renderControl);
			Name = "MainForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "OpenGL SandBox";
			this.ResumeLayout(false);
		}

		#endregion

		private RenderControl _renderControl;
	}
}
