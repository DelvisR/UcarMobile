using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FastReport;
using FastReport.Export.PdfSimple;
using UcarMobileApi.Application.Common.Interfaces;

namespace UcarMobileApi.Infrastructure.Reports;

public sealed class FastReportService : IReportService
{
    private const string TemplateNamespace = "UcarMobileApi.Infrastructure.Reports.Templates";

    public Task<byte[]> GeneratePdfAsync<T>(string templateName, T data, string dataSourceName = "Data", IDictionary<string, object>? parameters = null)
    {
        var templateStream = GetTemplateStream(templateName);

        using var report = new Report();
        report.Load(templateStream);

        // Register main model
        report.RegisterData(new[] { data! }, dataSourceName);

        // Register optional parameters
        if (parameters != null)
        {
            foreach (var param in parameters)
                report.SetParameterValue(param.Key, param.Value);
        }

        report.Prepare();

        using var output = new MemoryStream();
        using var pdfExport = new PDFSimpleExport();

        report.Export(pdfExport, output);

        return Task.FromResult(output.ToArray());
    }

    private static Stream GetTemplateStream(string templateName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{TemplateNamespace}.{templateName}.frx";

        var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
            throw new FileNotFoundException(
                $"The embedded template '{resourceName}' was not found. " +
                $"Verify the name, namespace, and Build Action.");

        return stream;
    }

    public Task<byte[]> GeneratePdfFromTemplateFileAsync<T>(string templateUrl, T data, string dataSourceName = "Data", IDictionary<string, object>? parameters = null)
    {
        using var report = new Report();
        report.Load(templateUrl);

        // Register main model
        report.RegisterData(new[] { data! }, dataSourceName);

        // Register optional parameters
        if (parameters != null)
        {
            foreach (var param in parameters)
                report.SetParameterValue(param.Key, param.Value);
        }

        report.Prepare();

        using var output = new MemoryStream();
        using var pdfExport = new PDFSimpleExport();

        report.Export(pdfExport, output);

        return Task.FromResult(output.ToArray());
    }
}
