using AutoMapper;
using Duende.IdentityServer.Extensions;
using ForSight.TimeZonesService.API.Dispatcher;
using ForSight.TimeZonesService.API.Models;
using ForSight.TimeZonesService.API.Routes;
using ForSight.TimeZonesService.Handlers.Query.GetActiveTimeZones;
using ForSight.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ForSight.TimeZonesService.API.Controllers
{
    [Route(RouteNames.TimeZones)]
    [ApiController]
    public class ActiveTimeZonesController : ControllerBase
    {
        private readonly IMapper _autoMapper;
        private readonly IDispatcher _dispatcher;
        private readonly ILogger<ActiveTimeZonesController> _logger;

        public ActiveTimeZonesController(IMapper autoMapper,
            IDispatcher dispatcher,
            ILogger<ActiveTimeZonesController> logger)
        {
            _autoMapper = autoMapper;
            _dispatcher = dispatcher;
            _logger = logger;
        }

        /// <summary>
        /// GET: /api/timezones
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<GetActiveTimeZonesResultsDto>> GetActiveTimeZones()
        {
            _logger.LogInformation("Executing {Action} - userId: {UserId}",
                this.GetExecutingActionName(), User.GetSubjectId());

            var query = new GetActiveTimeZonesQuery();

            var types = await _dispatcher.DispatchQuery<GetActiveTimeZonesQuery, List<GetActiveTimeZonesQueryResult>>(query);

            return _autoMapper.Map<List<GetActiveTimeZonesResultsDto>>(types);
        }
    }
}
