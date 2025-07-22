namespace MouseAttendance;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.ListBox listBoxLogs;

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
        this.components = new System.ComponentModel.Container();
    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    this.ClientSize = new System.Drawing.Size(800, 450);
    // listBoxLogs
    this.listBoxLogs = new System.Windows.Forms.ListBox();
    this.listBoxLogs.Dock = System.Windows.Forms.DockStyle.Fill;
    this.listBoxLogs.FormattingEnabled = true;
    this.listBoxLogs.ItemHeight = 15;
    this.listBoxLogs.Name = "listBoxLogs";
    this.Controls.Add(this.listBoxLogs);
    this.Text = "Mouse Attendance";
    }

    #endregion
}
