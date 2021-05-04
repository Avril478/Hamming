namespace Fitness
{
    using System;
    using Carter;
    using Microsoft.AspNetCore.Http;
    using Carter.ModelBinding;
    using Carter.Request;
    using Carter.Response;
    using System.Linq;
    using System.Collections.Generic;
    using static System.Console;

    public class HomeModule : CarterModule
    {
        static string Target = "";
        public HomeModule() {
            Post("/target", async(req, res) => {
                var t = await req.Bind<Text> ();
                Target = t.text;
                WriteLine ($"....POST /target {Target}");
                return;
            });

            Post("/assess", async(req, res) => {
                var g = await req.Bind<Text> ();
                var genome = g.text;
                WriteLine ($"....POST /assess {genome}");
                var len = Math.Min (Target.Length, genome.Length);
                var h = genome.Select ((x, i) => Convert.ToInt32 (i < len && x != Target[i])) .Sum ();
                
                if (Target.Length > genome.Length){
                    h += (Target.Length - genome.Length );
                }
                else{
                    h += genome.Length -Target.Length;
                }

                Number number = new Number() {number = h};

                await res.AsJson (number);
            });
        }
    }

    public class Text{
        public string text {set; get;}
    }

    public class Number{
        public int number {set; get;}
    }
}


namespace Fitness {
    using Carter;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup {
        public void ConfigureServices (IServiceCollection services) {
            services.AddCarter ();
        }

        public void Configure (IApplicationBuilder app) {
            app.UseRouting ();
            app.UseEndpoints( builder => builder.MapCarter ());
        }
    }
}



namespace Fitness
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class Program
    {
        public static void Main(string[] args)
        {
            var urls = new[] {"http://localhost:8091"};
            
            var host = Host.CreateDefaultBuilder (args)
            
                .ConfigureLogging (logging => {
                    logging
                        .ClearProviders ()
                        .AddConsole ()
                        .AddFilter (level => level >= LogLevel.Warning);
                })
                
                .ConfigureWebHostDefaults (webBuilder => {
                    webBuilder.UseStartup<Startup> ();
                    webBuilder.UseUrls (urls); 
                })
                
                .Build ();
            
            System.Console.WriteLine ($"..... starting on {string.Join (", ", urls)}");            
            host.Run ();
        }
    }
}
