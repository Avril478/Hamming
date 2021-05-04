namespace Monkeys
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
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
        public HomeModule()
        {
            Post("/try", async(req, res) => {
                var genome = await req.Bind<Text> ();
                var genomeText = genome.text;
                WriteLine ($"....POST /try {genomeText}");
                var client = new HttpClient ();
                client.BaseAddress = new Uri ("http://localhost:8091");
                client.DefaultRequestHeaders.Accept.Clear ();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var hrm = await client.PostAsJsonAsync ("/assess", genome);
                hrm.EnsureSuccessStatusCode();
                Number number = await hrm.Content.ReadAsAsync <Number>();
                Console.WriteLine($"...res: {number.number}");
                await res.AsJson(number);
                return;
            });
        }
    }

    public class Text{
        public string text {set; get;
            }
    }

    public class Number{
        public int number {set;get;
            }
    }

}


namespace Monkeys {
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



namespace Monkeys
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class Program
    {
        public static void Main(string[] args)
        {
            var urls = new[] {"http://localhost:8081"};
            
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
