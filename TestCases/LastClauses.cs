﻿using FirestoreLINQ;
using System;
using System.Linq;
using Xunit;

namespace TestCases
{
    public class LastClauses : IClassFixture<MyDb>
    {
        private readonly MyDb _db;

        private IQueryable<Student> students => _db.firestoreDb.Collection("Students").AsQuerable<Student>();

        public LastClauses(MyDb db)
        {
            _db = db;
        }

        [Fact]
        public void Last_WithoutOrderBy()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => students.Last());

            Assert.Equal("Queries using LimitToLast must specify at least one ordering.", ex.Message);
        }

        [Fact]
        public void Last_WithOrderBy()
        {
            var results = students.OrderBy(x => x.FirstName).Last();
            var mockResult = MockData.Students.OrderBy(x => x.FirstName).Last();

            Assert.Equal(mockResult, results);
        }

        [Fact]
        public void Last_InsidePredicate_Valid()
        {
            var result = students.OrderBy(x => x.Age).Last(x => x.Age > 15);
            var mockResult = MockData.Students.OrderBy(x => x.Age).Last(x => x.Age > 15);

            Assert.Equal(mockResult, result);
        }

        [Fact]
        public void Last_InsidePredicate_InValid()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => students.OrderBy(x => x.Age).Last(x => x.Age > 100));
            Assert.Equal("Sequence contains no elements", ex.Message);
        }

        [Fact]
        public void LastOrDefault_InsidePredicate_Valid()
        {
            var result = students.OrderBy(x => x.Age).LastOrDefault(x => x.Age > 15);
            var mockResult = MockData.Students.OrderBy(x => x.Age).LastOrDefault(x => x.Age > 15);

            Assert.Equal(mockResult, result);
        }

        [Fact]
        public void LastOrDefault_InsidePredicate_InValid()
        {
            var result = students.OrderBy(x => x.Age).LastOrDefault(x => x.Age > 100);
            var mockResult = MockData.Students.OrderBy(x => x.Age).LastOrDefault(x => x.Age > 100);

            Assert.Equal(mockResult, result);
        }
    }
}