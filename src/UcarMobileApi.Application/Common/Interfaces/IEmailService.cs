using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Core.Enums;

namespace UcarMobileApi.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default);

    Task SendTemplateEmailAsync(string to, EmailTemplate template, string templateDataJson, CancellationToken ct = default);
}
