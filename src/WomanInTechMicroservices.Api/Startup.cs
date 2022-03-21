using Microsoft.EntityFrameworkCore;

namespace WomanInTechMicroservices.Api;

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
    public DbSet<Voo> Voos { get; set; }
    public DbSet<Hotel> Hoteis { get; set; }
    public DbSet<Pacote> Pacotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Voo>()
            .HasData(new Voo(Guid.NewGuid(), "XPTO Airlines", 0, TimeSpan.FromHours(3),  1234.56m));
        
        modelBuilder.Entity<Hotel>()
            .HasData(new Hotel(Guid.NewGuid(), "XPTO Airlines", 2, 654.99m));
    }
}

public class Voo
{
    public Voo(Guid id, string compania, byte conexoes, TimeSpan duracao, decimal preco)
    {
        Id = id;
        Compania = compania;
        Conexoes = conexoes;
        Duracao = duracao;
        Preco = preco;
    }

    public Guid Id { get; set; }
    public string Compania { get; set; }
    public byte Conexoes { get; set; }
    public TimeSpan Duracao { get; set; }
    public decimal Preco { get; set; }
    public bool Disponivel { get; set; } = true;
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