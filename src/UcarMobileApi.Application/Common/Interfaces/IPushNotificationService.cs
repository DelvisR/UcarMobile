using System.Threading;
using System.Threading.Tasks;

namespace UcarMobileApi.Application.Common.Interfaces;

public interface IPushNotificationService
{
    Task SendPushAsync(int userId, string title, string body, CancellationToken ct = default);
}
