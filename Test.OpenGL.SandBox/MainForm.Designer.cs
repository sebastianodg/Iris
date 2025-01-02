﻿namespace Test.OpenGL.SandBox
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
			_irisRenderWindow = new Iris.Graphics.RenderWindow.IrisRenderWindow();
			renderControl1 = new RenderControl();
			this.SuspendLayout();
			// 
			// _irisRenderWindow
			// 
			_irisRenderWindow.BackColor = Color.FromArgb(44, 44, 44);
			_irisRenderWindow.Dock = DockStyle.Right;
			_irisRenderWindow.Location = new Point(930, 0);
			_irisRenderWindow.Name = "_irisRenderWindow";
			_irisRenderWindow.Size = new Size(968, 1024);
			_irisRenderWindow.TabIndex = 0;
			// 
			// renderControl1
			// 
			renderControl1.Dock = DockStyle.Fill;
			renderControl1.Location = new Point(0, 0);
			renderControl1.Name = "renderControl1";
			renderControl1.Size = new Size(930, 1024);
			renderControl1.TabIndex = 1;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(10F, 25F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1898, 1024);
			Controls.Add(renderControl1);
			Controls.Add(_irisRenderWindow);
			Name = "MainForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "OpenGL SandBox";
			this.ResumeLayout(false);
		}

		#endregion

		private Iris.Graphics.RenderWindow.IrisRenderWindow _irisRenderWindow;
		private RenderControl renderControl1;
	}
}
