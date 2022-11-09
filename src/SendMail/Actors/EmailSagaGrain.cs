using Mapster;
using Ports.Smtp.Actors;
using Proto;
using Proto.Cluster;
using SendMailService.Domain;

namespace SendMailService.Actors;

public class EmailSagaGrain : IActor
{
    public Task ReceiveAsync(IContext context) => context.Message switch
    {
        SendMail msg => Task.Run(async () =>
        {
            var ret = await context.RequestAsync<SmtpPortActor.SendMailResult>(ActorPath.SmtpPortActorPid, msg.Adapt<SmtpPortActor.SendMail>() with
            {
                Cid = context.ClusterIdentity().Identity
            });
            context.Respond(new SendMailResult());
        }),
        _ => Task.CompletedTask
    };

    public record SendMail
    (
        IEnumerable<Email> To,
        Email From,
        IEnumerable<Email> Cc,
        IEnumerable<Email> Bcc
    );

    public record SendMailResult
    (
    );
}
