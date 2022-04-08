using FirestoreLINQ;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class WhereClauses : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<Student> students => _db.firestoreDb.AsQuerable<Student>();

        public WhereClauses(MyDb db)
        {
            _db = db;
        }

        [Fact]
        public void WhereEqualTo()
        {
            var results = students.Where(x => x.Age == 25).ToList();
            var mockResults = MockData.Students.Where(x => x.Age == 25).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void WhereNotEqualTo()
        {
            var results = students.Where(x => x.Age != 25).ToList();
            var mockResults = MockData.Students.Where(x => x.Age != 25).ToList();

            Assert.Equal(mockResults.Count,results.Count);
        }

        [Fact]
        public void WhereGreaterThan()
        {
            var results = students.Where(x => x.Age > 25).ToList();
            var mockResults = MockData.Students.Where(x => x.Age > 25).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void WhereGreaterThanOrEqualTo()
        {
            var results = students.Where(x => x.Age >= 25).ToList();
            var mockResults = MockData.Students.Where(x => x.Age >= 25).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void WhereLessThan()
        {
            var results = students.Where(x => x.Age < 25).ToList();
            var mockResults = MockData.Students.Where(x => x.Age < 25).ToList();

            Assert.Equal(mockResults,results);
        }


        [Fact]
        public void WhereLessThanOrEqualTo()
        {
            var results = students.Where(x => x.Age <= 25).ToList();
            var mockResults = MockData.Students.Where(x => x.Age <= 25).ToList();

            Assert.Equal(mockResults.Count,results.Count);
        }

        [Fact]
        public void WhereArrayContains_Inline()
        {
            var results = students.Where(x => x.Skills.Contains("Problem Solver")).ToList();
            var mockResults = MockData.Students.Where(x => x.Skills?.Contains("Problem Solver")??false).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void WhereArrayContains_Member()
        {
            var requiredSkill = "Problem Solver";

            var results = students.Where(x => x.Skills.Contains(requiredSkill)).ToList();
            var mockResults = MockData.Students.Where(x => x.Skills?.Contains(requiredSkill)??false).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void WhereArrayContainsAny_MemberList()
        {
            var skills = new List<string> { "Problem Solver", "Hacker" };

            var results = students.Where(x => x.Skills.Any(x => skills.Contains(x))).ToList();
            var mockResults = MockData.Students.Where(x => x.Skills?.Any(x => skills.Contains(x))??false).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void WhereArrayContainsAny_InlineList()
        {
            var results = students.Where(x => x.Skills.Any(x => new List<string> { "Problem Solver", "Hacker" }.Contains(x))).ToList();
            
            var mockResults = MockData.Students.Where(x => x.Skills?.Any(x => new List<string> { "Problem Solver", "Hacker" }.Contains(x))??false).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void WhereArrayContainsAny_MemberArray()
        {
            string[] skills = { "Problem Solver", "Hacker" };

            var results = students.Where(x => x.Skills.Any(x => skills.Contains(x))).ToList();
            var mockResults = MockData.Students.Where(x => x.Skills?.Any(x => skills.Contains(x))??false).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void WhereIn_MemberArray()
        {
            string[] lastNames = { "Teja" };

            var results = students.Where(x => lastNames.Contains(x.LastName)).ToList();
            var mockResults = MockData.Students.Where(x => lastNames.Contains(x.LastName)).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void WhereIn_MemberList()
        {
            var lastNames = new List<string> { "Teja" };

            var results = students.Where(x => lastNames.Contains(x.LastName)).ToList();
            var mockResults = MockData.Students.Where(x => lastNames.Contains(x.LastName)).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void WhereIn_InlineList()
        {
            var results = students.Where(x => new List<string> { "Teja" }.Contains(x.LastName)).ToList();
            var mockResults = MockData.Students.Where(x => new List<string> { "Teja" }.Contains(x.LastName)).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void WhereNotIn_InlineList()
        {
            var results = students.Where(x => !new List<string> { "Teja" }.Contains(x.LastName)).ToList();
            var mockResults = MockData.Students.Where(x => !new List<string> { "Teja" }.Contains(x.LastName)).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void WhereNotIn_MemberList()
        {
            var lastNames = new List<string> { "Teja" };

            var results = students.Where(x => !lastNames.Contains(x.LastName)).ToList();
            var mockResults = MockData.Students.Where(x => !lastNames.Contains(x.LastName)).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void WhereNotIn_MemberArray()
        {
            string[] lastNames = { "Teja" };

            var results = students.Where(x => !lastNames.Contains(x.LastName)).ToList();
            var mockResults = MockData.Students.Where(x => !lastNames.Contains(x.LastName)).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void Where_AndAlso_Where()
        {
            var results = students.Where(x => x.Age > 15 && x.Age < 30).ToList();
            var mockResults = MockData.Students.Where(x => x.Age > 15 && x.Age < 30).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void Where_FirestoreMemberComparision_ThrowsException()
        {
            var ex = Assert.Throws<NotSupportedException>(() => students.Where(x => x.FirstName != x.LastName).ToList());
            Assert.Equal(Constants.FirestoreDontSupportMsg, ex.Message);
        }

        [Fact]
        public void WhereStringStartsWithSingleChar()
        {
            var results = students.Where(x => x.FirstName.StartsWith("H")).ToList();
            var mockResults = MockData.Students.Where(x => x.FirstName.StartsWith("H")).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void WhereStringStartsWithSingleString()
        {
            var results = students.Where(x => x.LastName.StartsWith("Ta")).ToList();
            var mockResults = MockData.Students.Where(x => x.LastName.StartsWith("Ta")).ToList();
            Assert.Equal(mockResults,results);

            results = students.Where(x => x.LastName.StartsWith("Te")).ToList();
            mockResults = MockData.Students.Where(x => x.LastName.StartsWith("Te")).ToList();

            Assert.Equal(mockResults,results);
        }

        [Fact]
        public void NestedWhereClause()
        {
            var results = students.Where(x => x.Address.Country == "India").ToList();
            var mockResults= MockData.Students.Where(x => x.Address?.Country == "India").ToList();

            Assert.Equal(mockResults,results);
        }
    }
}