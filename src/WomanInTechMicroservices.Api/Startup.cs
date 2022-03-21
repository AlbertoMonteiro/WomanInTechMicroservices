using Microsoft.EntityFrameworkCore;
using Refit;

namespace WomanInTechMicroservices.Api;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddDbContext<WomanInTechDbCtx>(x => x.UseSqlServer(configuration["ConnectionStrings:Database"]));

        services.AddRefitClient<IVooApi>()
            .ConfigureHttpClient(x => x.BaseAddress = new Uri(configuration["Services:Voo"]));
        services.AddRefitClient<IHospedagemApi>()
            .ConfigureHttpClient(x => x.BaseAddress = new Uri(configuration["Services:Hospedagem"]));
    }

    internal static void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        using var scope = app.Services.CreateScope();
        using var ctx = scope.ServiceProvider.GetRequiredService<WomanInTechDbCtx>();
        ctx.Database.EnsureCreated();
    }
}

public class WomanInTechDbCtx : DbContext
{
    public WomanInTechDbCtx(DbContextOptions<WomanInTechDbCtx> options)
        : base(options)
    {

    }
    public DbSet<Pacote> Pacotes { get; set; }
}

public class Pacote
{
    public Pacote(Guid id, Guid hospedagemId, Guid vooId)
    {
        Id = id;
        HospedagemId = hospedagemId;
        VooId = vooId;
    }

    public Guid Id { get; set; }
    public Guid HospedagemId { get; set; }
    public Guid VooId { get; set; }
}

public interface IVooApi
{
    [Get("/api/voos/")]
    Task<VooOutput[]> Voos();

    [Post("/api/voos/{id}")]
    Task<IApiResponse> Comprar(Guid id);

    public record Duracao(byte Horas, byte Minutos)
    {
        public static implicit operator Duracao(TimeSpan timeSpan)
            => new((byte)timeSpan.Hours, (byte)timeSpan.Minutes);
    }
    public record VooOutput(Guid Id, string Compania, Duracao Duracao, byte Conexoes, decimal Preco);
}

public interface IHospedagemApi
{
    [Get("/api/hoteis/")]
    Task<HotelOutput[]> Hoteis();

    [Post("/api/hoteis/{id}")]
    Task<IApiResponse> Comprar(Guid id);

    public record HotelOutput(Guid Id, string Nome, byte Quartos, decimal Preco);
}