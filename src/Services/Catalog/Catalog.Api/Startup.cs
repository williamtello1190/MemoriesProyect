using Catalog.Persistence.Database;
using Catalog.Services.Queries.StoredProcedure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Catalog.Api
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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin",
                builder =>
                {
                    builder.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("Access-Control-Allow-Origin", "*");

                });
            });

            services.AddDbContext<ApplicationDbContext>(opts =>
                opts.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                .MigrationsHistoryTable("__EFMigrationhistory", "Memories"))
            );

            services.AddMediatR(Assembly.Load("Catalog.Services.EventHandlers"));
            services.AddTransient<IGetMemoryPersonQueryService, GetMemoryPersonQueryService>();
            services.AddTransient<IGetMemoryPersonDetailQueryService, GetMemoryPersonDetailQueryService>();
            services.AddTransient<IGetMemoryPersonByUserIdQueryService, GetMemoryPersonByUserIdQueryService>();
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.Api", Version = "v1" });
            });
            services.AddMvc().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = new LowerCaseNamingPolicy());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.Api v1"));
            }

            app.UseRouting();

            app.UseCors(options => options.WithOrigins("http://localhost:4200"));

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public class LowerCaseNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) =>
                name.ToLower();
        }
    }
}
