
using ActivityInfo;
using ActivityInfo.Query;
using System;
using System.Collections.Generic;

namespace UpdateTest
{
    public enum SchoolType
    {
        [EnumId("t1115438694")]
        Public,

        [EnumId("t1107936090")]
        Catholic
    }

    class School : BaseRecord
    {
        public School() : base("a2145507918")
        {
        }
        public string Name { get; set; }
        public DateTime DateStarted { get; set; }
        public RecordRef Partner { get; set; }
        public GeoPoint Point { get; set; }
        public int Students { get; set; }
        public RecordRef Camp { get; set; }
        public SchoolType Type { get; set; }
    }

    class RefugeeCampQuery 
    {
        [RecordId]
        public string Id { get; set; }

        public string Name { get; set; }

        [Formula("Governorate.Name")]
        public string Governorate { get; set; }

        [Formula("Region.Name")]
        public string Region { get; set; }

    }

    class MainClass
    {
        
        public static void Main(string[] args)
        {
            var client = new ActivityInfo.Client("api+training@bedatadriven.com", "testing123");

            var database = client.QueryDatabase(9907);

            var form = client.QuerySchema("a2145507918");

            var partners = client.QueryPartners(9907);

            var campFormId = "E0000001527";
            var camps = client.Query<RefugeeCampQuery>(campFormId);

            School school = new School();
            school.Partner = partners[0].RecordRef;
            school.Name = "Kirby Elementary School";
            school.Point = new ActivityInfo.GeoPoint(14.5, 20.5);
            school.Students = 94;
            school.Camp = new RecordRef(campFormId, camps[0].Id);
            school.Type = SchoolType.Catholic;
            school.DateStarted = new DateTime(1982, 1, 16);

            var newRecordRef = client.CreateRecord(school);

            var records = client.Query<School>("a2145507918");

            Console.WriteLine(records);

        }



    }
}
