using Microsoft.VisualStudio.TestTools.UnitTesting;
using MouseAttendance;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MouseAttendance.Tests
{
    [TestClass]
    public class AttendanceDetectorTests
    {
        [TestMethod]
        public void Tick_NewPosition_RaisesWorking()
        {
            var detector = new AttendanceDetector();
            var events = new List<AttendanceEventArgs>();
            detector.StateChanged += (s, e) => events.Add(e);

            var time0 = new DateTime(2025, 7, 22, 9, 0, 0);
            var pos = new Point(100, 100);
            detector.Tick(time0, pos);

            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(State.Working, events[0].State);
            Assert.AreEqual(time0, events[0].Timestamp);
        }

        [TestMethod]
        public void Tick_SamePositionWithin10Minutes_NoEvent()
        {
            var detector = new AttendanceDetector();
            var events = new List<AttendanceEventArgs>();
            detector.StateChanged += (s, e) => events.Add(e);

            var time0 = new DateTime(2025, 7, 22, 9, 0, 0);
            var pos = new Point(100, 100);
            detector.Tick(time0, pos);
            detector.Tick(time0.AddMinutes(5), pos);

            Assert.AreEqual(1, events.Count);
        }

        [TestMethod]
        public void Tick_SamePositionAfter10Minutes_RaisesLeft()
        {
            var detector = new AttendanceDetector();
            var events = new List<AttendanceEventArgs>();
            detector.StateChanged += (s, e) => events.Add(e);

            var time0 = new DateTime(2025, 7, 22, 9, 0, 0);
            var pos = new Point(50, 50);
            detector.Tick(time0, pos); // Working
            var timeIdle = time0.AddMinutes(10);
            detector.Tick(timeIdle, pos); // Idle threshold

            Assert.AreEqual(2, events.Count);
            Assert.AreEqual(State.Left, events[1].State);
            Assert.AreEqual(timeIdle, events[1].Timestamp);
        }

        [TestMethod]
        public void Tick_FluctuateStates_RaisesEventsOnlyOnChange()
        {
            var detector = new AttendanceDetector();
            var events = new List<AttendanceEventArgs>();
            detector.StateChanged += (s, e) => events.Add(e);

            var t0 = new DateTime(2025, 7, 22, 9, 0, 0);
            var p1 = new Point(10, 10);
            detector.Tick(t0, p1); // Working
            // Idle after 10
            detector.Tick(t0.AddMinutes(10), p1);
            // Move again after idle
            var t2 = t0.AddMinutes(15);
            var p2 = new Point(20, 20);
            detector.Tick(t2, p2);

            Assert.AreEqual(3, events.Count);
            Assert.AreEqual(State.Working, events[0].State);
            Assert.AreEqual(State.Left, events[1].State);
            Assert.AreEqual(State.Working, events[2].State);
        }
    }
}
