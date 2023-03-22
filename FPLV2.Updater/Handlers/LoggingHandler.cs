using FPLV2.Database.Repositories.Interfaces;
using System.Diagnostics;

namespace FPLV2.Updater.Handlers;

/// <summary>
/// Used to log all the requests that a HttpClient makes
/// </summary>
public class LoggingHandler : DelegatingHandler
{

    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Constructor for the LoggingHandler
    /// </summary>
    /// <param name="unitOfWork">Repositories to call the database</param>
    public LoggingHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        InnerHandler = new HttpClientHandler();
    }

    /// <summary>
    /// Override the SendAsync method to include logging the http request calls
    /// </summary>
    /// <param name="request">The http request to log</param>
    /// <param name="cancellationToken">Any cancellation token that is used</param>
    /// <returns>The HttpResponseMessage</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        await _unitOfWork.Logs.LogInformation($"Call '{request.RequestUri}' Begins");
        var response = await base.SendAsync(request, cancellationToken);
        stopwatch.Stop();
        await _unitOfWork.Logs.LogInformation($"Call '{request.RequestUri}' Completed in {stopwatch.Elapsed.TotalMilliseconds}ms");

        return response;
    }
}
