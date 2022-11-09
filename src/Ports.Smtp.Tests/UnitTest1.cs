using AutoMapper;
using Mapster;
using MimeKit;
using SendMailService.Domain;

namespace Ports.Smtp.Tests;

public class UnitTest1
{
    static UnitTest1()
    {
        TypeAdapterConfig<Email, MailboxAddress>.NewConfig()
            .MapWith(src => new MailboxAddress(src.Name.Value, src.Address.Value))
            .Compile();
    }

    [Fact]
    public void Test1()
    {
        var sut = new Email(new("Hong"), new("hong@hong.com"));
        var ret = sut.Adapt<Email, MailboxAddress>();

        Assert.Equal(ret.Name, "Hong");
        Assert.Equal(ret.Address, "hong@hong.com");
    }

    [Fact]
    public void Test2()
    {
        var sut = new Email(new("Hong"), new("hong@hong.com"));
        var ret = new MailboxAddress(sut.Name.Value, sut.Address.Value);
    }
}
