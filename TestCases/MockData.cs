using System.Collections.Generic;

namespace TestCases
{
    public static class MockData
    {
        public static List<Student> Students = new List<Student>
            {
                new Student
                {
                    Id="7XEjk6eOGov2RGq4FIvl",
                    FirstName = "Sai",
                    LastName = "Teja",
                    Age = 25
                },
                new Student
                {
                    Id="MAwrerO5jDYGUpwFVQHY",
                    Age = 30,
                    FirstName = "Varun",
                    LastName = "Teja",
                    Skills = new List<string>
                    {
                        "Problem Solver"
                    },
                    Address = new Address
                    {
                        City = "Hyderabad",
                        Country = "India"
                    }
                },
                new Student
                {
                    Id="jIdzo0HTUeJT3nfH1McM",
                    FirstName = "Harshini",
                    LastName = "Darisi",
                    Skills = new List<string>
                    {
                        "Problem Solver",
                        "LINQ"
                    },
                    Age = 15
                },
            };
    }
}
