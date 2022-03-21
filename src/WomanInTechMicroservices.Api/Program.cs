using Microsoft.AspNetCore.Mvc;
using WomanInTechMicroservices.Api;

var builder = WebApplication.CreateBuilder(args);

Startup.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

Startup.Configure(app);

app.MapGet("/api/voos", (WomanInTechDbCtx ctx) => ctx.Voos.Where(voo => voo.Disponivel).Select(voo => new VooOutput(voo.Id, voo.Compania, voo.Duracao, voo.Conexoes, voo.Preco)))
    .WithName("Obter Voos")
    .WithTags("02. Voos")
    .Produces(200);

app.MapGet("/api/hoteis", (WomanInTechDbCtx ctx) => ctx.Hoteis.Where(hotel => hotel.Disponivel).Select(hotel => new HotelOutput(hotel.Id, hotel.Nome, hotel.Quartos, hotel.Preco)))
    .WithName("Obter Hoteis")
    .WithTags("01. Hoteis")
.Produces(200);

app.MapPost("/api/comprar", async ([FromBody] PacoteInput pacoteInput, WomanInTechDbCtx ctx) =>
    {
        var transacao = await ctx.Database.BeginTransactionAsync();
        try
        {
            var voo = await ctx.Voos.FindAsync(pacoteInput.Voo);
            voo.Disponivel = false;
            await ctx.SaveChangesAsync();

            var hotel = await ctx.Hoteis.FindAsync(pacoteInput.Hotel);
            hotel.Disponivel = false;
            await ctx.SaveChangesAsync();

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

app.MapPut("/api/reset", async (WomanInTechDbCtx ctx) =>
    {
        ctx.Hoteis.ToList().ForEach(x => x.Disponivel = true);
        ctx.Voos.ToList().ForEach(x => x.Disponivel = true);
        await ctx.SaveChangesAsync();
    })
    .WithTags("04. Reset")
    .Produces(200);

app.Run();

public record Duracao(byte Horas, byte Minutos)
{
    public static implicit operator Duracao(TimeSpan timeSpan)
        => new((byte)timeSpan.Hours, (byte)timeSpan.Minutes);
}
public record VooOutput(Guid Id, string Compania, Duracao Duracao, byte Conexoes, decimal Preco);
public record HotelOutput(Guid Id, string Nome, byte Quartos, decimal Preco);
public record PacoteInput(Guid Hotel, Guid Voo, byte QuantidadeDias);