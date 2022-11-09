using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MimeKit;
using SendMailService.Domain;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Ports.Smtp;

public record SmtpOptions
{
    public SmtpOption Smtp { get; set; }
}

public readonly record struct SmtpOption(string Host, int Port)
{
    
}

public static class Extensions
{
    public static IHostBuilder UseSmtp(this IHostBuilder host, Action<SmtpOptions, IServiceProvider>? action = null)
    {
        action ??= (_, _) => { };

        host.ConfigureServices((ctx, services) =>
        {
            services.AddOptions<SmtpOptions>()
                    .BindConfiguration("")
                    .PostConfigure(action);
        });

        return host;
    }
}
