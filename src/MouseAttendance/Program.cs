using System;
using System.Threading;

namespace MouseAttendance;

static class Program
{
    static void Main()
    {
        Console.WriteLine("Mouse Attendance Console started. Press Ctrl+C to exit.");
        var detector = new AttendanceDetector();
        var reporter = new AttendanceReporter();
        using var scheduler = new Scheduler(detector, reporter);
        // Keep running until cancelled
        var exitEvent = new ManualResetEvent(false);
        Console.CancelKeyPress += (sender, e) => exitEvent.Set();
        exitEvent.WaitOne();
    }
}