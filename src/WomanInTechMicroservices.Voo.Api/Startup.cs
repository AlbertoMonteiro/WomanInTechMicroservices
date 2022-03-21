using Microsoft.EntityFrameworkCore;

namespace WomanInTechMicroservices.Voo.Api;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Voo>()
            .HasData(new Voo(Guid.NewGuid(), "XPTO Airlines", 0, TimeSpan.FromHours(3),  1234.56m));
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