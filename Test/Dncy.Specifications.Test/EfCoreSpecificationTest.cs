using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dncy.Specifications.EntityFrameworkCore;
using Dncy.Specifications.Extensions;
using Dncy.Specifications.Test.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Dncy.Specifications.Test
{
    public class EfCoreSpecificationTest
    {
        [SetUp]
        public async Task Setup()
        {
            await using var ctx = new EfCoreSpecificationDbContext();
            if (!ctx.Users.Any())
            {
                ctx.Users.AddRange(DataSource);
                await ctx.SaveChangesAsync();
            }
        }

        [Test]
        public async Task DbSetWithSpecificationTest()
        {
            await using var ctx = new EfCoreSpecificationDbContext();
            var res = ctx.Users.WithSpecification(new WithOrder());
            Assert.That(res, Is.Not.Null);
            var count= await res.CountAsync();
            var data = await res.ToListAsync();
            Assert.That(count==10);

            var res2= ctx.Users.WithSpecification(new WithOrder(30));
            var count2 = await res2.CountAsync();
            Assert.That(count2 == 30);

        }


        public class EfCoreSpecificationDbContext:DbContext
        {
            public DbSet<User> Users { get; set; }


            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("SpecificationDb");
            }
        }



        class IdBetweenOneAndTen:Specification<User>
        {
            public IdBetweenOneAndTen()
            {
                Query.Where(x => x.Id >= 1 && x.Id <= 10);
            }
        }


        class WithOrder:Specification<User,UserDto>
        {
            public WithOrder()
            {
                Query.OrderByDescending(x => x.Age).ThenByDescending(x => x.Sort);
                Query.Select(x => new UserDto() {Id = x.Id, Name = x.Name});
                Query.Take(10);
            }

            public WithOrder(int pageSize)
            {
                Query.OrderByDescending(x => x.Age).ThenByDescending(x => x.Sort);
                Query.Select(x => new UserDto() { Id = x.Id, Name = x.Name });
                Query.Take(pageSize);
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