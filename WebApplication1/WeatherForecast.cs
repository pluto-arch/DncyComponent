using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System.Transactions;

namespace WebApplication1
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)( TemperatureC / 0.5556 );

        public string? Summary { get; set; }
    }



    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Qunatity { get; set; }
    }


    public class AddProduct
    {
        public string Title { get; set; }
        public int Quantity { get; set; }
    }


    public class GetAllProductsQuery : IRequest<string> {}


    public class GetAllProductsQuery1 : INotification {}


    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, string>
    {
        private readonly ITA _iTA;
        private readonly ILogger<innnn> _LOGGER;
        private readonly IMediator mediator;
        private readonly DemoContext demoContext;

        public GetAllProductsQueryHandler(ITA iTa, ILogger<innnn> logger, IMediator mediator, DemoContext demoContext)
        {
            _iTA = iTa;
            _LOGGER = logger;
            this.mediator = mediator;
            this.demoContext = demoContext;
        }

        public async Task<string> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            _LOGGER.LogInformation($"222  {demoContext.GetHashCode()}");
            await mediator.Publish(new GetAllProductsQuery1(),cancellationToken);
            return await _iTA.Get();
        }
    }


    public class innnn : INotificationHandler<GetAllProductsQuery1>
    {
        private readonly ITA _iTA;
        private readonly ILogger<innnn> _LOGGER;
        private readonly DemoContext demoContext;

        public innnn(ITA iTa, ILogger<innnn> logger, DemoContext demoContext)
        {
            _iTA = iTa;
            _LOGGER = logger;
            this.demoContext = demoContext;
        }

        /// <inheritdoc />
        public Task Handle(GetAllProductsQuery1 notification, CancellationToken cancellationToken)
        {
            _LOGGER.LogInformation($"333  {demoContext.GetHashCode()}");
            return Task.CompletedTask;
        }
    }

    public class ITA
    {
        public async Task<string> Get()
        {
            return await Task.FromResult("123");
        }
    }


    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;


        public TransactionBehavior(ILogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger ?? NullLogger<TransactionBehavior<TRequest, TResponse>>.Instance;
        }

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            TResponse response = default;
            {
                try
                {
                    response = await next();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return response;
        }
    }

    public class DemoContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "BookStoreDb");
        }

        public DbSet<Product> Product { get; set; }
    }
}