namespace QuartzJobHostTest.Services
{
    public class DemoService
    {
        public void OutPutHash()
        {
            Console.WriteLine($"服务hash： {this.GetHashCode()}");
        }
    }
}
