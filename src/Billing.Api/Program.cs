
using Billing.Api.Extensions;
using Billing.Core.Interfaces;
using Billing.Core.Services;

namespace Billing.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.AddLog4Net();

            ConfigureServices(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseLoggingMiddleware();

            app.Run();
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddSingleton<IDataService, MemoryDataService>();

            services.AddScoped<GoogleValidationService>();
            services.AddScoped<IBillTxService, BillTxService>();
            services.AddScoped<IBillService, BillService>();
        }
    }
}
