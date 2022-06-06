using Microsoft.EntityFrameworkCore;
using PeliculasWebAPI.Entidades;
using PeliculasWebAPI.Entidades.Configuraciones;
using PeliculasWebAPI.Entidades.Seeding;
using PeliculasWebAPI.Entidades.SinLlaves;
using System.Reflection;

namespace PeliculasWebAPI {
    public class ApplicationDBContext : DbContext {
        
        /* Al usar DbContextOptions podemos usar la inyección de dependencias */
        public ApplicationDBContext(DbContextOptions options) : base(options) {

        }

        /* Sirve para configurar una propiedad manual y no por defecto */
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) {
            configurationBuilder.Properties<DateTime>()
                                .HaveColumnType("date");
        }

        /* API Fluente */
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            /* Implementando la configuracion de la clase Genero */
            /* Aunque este es una forma, pero si tenemos varias configuraciones, sería
             * muchas lineas de código */
            // modelBuilder.ApplyConfiguration(new GeneroConfig());

            /* La forma correcta es por Assembly, el cual scanea en el proyecto
             * todas las configuraciones que heredan de IEntityTypeConfiguration */
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            SeedingModuloConsulta.Seed(modelBuilder);
            SeedingPersonaMensaje.Seed(modelBuilder);

            /* modelBuilder.Entity<Log>()
                        .Property(l => l.Id)
                        .ValueGeneratedNever(); */

            /* Ignorando una clase */
            //modelBuilder.Ignore<Direccion>();

            modelBuilder.Entity<CineSinUbicacion>()
                        .HasNoKey() /* Hace que la entidad no tenga llave primaria */
                        .ToSqlQuery("SELECT Id, Nombre FROM Cines")
                        .ToView(null); /* Evita que se agruege la tabla con el esquema a la BD */

            modelBuilder.Entity<PeliculaConteos>()
                        .HasNoKey()
                        .ToView("PeliculasConteos");

            /* Configuracion para una tipo de dato URL */
            foreach (var tipoEntidad in modelBuilder.Model.GetEntityTypes()) {
                foreach (var prop in tipoEntidad.GetProperties()) { 
                    if(prop.ClrType == typeof(string) && prop.Name.Contains("URL", StringComparison.CurrentCultureIgnoreCase)) {
                        prop.SetIsUnicode(false);
                        prop.SetMaxLength(600);
                    }
                }
            }

            modelBuilder.Entity<Mercancia>().ToTable("Mercancia");
            modelBuilder.Entity<PeliculaAlquilable>().ToTable("PeliculasAlquilables");

            var pelicula1 = new PeliculaAlquilable() { 
                Id         = 1,
                Nombre     = "Dr Strange",
                PeliculaId = 6,
                Precio     = 5.99m
            };

            var mercancia1 = new Mercancia() {
                Id                   = 2,
                DisponibleInventario = true,
                EsRopa               = true,
                Nombre               = "Taza coleccionable",
                Peso                 =  1,
                Volumen              = 1, 
                Precio               = 11
            };

            modelBuilder.Entity<Mercancia>().HasData(mercancia1);
            modelBuilder.Entity<PeliculaAlquilable>().HasData(pelicula1);
        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Cine> Cines { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<CineOferta> CinesOfertas { get; set; }
        public DbSet<SalaCine> SalasCines { get; set; }
        public DbSet<PeliculaActor> PeliculasActores { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<CineSinUbicacion> CineSinUbicacion { get; set; }
        public DbSet<PeliculaConteos> PeliculasConteos { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Mensaje> Mensajes { get; set; }
        public DbSet<CineDetalle> CineDetalle { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Producto> Productos { get; set; }
    }
}
