# 规约模式 - EfCore组件

支持EntityFrameworkCore的规约评估器。

使用示例：

```csharp
// 1. efcore上下文
public class EfCoreSpecificationDbContext:DbContext
{
    public DbSet<User> Users { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("SpecificationDb");
    }
}


// 2. 创建规约
    // 创建规约，此规约会查询id为[1,10]的数据
class IdBetweenOneAndTen:Specification<User>
{
    public IdBetweenOneAndTen()
    {
        Query.Where(x => x.Id >= 1 && x.Id <= 10);
    }
}

    // 创建规约，此规约会查询id为[1,10]的数据并执行投影到新结果集
class IdBetweenOneAndTenWithProjection:Specification<User,UserDto>
{
    public IdBetweenOneAndTen()
    {
        Query.Where(x => x.Id >= 1 && x.Id <= 10);
        Query.Select(x => new UserDto {Id = x.Id, Name = x.Name});
    }
}


// 3. 由规约创建查询
[Test]
public async Task DbSetWithSpecificationTest()
{
    await using var ctx = new EfCoreSpecificationDbContext();
    var res = ctx.Users.WithSpecification(new WithOrder()); // 使用DbSet扩展进行规约模式，也可以直接使用 EfCoreSpecificationEvaluator.Default.GetQuery
    Assert.That(res, Is.Not.Null);
    var count= await res.CountAsync();
    var data = await res.ToListAsync();
    Assert.That(count==10);
}


```
