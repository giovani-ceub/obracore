using FastEndpoints;
using ProjectTemplate.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using static FastEndpoints.Ep;

namespace ProjectTemplate.Api.Endpoints.WeatherForecasts
{
    public class GetWeatherForecastsEndpoint : EndpointWithoutRequest<IEnumerable<WeatherForecast>>
    {
        private ISender _sender;

        public GetWeatherForecastsEndpoint(ISender sender)
        {
            _sender = sender;
        }

        public override void Configure()
        {
            Get("");
            Group<WeatherForecastsEndpointGroup>();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = await _sender.Send(new GetWeatherForecastsQuery());
            await Send.OkAsync(result, ct);            
        }
    }
}
