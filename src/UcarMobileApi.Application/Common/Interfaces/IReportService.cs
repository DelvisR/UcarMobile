using System.Collections.Generic;
using System.Threading.Tasks;

namespace UcarMobileApi.Application.Common.Interfaces;

public interface IReportService
{
    Task<byte[]> GeneratePdfAsync<T>(string templateName, T data, string dataSourceName = "Data", IDictionary<string, object>? parameters = null);
    Task<byte[]> GeneratePdfFromTemplateFileAsync<T>(string templateUrl, T data, string dataSourceName = "Data", IDictionary<string, object>? parameters = null);
}

