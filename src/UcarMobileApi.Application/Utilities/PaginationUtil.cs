using Gridify;
using Microsoft.AspNetCore.Http;

//using Microsoft.AspNetCore.Http;

namespace UcarMobileApi.Application.Utilities;

public static class PaginationUtil
{
    private const string XTotalCountHeaderName = "X-Total-Count";

    public static IHeaderDictionary GeneratePaginationHttpHeaders<T>(this Paging<T> page)
        where T : class
    {
        var headers = new HeaderDictionary { { XTotalCountHeaderName, page.Count.ToString() } };

        return headers;
    }

    public static IHeaderDictionary GeneratePaginationHttpHeaders<T>(this QueryablePaging<T> queryablePaging)
        where T : class
    {
        var headers = new HeaderDictionary { { XTotalCountHeaderName, queryablePaging.Count.ToString() } };

        return headers;
    }
}
