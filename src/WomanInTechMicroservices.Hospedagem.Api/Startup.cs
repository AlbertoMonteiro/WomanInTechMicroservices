using Microsoft.EntityFrameworkCore;

namespace WomanInTechMicroservices.Hospedagem.Api;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddDbContext<WomanInTechDbCtx>(x => x.UseSqlServer(configuration["ConnectionStrings:Database"]));
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
        :base(options)
    {

    }
    public DbSet<Hotel> Hoteis { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hotel>()
            .HasData(new Hotel(Guid.NewGuid(), "XPTO Airlines", 2, 654.99m));
    }
}

public class Hotel
{
    public Hotel(Guid id, string nome, byte quartos, decimal preco)
    {
        Id = id;
        Nome = nome;
        Quartos = quartos;
        Preco = preco;
    }

    public Guid Id { get; set; }
    public string Nome { get; set; }
    public byte Quartos { get; set; }
    public decimal Preco { get; set; }
    public bool Disponivel { get; set; } = true;
}
