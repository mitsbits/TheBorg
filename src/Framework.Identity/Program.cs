using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Borg.Framework.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration().UseUrls("https://localhost:44383/")
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}