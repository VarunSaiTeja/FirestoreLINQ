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
            Assert.Single(results);
        }

        [Fact]
        public void WhereNotEqualTo()
        {
            var results = students.Where(x => x.Age != 25).ToList();
            Assert.True(results.Count == 2);
        }

        [Fact]
        public void WhereGreaterThan()
        {
            var results = students.Where(x => x.Age > 25).ToList();
            Assert.True(results.Count == 1);
        }

        [Fact]
        public void WhereGreaterThanOrEqualTo()
        {
            var results = students.Where(x => x.Age >= 25).ToList();
            Assert.True(results.Count == 2);
        }

        [Fact]
        public void WhereLessThan()
        {
            var results = students.Where(x => x.Age < 25).ToList();
            Assert.True(results.Count == 1);
        }


        [Fact]
        public void WhereLessThanOrEqualTo()
        {
            var results = students.Where(x => x.Age <= 25).ToList();
            Assert.True(results.Count == 2);
        }

        [Fact]
        public void WhereArrayContains_Inline()
        {
            var results = students.Where(x => x.Skills.Contains("Problem Solver")).ToList();
            Assert.True(results.Count == 2);
        }

        [Fact]
        public void WhereArrayContains_Member()
        {
            var requiredSkill = "Problem Solver";
            var results = students.Where(x => x.Skills.Contains(requiredSkill)).ToList();
            Assert.True(results.Count == 2);
        }

        [Fact]
        public void WhereArrayContainsAny_MemberList()
        {
            var skills = new List<string> { "Problem Solver", "Hacker" };
            var results = students.Where(x => x.Skills.Any(x => skills.Contains(x))).ToList();
            Assert.True(results.Count == 2);
        }

        [Fact]
        public void WhereArrayContainsAny_InlineList()
        {
            var results = students.Where(x => x.Skills.Any(x => new List<string> { "Problem Solver", "Hacker" }.Contains(x))).ToList();
            Assert.True(results.Count == 2);
        }

        [Fact]
        public void WhereArrayContainsAny_MemberArray()
        {
            string[] skills = { "Problem Solver", "Hacker" };
            var results = students.Where(x => x.Skills.Any(x => skills.Contains(x))).ToList();
            Assert.True(results.Count == 2);
        }

        [Fact]
        public void WhereIn_MemberArray()
        {
            string[] lastNames = { "Teja" };
            var results = students.Where(x => lastNames.Contains(x.LastName)).ToList();
            Assert.True(results.Count == 2);
        }

        [Fact]
        public void WhereIn_MemberList()
        {
            var lastNames = new List<string> { "Teja" };
            var results = students.Where(x => lastNames.Contains(x.LastName)).ToList();
            Assert.True(results.Count == 2);
        }

        [Fact]
        public void WhereIn_InlineList()
        {
            var results = students.Where(x => new List<string> { "Teja" }.Contains(x.LastName)).ToList();
            Assert.True(results.Count == 2);
        }

        [Fact]
        public void WhereNotIn_InlineList()
        {
            var results = students.Where(x => !new List<string> { "Teja" }.Contains(x.LastName)).ToList();
            Assert.True(results.Count == 1);
        }

        [Fact]
        public void WhereNotIn_MemberList()
        {
            var lastNames = new List<string> { "Teja" };
            var results = students.Where(x => !lastNames.Contains(x.LastName)).ToList();
            Assert.True(results.Count == 1);
        }

        [Fact]
        public void WhereNotIn_MemberArray()
        {
            string[] lastNames = { "Teja" };
            var results = students.Where(x => !lastNames.Contains(x.LastName)).ToList();
            Assert.True(results.Count == 1);
        }

        [Fact]
        public void Where_AndAlso_Where()
        {
            var results = students.Where(x => x.Age > 15 && x.Age < 30).ToList();
            Assert.True(results.Count == 1);
        }

        [Fact]
        public void Where_FirestoreMemberComparision()
        {
            var ex = Assert.Throws<NotSupportedException>(() => students.Where(x => x.FirstName != x.LastName).ToList());
            Assert.Equal(Constants.FirestoreDontSupportMsg, ex.Message);
        }
    }
}