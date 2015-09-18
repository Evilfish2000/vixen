namespace VixenModules.Output.DebugController
{
	partial class DebugControllerOutputForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.chkVerbose = new System.Windows.Forms.CheckBox();
            this.panel_checkbox = new System.Windows.Forms.Panel();
            this.panel_outputWindow = new System.Windows.Forms.Panel();
            this.panel_checkbox.SuspendLayout();
            this.panel_outputWindow.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxOutput.Location = new System.Drawing.Point(15, 15);
            this.textBoxOutput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.Size = new System.Drawing.Size(777, 520);
            this.textBoxOutput.TabIndex = 0;
            // 
            // chkVerbose
            // 
            this.chkVerbose.AutoSize = true;
            this.chkVerbose.Dock = System.Windows.Forms.DockStyle.Right;
            this.chkVerbose.Location = new System.Drawing.Point(672, 0);
            this.chkVerbose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkVerbose.Name = "chkVerbose";
            this.chkVerbose.Size = new System.Drawing.Size(95, 73);
            this.chkVerbose.TabIndex = 1;
            this.chkVerbose.Text = "Verbose";
            this.chkVerbose.UseVisualStyleBackColor = true;
            this.chkVerbose.CheckedChanged += new System.EventHandler(this.chkVerbose_CheckedChanged);
            // 
            // panel_checkbox
            // 
            this.panel_checkbox.Controls.Add(this.chkVerbose);
            this.panel_checkbox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_checkbox.Location = new System.Drawing.Point(0, 550);
            this.panel_checkbox.Name = "panel_checkbox";
            this.panel_checkbox.Padding = new System.Windows.Forms.Padding(0, 0, 40, 0);
            this.panel_checkbox.Size = new System.Drawing.Size(807, 73);
            this.panel_checkbox.TabIndex = 2;
            // 
            // panel_outputWindow
            // 
            this.panel_outputWindow.Controls.Add(this.textBoxOutput);
            this.panel_outputWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_outputWindow.Location = new System.Drawing.Point(0, 0);
            this.panel_outputWindow.Name = "panel_outputWindow";
            this.panel_outputWindow.Padding = new System.Windows.Forms.Padding(15);
            this.panel_outputWindow.Size = new System.Drawing.Size(807, 550);
            this.panel_outputWindow.TabIndex = 3;
            // 
            // DebugControllerOutputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 623);
            this.Controls.Add(this.panel_outputWindow);
            this.Controls.Add(this.panel_checkbox);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "DebugControllerOutputForm";
            this.Text = "Debug Controller Output";
            this.panel_checkbox.ResumeLayout(false);
            this.panel_checkbox.PerformLayout();
            this.panel_outputWindow.ResumeLayout(false);
            this.panel_outputWindow.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.CheckBox chkVerbose;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Panel panel_checkbox;
        private System.Windows.Forms.Panel panel_outputWindow;
    }
}