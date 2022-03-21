using WomanInTechMicroservices.Api;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

Startup.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

Startup.Configure(app);

app.MapGet("/api/hoteis", async (IHospedagemApi api) => await api.Hoteis())
    .WithName("Obter Hoteis")
    .WithTags("01. Hoteis")
    .Produces(200);

app.MapGet("/api/voos", async (IVooApi api) => await api.Voos())
    .WithName("Obter Voos")
    .WithTags("02. Voos")
    .Produces(200);

app.MapPost("/api/comprar", async ([FromBody] PacoteInput pacoteInput, WomanInTechDbCtx ctx, IVooApi vooApi, IHospedagemApi hospedagemApi) =>
    {
        var transacao = await ctx.Database.BeginTransactionAsync();
        try
        {
            var voo = await vooApi.Comprar(pacoteInput.Voo);

            var hotel = await hospedagemApi.Comprar(pacoteInput.Hotel);

            var pacote = new Pacote(Guid.NewGuid(), pacoteInput.Hotel, pacoteInput.Voo);
            ctx.Add(pacote);
            await ctx.SaveChangesAsync();
            await transacao.CommitAsync();
            return Results.Created("/", new { pacote.Id, pacote.HospedagemId, pacote.VooId });
        }
        catch
        {
            await transacao.RollbackAsync();
            return Results.BadRequest(new { Message = "Alguns recursos não foram encontrados" });
        }
    })
    .WithName("Comprar")
    .WithTags("03. Compra")
    .Produces(201);

app.Run();
public record PacoteInput(Guid Hotel, Guid Voo, byte QuantidadeDias);