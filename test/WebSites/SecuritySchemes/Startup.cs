using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SecuritySchemes.Swagger;
using Swashbuckle.Swagger.Model;
using System.Collections.Generic;

namespace SecuritySchemes
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
        }

        // This method gets called by a runtime.
        // Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNetCore.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
            // services.AddWebApiConventions();

            services.AddSwaggerGen(c =>
                {
                    c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                    {
                        Type = "oauth2",
                        Flow = "implicit",
                        AuthorizationUrl = "https://login.microsoftonline.com/a517fbdd-6b74-48c7-b4eb-2e43fb64a8aa/oauth2/authorize",
                        Scopes = new Dictionary<string, string>
                        {
                            { "read", "read access" },
                            { "write", "write access" }
                        }
                    });

                    c.OperationFilter<AssignSecurityRequirements>();
                }
            );
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            // Configure the HTTP request pipeline.
            app.UseStaticFiles();

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                Authority = "https://login.microsoftonline.com/a517fbdd-6b74-48c7-b4eb-2e43fb64a8aa",
                Audience = "dd38b110-398d-4265-b99f-82004d69d442"
            });

            // Add MVC to the request pipeline.
            app.UseMvc();
            // Add the following route for porting Web API 2 controllers.
            // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");

            app.UseSwagger();
            app.UseSwaggerUi();

            // TOOD: Figure out oauth middleware to validate token
            //app.UseOAuthBearerAuthentication(opts =>
            //{
            //});
        }
    }
}
