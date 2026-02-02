using Gridify;

namespace UcarMobileApi.Application.Common.Interfaces;

public interface IGridifyMapperResolver
{
    IGridifyMapper<T> Get<T>();
}
