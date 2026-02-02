using System.Threading;
using System.Threading.Tasks;

namespace UcarMobileApi.Application.Common.Interfaces;

public interface IStoredFileCleanupService
{
    Task RunCleanupAsync(CancellationToken cancellationToken);
}

