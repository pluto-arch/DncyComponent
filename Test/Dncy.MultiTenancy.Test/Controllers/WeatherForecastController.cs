using Dncy.MultiTenancy.Model;
using Microsoft.AspNetCore.Mvc;

namespace Dncy.MultiTenancy.Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ICurrentTenant _currentTenant;

        public WeatherForecastController(ICurrentTenant currentTenant)
        {
            _currentTenant = currentTenant;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public TenantInfo Get()
        {
            return new TenantInfo(_currentTenant.Id,_currentTenant.Name);
        }
    }
}