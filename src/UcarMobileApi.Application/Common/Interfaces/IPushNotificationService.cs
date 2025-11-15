using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UcarMobileApi.Application.Common.Interfaces;

public interface IPushNotificationService
{
    Task<List<string>> SendPushAsync(int userId, string title, string body, CancellationToken ct = default);
    Task CleanInvalidEndpointsAsync(int userId, IEnumerable<string> endpointArns, CancellationToken ct = default);
}
