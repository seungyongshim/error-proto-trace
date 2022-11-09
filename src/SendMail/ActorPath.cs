using Ports.Smtp.Actors;
using Proto;
using Proto.Cluster;

namespace SendMailService;

public static class ActorPath
{
    public static ClusterIdentity EmailSagaGrain(string cid) => ClusterIdentity.Create(cid, nameof(EmailSagaGrain));

    public static PID SmtpPortActorPid { get; } = PID.FromAddress(ActorSystem.NoHost, nameof(SmtpPortActor));
}
