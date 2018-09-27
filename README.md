
# ActivityInfo.NET

This repository contains a client for ActivityInfo for the .NET environment, written in C#.

## Adding the Library to your Project

The ActivityInfo Client is available as a [Nuget Package](https://www.nuget.org/packages/ActivityInfo.Client/).

## Creating the ActivityInfo Client

The ActivityInfo `Client` class must be instantiated with your email and password for your ActivityInfo
account:

```.cs
using ActivityInfo;
using ActivityInfo.Query;

namespace MyApplication
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var client = new ActivityInfo.Client("api+training@bedatadriven.com", "testing123");
        }
    }
}
```

## Defining Form model classes

Defining a model class makes it easy to create, query, and update records for a specific form. 

Your model class should extend the `ActivityInfo.BaseRecord` class, and 
define a property for each ActivityInfo field. The field should have the same 
name as the field's code.

For example, if you have a form for collecting data on Schools that has the following ActivityInfo
Schema:

```.cs

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

```

The type of the property should correspond to the ActivityInfo field type:

ActivityInfo Field Type  | .NET Property Type
-------------------------|--------------------
Text, Narrative, Barcode | string 
Serial Number            | string
Quantity                 | int, double
Date                     | DateTime
Single Select            | Enum (see below)
Multiple Select          | Set<Enum>
Reference Field          | ActivityInfo.RecordRef
GeoPoint                 | ActivityInfo.GeoPoint


### Enums

For select and multiple select fields, you can define an Enum
type that corresponds to the ActivityInfo field. For example,
for a select field that looks like:

```.cs

    public enum SchoolType
    {
        [EnumId("t1115438694")]
        Public,

        [EnumId("t1107936090")]
        Private
    }
``` 

The `ActivityInfo.EnumId` attribute specifies links the 
enumerated id to the Enum member.

## Querying Records

Records can be queried from ActivityInfo by using the `Query<>` method
and a Record model class:


```.cs
var client = new ActivityInfo.Client("user", "password");
var schoolFormId = "a2145507918";

List<School> schools = client.Query<School>(schoolFormId);
foreach(School school in schools) {
    Console.WriteLine(school.Name);
}
```

## Creating New Records


```.cs
var databaseId = 9907;
var partners = client.QueryPartners(databaseId);


School school = new School();
school.Partner = partners[0].RecordRef;
school.Name = "Kirby Elementary School";
school.Point = new ActivityInfo.GeoPoint(14.5, 20.5);
school.Students = 94;
school.Type = SchoolType.Private;
school.DateStarted = new DateTime(1982, 1, 16);

var newSchoolRef = client.CreateRecord(school);

```

## Updating a Record

```.cs
var schools = client.Query<School>(schoolFormId);

schools[0].Name = "Updated School Name";

client.UpdateRecord(schools[0]);

```

## Deleting a Record 







