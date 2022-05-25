# 规约模式基础组件

默认有InMemory规约评估器，也就是内存中IQuerable对象的规约化操作。

使用示例：

```csharp
// 1. 数据源
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
var res = InMemorySpecificationEvaluator.Default.Evaluate<User>(DataSource, new IdBetweenOneAndTen());
Assert.IsTrue(res.Count()==10);

var res = InMemorySpecificationEvaluator.Default.Evaluate<User>(DataSource, new IdBetweenOneAndTenWithProjection());
Assert.IsTrue(res.Count()==10);

```
