using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace DatingAPI
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateWebHostBuilder(args).Build().Run();
      //var host = new WebHostBuilder()
      //  .UseKestrel()
      //  .UseContentRoot(Directory.GetCurrentDirectory())
      //  .UseIISIntegration()
      //  .UseStartup<Startup>().Build();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
  }
}
