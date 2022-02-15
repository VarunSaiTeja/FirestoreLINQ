using FirestoreLINQ;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class MockDataSetup : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<DataType> types => _db.firestoreDb.AsQuerable<DataType>();

        public MockDataSetup(MyDb db)
        {
            _db = db;
        }

        [Fact]
        //[Fact(Skip = "Use this test only for mocking/inserting Types data in firestore, Run this test alone intially.")]
        public void Setup_Types()
        {
            var rs = _db.firestoreDb.Collection<DataType>().Document("Test").SetAsync(DataTypeCases.dataTypes).Result;
            Assert.NotNull(rs);
        }

        [Fact]
        //[Fact(Skip = "Use this test only for mocking/inserting Students data in firestore, Run this test alone intially.")]
        public void Setup_Students()
        {
            //Assert.Equal("Sai", firstNames[0]);
            //Assert.Equal("Varun", firstNames[1]);
            var students = new List<Student>
            {
                new Student
                {
                    FirstName = "Sai",
                    LastName = "Teja",
                    Age = 25
                },
                new Student
                {
                    FirstName = "Harshini",
                    LastName = "Darisi",
                    Skills = new List<string>
                    {
                        "Problem Solver",
                        "LINQ"
                    },
                    Age=15
                },
                new Student
                {
                    Age = 30,
                    FirstName = "Varun",
                    LastName = "Teja",
                    Skills = new List<string>
                    {
                        "Problem Solver"
                    }
                }
            };
            Console.WriteLine(students.Count);
            foreach (var student in students)
            {
                var rs = _db.firestoreDb.Collection<Student>().AddAsync(student).Result;
                //var rs = _db.firestoreDb.Collection("Students").AddAsync(student).Result;
                Assert.NotNull(rs);
            }
        }
    }
}
