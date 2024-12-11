using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CrossSolver.API.Data;

namespace CrossSolver.API {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<CrossSolverAPIContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("CrossSolverAPIContext") ?? throw new InvalidOperationException("Connection string 'CrossSolverAPIContext' not found.")));

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.WebHost.UseKestrel();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
