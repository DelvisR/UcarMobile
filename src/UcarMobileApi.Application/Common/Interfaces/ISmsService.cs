using System.Threading;
using System.Threading.Tasks;

namespace UcarMobileApi.Application.Common.Interfaces;

public interface ISmsService
{
    Task SendSmsAsync(string to, string message, CancellationToken ct = default);
}
