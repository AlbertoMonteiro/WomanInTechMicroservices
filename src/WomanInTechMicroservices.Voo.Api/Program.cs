using WomanInTechMicroservices.Voo.Api;

var builder = WebApplication.CreateBuilder(args);

Startup.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

Startup.Configure(app);

app.MapGet("/api/voos", (WomanInTechDbCtx ctx) => ctx.Voos.Where(voo => voo.Disponivel).Select(voo => new VooOutput(voo.Id, voo.Compania, voo.Duracao, voo.Conexoes, voo.Preco)))
    .WithName("Obter Voos")
    .WithTags("02. Voos")
    .Produces(200);

app.MapPost("/api/voos/{id}", async (Guid id, WomanInTechDbCtx ctx) =>
    {
        var transacao = await ctx.Database.BeginTransactionAsync();
        try
        {
            var voo = await ctx.Voos.FindAsync(id);
            voo.Disponivel = false;
            await ctx.SaveChangesAsync();
            await transacao.CommitAsync();
            return Results.Created("/", new { id });
        }
        catch
        {
            await transacao.RollbackAsync();
            return Results.NotFound(new { Message = "Voo não encontrada" });
        }
    })
    .WithName("Comprar")
    .WithTags("02. Compra")
    .Produces(201);

app.MapPut("/api/reset", async (WomanInTechDbCtx ctx) =>
    {
        ctx.Voos.ToList().ForEach(x => x.Disponivel = true);
        await ctx.SaveChangesAsync();
    })
    .WithTags("03. Reset")
    .Produces(200);

app.Run();

public record Duracao(byte Horas, byte Minutos)
{
    public static implicit operator Duracao(TimeSpan timeSpan)
        => new((byte)timeSpan.Hours, (byte)timeSpan.Minutes);
}
public record VooOutput(Guid Id, string Compania, Duracao Duracao, byte Conexoes, decimal Preco);