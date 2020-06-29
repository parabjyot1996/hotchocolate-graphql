using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GraphQLServer.Repository;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Voyager;
using HotChocolate.Execution.Batching;
using HotChocolate.Execution.Configuration;
using HotChocolateGraphQL.Contracts;
using HotChocolateGraphQL.Entities.Context;
using HotChocolateGraphQL.GraphQL.Mutations;
using HotChocolateGraphQL.GraphQL.Queries;
using HotChocolateGraphQL.GraphQL.Subscriptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HotChocolateGraphQL
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            //Okta OAuth authentication
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
              options.Authority = "https://dev-478144.okta.com/oauth2/default";
              options.Audience = "api://default";
              options.RequireHttpsMetadata = false;
            });
            //End

            //Authorization
            services.AddAuthorization();
            //End

            //Register database provider with connection string
            services.AddDbContextPool<ApplicationDbContext>(options => {
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            });

            //Register dependency
            services.AddScoped<IOwnerRepository, OwnerRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            services.AddInMemorySubscriptions();

            //Create schema using SchemaBuilder
            services
                .AddDataLoaderRegistry()
                .AddGraphQL(
                    SchemaBuilder.New()
                    .AddDirectiveType<ExportDirectiveType>()
                    .AddAuthorizeDirectiveType()
                    .AddQueryType(d => d.Name("Query"))
                    .AddType<OwnerQueries>()
                    .AddType<AccountQueries>()
                    .AddMutationType(d => d.Name("Mutation"))
                    .AddType<OwnerMutations>()
                    .AddType<AccountMutations>()
                    .AddSubscriptionType(d => d.Name("Subscription"))
                    .AddType<AccountSubscriptions>()
                    .Create(),
                    new QueryExecutionOptions { ForceSerialExecution = true });                    
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseWebSockets();

            app.UseGraphQL()
                .UsePlayground()
                .UseVoyager();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}