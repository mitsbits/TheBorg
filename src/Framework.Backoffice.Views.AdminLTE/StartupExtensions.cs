using System.IO;
using System.Reflection;
using Borg.Framework.Backoffice.Views.AdminLTE;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {
        public static RazorViewEngineOptions AddEmbeddedAdminLteViewsForBackOffice(this RazorViewEngineOptions options)
        {
            var p = new EmbeddedFileProvider(
                typeof(AssenblyPointer).GetTypeInfo().Assembly
            );
            options.FileProviders.Add(p);

            return options;
        }

        public static IApplicationBuilder UseEmbeddedAdminLteStaticFilesForBackOffice(this IApplicationBuilder app)
        {
            var p =  
            new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), @"Areas"));
            var r = new PathString("/Areas");
            return app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = p,
                RequestPath = r
            });
        }
    }
}