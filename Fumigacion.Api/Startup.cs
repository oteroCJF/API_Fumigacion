using Fumigacion.Persistence.Database;
using Fumigacion.Service.Queries.Queries.Contratos;
using Fumigacion.Service.Queries.Queries.Convenios;
using Fumigacion.Service.Queries.Queries.Cuestionarios;
using Fumigacion.Service.Queries.Queries.Entregables;
using Fumigacion.Service.Queries.Queries.EntregablesContratacion;
using Fumigacion.Service.Queries.Queries.Facturacion;
using Fumigacion.Service.Queries.Queries.Facturas;
using Fumigacion.Service.Queries.Queries.Firmantes;
using Fumigacion.Service.Queries.Queries.Incidencias;
using Fumigacion.Service.Queries.Queries.LogCedulas;
using Fumigacion.Service.Queries.Queries.LogEntregables;
using Fumigacion.Service.Queries.Queries.ServiciosContrato;
using Fumigacion.Service.Queries.Queries.Variables;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using Fumigacion.Service.Queries.Queries.Flujo;
using Fumigacion.Service.Queries.Queries.CedulasEvaluacion;
using Fumigacion.Service.Queries.Queries.Oficios;

namespace Fumigacion.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(opts => {
                opts.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
               x => x.MigrationsHistoryTable("__EFMigrationHistory", "Fumigacion")
               );
            });

            services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; options.JsonSerializerOptions.PropertyNameCaseInsensitive = false; });

            services.AddMediatR(Assembly.Load("Fumigacion.Service.EventHandler"));

            services.AddTransient<ICuestionariosQueryService, CuestionarioQueryService>();
            services.AddTransient<IVariablesQueryService, VariableQueryService>();
            services.AddTransient<IFlujoCedulaQueryService, FlujoCedulaQueryService>();
            services.AddTransient<IFirmantesQueryService, FirmanteQueryService>();
            services.AddTransient<IRepositorioQueryService, RepositorioQueryService>();
            services.AddTransient<IFacturasQueryService, FacturaQueryService>();
            services.AddTransient<IFumigacionQueryService, FumigacionQueryService>();
            services.AddTransient<IRespuestasQueryService, RespuestaQueryService>();
            services.AddTransient<IContratosQueryService, ContratoQueryService>();
            services.AddTransient<IConvenioQueryService, ConvenioQueryService>();
            services.AddTransient<IServicioContratoQueryService, ServicioContratoQueryService>();
            services.AddTransient<IIncidenciasQueryService, IncidenciaQueryService>();
            services.AddTransient<IEntregableQueryService, EntregableQueryService>();
            services.AddTransient<IEntregableContratacionQueryService, EntregableContratacionQueryService>();
            services.AddTransient<IOficioQueryService, OficioQueryService>();
            services.AddTransient<ILogCedulasQueryService, LogCedulaQueryService>();
            services.AddTransient<ILogEntregablesQueryService, LogEntregableQueryService>();

            services.AddControllers();

            var secretKey = Encoding.ASCII.GetBytes(
               Configuration.GetValue<string>("SecretKey")
           );

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
