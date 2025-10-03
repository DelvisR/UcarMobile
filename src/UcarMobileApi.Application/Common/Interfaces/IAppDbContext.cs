using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UcarMobileApi.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
