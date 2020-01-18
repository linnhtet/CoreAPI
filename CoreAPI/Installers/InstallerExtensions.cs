using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace CoreAPI.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallSericesInAssembly(this IServiceCollection services,IConfiguration configuration)
        {
            var installers = typeof(Startup).Assembly.ExportedTypes.Where(s => typeof(IInstaller).IsAssignableFrom(s) &&
            !s.IsInterface && !s.IsAbstract).Select(Activator.CreateInstance).Cast<IInstaller>().ToList();
            installers.ForEach(installer => installer.InstallServices(services, configuration));
        }
    }
}
