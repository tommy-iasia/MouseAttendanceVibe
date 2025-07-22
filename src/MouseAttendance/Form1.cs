namespace MouseAttendance;

public partial class Form1 : Form
{
    private readonly AttendanceReporter _reporter;
    public Form1(AttendanceReporter reporter)
    {
        InitializeComponent();
        _reporter = reporter;
        _reporter.StateReported += Reporter_StateReported;
    }

    private void Reporter_StateReported(object? sender, AttendanceEventArgs e)
    {
        // Invoke on UI thread
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => Reporter_StateReported(sender, e)));
            return;
        }
        listBoxLogs.Items.Add($"{e.Timestamp:yyyy-MM-dd HH:mm:ss} - {e.State}");
    }
}
