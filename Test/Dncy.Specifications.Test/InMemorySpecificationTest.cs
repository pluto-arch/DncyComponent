using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Dncy.Specifications.Evaluators;
using Dncy.Specifications.Extensions;
using Dncy.Specifications.Test.Models;
using NUnit.Framework;

namespace Dncy.Specifications.Test
{
    public class InMemorySpecificationTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void EvaluateTest()
        {
            var res = InMemorySpecificationEvaluator.Default.Evaluate<User>(DataSource, new IdBetweenOneAndTen());
            Assert.IsTrue(res.Count()==10);
        }


        [Test]
        public void EvaluateWithResultTest()
        {
            var res = InMemorySpecificationEvaluator.Default.Evaluate<User>(DataSource, new IdBetweenOneAndTenWithDto());
            Assert.IsTrue(res.Count()==10);
        }



        [Test]
        public void EvaluateWithSortTest()
        {
            var dataSource = DataSource.ToImmutableList();
            var target = dataSource.OrderByDescending(x=>x.Age).ThenByDescending(x=>x.Sort).Take(10);
            var res = InMemorySpecificationEvaluator.Default.Evaluate<User>(dataSource, new IdBetweenOneAndTenWithDto(true));
            Assert.IsTrue(res.Count()==10);
            Assert.IsTrue(res.All(x=>target.Contains(x)));
        }



        class IdBetweenOneAndTen:Specification<User,UserDto>
        {
            public IdBetweenOneAndTen()
            {
                Query.Where(x => x.Id >= 1 && x.Id <= 10);
            }
        }

        class IdBetweenOneAndTenWithDto:Specification<User,UserDto>
        {
            public IdBetweenOneAndTenWithDto(bool isOrderby=false)
            {
                if (isOrderby)
                {
                    Query.OrderByDescending(x=>x.Age).ThenByDescending(x=>x.Sort);
                }

                Query.Select(x => new UserDto {Id = x.Id, Name = x.Name});
                Query.Take(10);
            }
        }


        class UserDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }



        private IEnumerable<User> DataSource
        {
            get
            {
                foreach (var index in Enumerable.Range(1,200))
                {
                    yield return new User
                    {
                        Id = index,
                        Name = $"{DateTime.Now.Ticks}_{index}",
                        Age = Random.Shared.Next(1,100),
                        Sort = Random.Shared.Next(1,50)
                    };
                }
            }
        }
    }
}