using ActivityInfo.Query;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;

namespace ActivityInfoTest
{
    [TestFixture()]
    public class Test
    {
        [Test()]
        public void TestCase()
        {
            var filename = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "TestData", "columns.json");
            using (StreamReader reader = File.OpenText(filename))
            {
                var json = new JsonSerializer();
                ColumnSet columnSet = json.Deserialize<ColumnSet>(new JsonTextReader(reader));

                Assert.AreEqual(2, columnSet.RowCount);
            }
        }
    }
}
