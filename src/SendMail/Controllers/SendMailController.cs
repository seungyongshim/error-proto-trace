using System.ComponentModel;
using System.Diagnostics;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Proto.Cluster;
using SendMailService.Actors;
using SendMailService.Domain;
using static SendMailService.Actors.EmailSagaGrain;
using static SendMailService.Controllers.SendMailController.Dto;

namespace SendMailService.Controllers;



[ApiController]
[Route("[controller]")]
public class SendMailController : ControllerBase
{
    static SendMailController()
    {
        TypeAdapterConfig<EmailContext, Email>.NewConfig()
            .MapWith(src => new Email(new(src.Name), new(src.Address)));
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] Dto dto, [FromServices] Cluster cluster, CancellationToken ct)
    {
        var cid = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        var ret = await cluster.RequestAsync<SendMailResult>(ActorPath.EmailSagaGrain(cid), dto.Adapt<SendMail>(), ct);

        return Ok(ret);
    }

    public record Dto
    {
        public IEnumerable<EmailContext> To { get; init; } = Enumerable.Empty<EmailContext>();
        public EmailContext From { get; init; }
        public IEnumerable<EmailContext> Cc { get; init; } = Enumerable.Empty<EmailContext>();
        public IEnumerable<EmailContext> Bcc { get; init; } = Enumerable.Empty<EmailContext>();


        public readonly record struct EmailContext
        (
            [property: DefaultValue("Hong")]
            string Name,
            [property: DefaultValue("Hong@Hong.com")]
            string Address
        )
        {
            public Email ToEmail() => new(new(Name), new(Address));
        };

        public SendMail ToSendMail() => new
        (
            To.Select(_ => _.ToEmail()),
            From.ToEmail(),
            Cc.Select(_ => _.ToEmail()),
            Bcc.Select(_ => _.ToEmail())
        );
    }
}
