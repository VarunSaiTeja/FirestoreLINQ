# FirestoreLINQ
<a href="https://www.nuget.org/packages/FirestoreLINQ"><img alt="NuGet Version" src="https://img.shields.io/nuget/v/FirestoreLINQ"></a>
<a href="https://www.nuget.org/packages/FirestoreLINQ"><img alt="NuGet Downloads" src="https://img.shields.io/nuget/dt/FirestoreLINQ"></a>

[!["Buy Me A Coffee"](https://cdn.buymeacoffee.com/assets/img/home-page-v3/bmc-new-logo.png)](https://www.buymeacoffee.com/varunteja)

## Gives LINQ support to Firestore database.

### Getting started

#### Define your C# Poco that matches the firestore document

```csharp
[FirestoreData]
public class Student
{
    [FirestoreProperty]
    public List<string> Skills { get; set; }

    [FirestoreProperty]
    public string FirstName { get; set; }

    [FirestoreProperty]
    public string LastName { get; set; }

    [FirestoreProperty]
    public int Age { get; set; }
}
````

#### Add AsQuerable to Collection Reference
```csharp
public class QueryTest
{
    FirestoreDb db;
    QueryTest()
    {
        db = new FirestoreDbBuilder
        {
            ProjectId = <<Project ID>>,
            JsonCredentials = "<<Path to JSON>>"
        }.Build();
    }

    public void GetYoungers()
    {
        var youngers = db.Collection("Students").AsQuerable<Student>()
                                                .Where(s => s.Age >= 15 && s.Age <= 25).ToList();
        
        //Above LINQ Expression will be executed in form of following firestore query
        var youngersTranslated = db.Collection("Students")
                                    .WhereGreaterThanOrEqualTo("Age", 15)
                                    .WhereLessThanOrEqualTo("Age", 25)
                                    .Select(x=> x.ConvertTo<Student>())
                                    .ToList();
    }
}
```

#### Here are supported Query Methods
Any, Avg, Count, First, FirstOrDefault, Last, LastOrDefault, Max, MaxBy, Min, MinBy, OrderBy, OrderByDescending, Select, Single, SingleOrDefault, Skip, Sum, Take, Where

Note: Where predicate with OR clause not yet supported.

More features coming soon.
