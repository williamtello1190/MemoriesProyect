using Catalog.Services.EventHandlers.Commands.StoredProcedure;
using Catalog.Services.Queries.DTOs;
using Catalog.Services.Queries.StoredProcedure;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service.Common.Collection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("catalog")]
    [EnableCors("AllowOrigin")]
    public class MemoryPersonController : ControllerBase
    {
        private readonly ILogger<MemoryPersonController> _logger;
        private readonly IMediator _mediator;
        private readonly string routeRoot;
        private readonly string _defaultConnection;
        private IConfiguration _configuration { get; }
        private readonly IGetMemoryPersonQueryService _IGetMemoryPersonQueryService;
        private readonly IGetMemoryPersonDetailQueryService _IGetMemoryPersonDetailQueryService;

        public MemoryPersonController(
          ILogger<MemoryPersonController> logger,
          IMediator mediator,
          IConfiguration configuration,
          IGetMemoryPersonQueryService IGetMemoryPersonQueryService,
          IGetMemoryPersonDetailQueryService IGetMemoryPersonDetailQueryService
          )
        {
            _logger = logger;
            _mediator = mediator;
            _configuration = configuration;
            routeRoot = _configuration.GetSection("ConfigDocument").GetSection("RouteRoot").Value;
            _defaultConnection = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            _IGetMemoryPersonQueryService = IGetMemoryPersonQueryService;
            _IGetMemoryPersonDetailQueryService = IGetMemoryPersonDetailQueryService;
        }

        [HttpGet("memoryPerson/{MemoryPersonId}")]
        public async Task<MemoryPersonDto> GetMemoryPerson(Int32 MemoryPersonId)
        {
            return await _IGetMemoryPersonQueryService.GetMemoryPerson(MemoryPersonId);
        }

        [HttpGet("memoryPersonDetail/{MemoryPersonId}")]
        public async Task<List<MemoryPersonDetailDto>>GetMemoryPersonDetail(Int32 MemoryPersonId)
        {
            return await _IGetMemoryPersonDetailQueryService.GetMemoryPersonDetail(MemoryPersonId);
        }

        [HttpPost("createMemoryPerson")]
        public async Task<DataResponse> CreateMemoryPerson(MemoryPersonCreateCommand comand)
        {
            return await _mediator.Send(comand);
        }
    }
}
