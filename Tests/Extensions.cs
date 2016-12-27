using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibreR.Controllers;
using LibreR.Models.Enums;
using NUnit.Framework;

namespace Tests {
    [TestFixture]
    public static class Extensions {
        public static void Main() {
            Console.WriteLine(new { X = CaretPosition.End }.Serialize());
            Thread.Sleep(999999);
        }

        [Test]
        public static void UnixEpoch() {
            var now = DateTime.Now;
            var timestamp = LibreR.Controllers.Extensions.DateTimeToUnixEpoch();
            var date = LibreR.Controllers.Extensions.GetDateTimeFromUnixEpoch(timestamp);

            Console.WriteLine($"Today is: {now}");
            Console.WriteLine($"Seconds since epoch: {timestamp}");
            Console.WriteLine($"Today should be: {date}");

            if (
                now.Year != date.Year &&
                now.Month != date.Month &&
                now.Day != date.Day &&
                now.Hour != date.Hour &&
                now.Minute != date.Minute &&
                (now.Second != date.Second && now.Second != date.Second - 1)
            )
                Assert.Fail();
        }
    }
}
