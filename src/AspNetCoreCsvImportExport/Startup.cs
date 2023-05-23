using AspNetCoreCsvImport.Formatters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using AspNetCoreCDRImport.Repository;

namespace AspNetCoreCsvImport
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var csvFormatterOptions = new CsvFormatterOptions();
            csvFormatterOptions.CsvDelimiter = ",";

            services.AddControllers(options =>
            {
                options.InputFormatters.Add(new CsvInputFormatter(csvFormatterOptions));
                options.FormatterMappings.SetMediaTypeMappingForFormat("csv", MediaTypeHeaderValue.Parse("text/csv"));
            }).AddNewtonsoftJson();

            // Register Entity Framework to save data to sql
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CdrDbContext")));

            // COnfiguring the Repositories to save data to  SQL datasource
      
            services.AddScoped<ISQLRepository, CDRSqlRepository>();

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CSV Test API V1");
            });
        }
    }
}
