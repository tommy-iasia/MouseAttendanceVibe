using Microsoft.VisualStudio.TestTools.UnitTesting;
using MouseAttendance;
using System;
using System.Collections.Generic;

namespace MouseAttendance.Tests
{
    [TestClass]
    public class AttendanceReporterTests
    {
    // Initialize with null-forgiving to satisfy non-nullable requirement; actual assignment in Setup
    private AttendanceReporter reporter = null!;
    private List<AttendanceEventArgs> reported = null!;

        [TestInitialize]
        public void Setup()
        {
            reporter = new AttendanceReporter();
            reported = new List<AttendanceEventArgs>();
            reporter.StateReported += (s, e) => reported.Add(e);
        }

        [TestMethod]
        public void WorkingWithinMorningWindow_IsReported()
        {
            var time = new DateTime(2025, 7, 22, 9, 30, 0);
            reporter.OnDetectorStateChanged(null, new AttendanceEventArgs(State.Working, time));

            Assert.AreEqual(1, reported.Count);
            Assert.AreEqual(State.Working, reported[0].State);
            Assert.AreEqual(time, reported[0].Timestamp);
        }

        [TestMethod]
        public void WorkingOutsideMorningWindow_IsSuppressed()
        {
            var time = new DateTime(2025, 7, 22, 7, 59, 59);
            reporter.OnDetectorStateChanged(null, new AttendanceEventArgs(State.Working, time));

            Assert.AreEqual(0, reported.Count);
        }

        [TestMethod]
        public void LeftWithinEveningWindow_IsReported()
        {
            var time = new DateTime(2025, 7, 22, 17, 0, 0);
            reporter.OnDetectorStateChanged(null, new AttendanceEventArgs(State.Left, time));

            Assert.AreEqual(1, reported.Count);
            Assert.AreEqual(State.Left, reported[0].State);
            Assert.AreEqual(time, reported[0].Timestamp);
        }

        [TestMethod]
        public void LeftOutsideEveningWindow_IsSuppressed()
        {
            var time = new DateTime(2025, 7, 22, 19, 0, 0);
            reporter.OnDetectorStateChanged(null, new AttendanceEventArgs(State.Left, time));

            Assert.AreEqual(0, reported.Count);
        }

        [TestMethod]
        public void DuplicateStatesAreSuppressedUntilChange()
        {
            var t1 = new DateTime(2025, 7, 22, 9, 0, 0);
            reporter.OnDetectorStateChanged(null, new AttendanceEventArgs(State.Working, t1));
            reporter.OnDetectorStateChanged(null, new AttendanceEventArgs(State.Working, t1.AddMinutes(1)));

            Assert.AreEqual(1, reported.Count);
        }

        [TestMethod]
        public void SubsequentDifferentStateResetsSuppression()
        {
            var t1 = new DateTime(2025, 7, 22, 9, 0, 0);
            reporter.OnDetectorStateChanged(null, new AttendanceEventArgs(State.Working, t1));
            var t2 = new DateTime(2025, 7, 22, 17, 30, 0);
            reporter.OnDetectorStateChanged(null, new AttendanceEventArgs(State.Left, t2));
            reporter.OnDetectorStateChanged(null, new AttendanceEventArgs(State.Left, t2.AddMinutes(1)));

            Assert.AreEqual(2, reported.Count);
            Assert.AreEqual(State.Working, reported[0].State);
            Assert.AreEqual(State.Left, reported[1].State);
        }
    }
}
