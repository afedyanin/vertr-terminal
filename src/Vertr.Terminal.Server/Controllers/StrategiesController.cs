using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vertr.Terminal.ApiClient.Contracts;
using Vertr.Terminal.Domain.Abstractions;
using Vertr.Terminal.Server.Strategies;

namespace Vertr.Terminal.Server.Controllers;

[Route("strategies")]
[ApiController]
public class StrategiesController(
    IMediator mediator,
    IMarketDataRepository marketDataRepository) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IMarketDataRepository _marketDataRepository = marketDataRepository;

    [HttpPost("random-walk")]
    public async Task<IActionResult> RandomWalk(RandomWalkRequest request)
    {
        var strategyParams = new RandomWalkStrategyParams(
            request.UserId,
            request.SymbolId,
            request.BasePrice,
            request.PriceDelta,
            request.OrdersCount);

        var strategy = new RandomWalkStrategy(_mediator, _marketDataRepository, strategyParams);

        await strategy.Execute();
        return Ok();
    }
}
