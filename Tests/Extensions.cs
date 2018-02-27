using NUnit.Framework;
using RestSharp;

namespace Tests
{
    [TestFixture]
    public static class Extensions {
        [Test]
        public static void Foo() {
            var client = new RestClient("http://localhost:776/keepAlive");
            var request = new RestRequest(Method.POST);

            request.AddParameter("application/json", "{\n\t\"location\": {\n\t\t\"latitude\": \"Esta!\",\n\t\t\"longitude\": \"Esta!\"\n\t}\n}", ParameterType.RequestBody);

            var response = client.Execute(request);
        }

        //[Test]
        //public static void UnixEpoch() {
        //    var now = DateTime.Now;
        //    var timestamp = LibreR.Controllers.Extensions.DateTimeToUnixEpoch();
        //    var date = LibreR.Controllers.Extensions.GetDateTimeFromUnixEpoch(timestamp);

        //    Console.WriteLine($"Today is: {now}");
        //    Console.WriteLine($"Seconds since epoch: {timestamp}");
        //    Console.WriteLine($"Today should be: {date}");

        //    if (
        //        now.Year != date.Year &&
        //        now.Month != date.Month &&
        //        now.Day != date.Day &&
        //        now.Hour != date.Hour &&
        //        now.Minute != date.Minute &&
        //        (now.Second != date.Second && now.Second != date.Second - 1)
        //    )
        //        Assert.Fail();
        //}
    }
}
