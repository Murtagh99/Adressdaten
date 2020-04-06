using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Adressdaten.Models;
using Newtonsoft.Json;
using Adressdaten.Imports;

namespace Adressdaten
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddDbContext<AdressdatenContext>(opt =>
        //        opt.UseInMemoryDatabase("AdressdatenList"));
        //    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(o => { o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; });
        //}

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AdressdatenContext>(o => o.UseSqlite(Configuration.GetConnectionString("AdressDb")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(o => { o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc();

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var dbContext = scope.ServiceProvider.GetService<AdressdatenContext>())
            //{
            //    context.Database.EnsureCreated();
            //}
            //using (var dbContext = new AdressdatenContext())
            {
                dbContext.Database.EnsureCreated();
                var importedCities = JsonConvert.DeserializeObject<ImportCity[]>(System.IO.File.ReadAllText("Adressen/Cities.json"));
                if (!dbContext.Cities.Any())
                {
                    dbContext.Cities.AddRange(importedCities.Select(city => new City { PostCode = city.PostCode, Name = city.Name }).ToArray());
                    dbContext.SaveChanges();
                }
                if (!dbContext.Streets.Any())
                {
                    var streetsImport = importedCities.Select(city => city.Streets.Select(street => new Street { PostCodeFK = city.PostCode, Name = street.Name })).SelectMany(i => i);
                    dbContext.Streets.AddRange(streetsImport.ToArray());
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
