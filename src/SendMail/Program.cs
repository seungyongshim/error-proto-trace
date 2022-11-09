using Boost.Proto.Actor.DependencyInjection;
using Boost.Proto.Actor.Hosting.Cluster;
using OpenTelemetry.Trace;
using Ports.Smtp;
using Ports.Smtp.Actors;
using Proto.Cluster.Consul;
using Proto.OpenTelemetry;
using Proto.Router;
using SendMailService.Actors;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseProtoActorCluster((option, sp) =>
{
    option.Name = "test";
    option.Provider = ClusterProviderType.Consul;
    option.SystemShutdownDelaySec = 0;
    option.ClusterKinds.Add(new
    (
        nameof(EmailSagaGrain),
        sp.GetRequiredService<IPropsFactory<EmailSagaGrain>>().Create()
    ));

    option.FuncActorSystemStart = root =>
    {
        var pid = root.SpawnNamed(root.NewRoundRobinPool(sp.GetRequiredService<IPropsFactory<SmtpPortActor>>().Create(), 10), nameof(SmtpPortActor));

        return root;
    };
});

builder.Host.UseSmtp();
builder.Services.AddSingleton<FuncIRootContext>(x => x.WithTracing());
builder.Services.AddSingleton<FuncProps>(x => x.WithTracing());
builder.Services.AddSingleton<ConsulProvider>(sp => new ConsulProvider(new ConsulProviderConfig(), config =>
{
    //var ub = new UriBuilder("http", "consul", 8500);
    //config.Address = ub.Uri;
}));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenTelemetryTracing(trace =>
{
    trace.AddSource("Proto.Actor")
         .AddJaegerExporter(option =>
         {
             //option.Endpoint = new Uri("http://jaeger-all-in-one:14268/api/traces");
         })
         .AddAspNetCoreInstrumentation();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
