using WomanInTechMicroservices.Hospedagem.Api;

var builder = WebApplication.CreateBuilder(args);

Startup.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

Startup.Configure(app);

app.MapGet("/api/hoteis", (WomanInTechDbCtx ctx) => ctx.Hoteis.Where(hotel => hotel.Disponivel).Select(hotel => new HotelOutput(hotel.Id, hotel.Nome, hotel.Quartos, hotel.Preco)))
    .WithName("Obter Hoteis")
    .WithTags("01. Hoteis")
    .Produces(200);

app.MapPost("/api/hoteis/{id}", async (Guid id, WomanInTechDbCtx ctx) =>
    {
        var transacao = await ctx.Database.BeginTransactionAsync();
        try
        {
            var hotel = await ctx.Hoteis.FindAsync(id);
            hotel.Disponivel = false;
            await ctx.SaveChangesAsync();
            await transacao.CommitAsync();
            return Results.Created("/", new { id });
        }
        catch
        {
            await transacao.RollbackAsync();
            return Results.NotFound(new { Message = "Hospedagem não encontrada" });
        }
    })
    .WithName("Comprar")
    .WithTags("02. Compra")
    .Produces(201);

app.MapPut("/api/reset", async (WomanInTechDbCtx ctx) =>
    {
        ctx.Hoteis.ToList().ForEach(x => x.Disponivel = true);
        await ctx.SaveChangesAsync();
    })
    .WithTags("03. Reset")
    .Produces(200);

app.Run();

public record HotelOutput(Guid Id, string Nome, byte Quartos, decimal Preco);