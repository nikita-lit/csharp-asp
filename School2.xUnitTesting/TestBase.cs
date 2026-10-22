using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using School2.ApplicationServices.Services;
using School2.Core.ServiceInterface;
using School2.Data;
using School2.xUnitTesting.Macros;
using School2.xUnitTesting.Mock;

namespace School2.xUnitTesting;

public abstract class TestBase
{
    protected TestBase()
    {
        var services = new ServiceCollection();
        SetupServices( services );
        serviceProvider = services.BuildServiceProvider();
    }

    protected IServiceProvider serviceProvider { get; set; }

    public virtual void SetupServices( IServiceCollection services )
    {
        services.AddScoped<IHostEnvironment, MockIHostEnvironment>();

        services.AddScoped<ILanguageCoursesServices, LanguageCoursesServices>();
        services.AddScoped<IFilesServices, FilesServices>();
        services.AddScoped<ILecturersServices, LecturersServices>();
        services.AddScoped<IStudentsServices, StudentsServices>();

        services.AddDbContext<School2Context>
        ( x =>
            {
                x.UseInMemoryDatabase( "TEST" );
                x.ConfigureWarnings( w => w.Ignore( InMemoryEventId.TransactionIgnoredWarning ) );
            }
        );
        RegisterMacros( services );
    }

    private void RegisterMacros( IServiceCollection services )
    {
        var macroBaseType = typeof(IMacros);

        var macros = macroBaseType.Assembly.GetTypes()
            .Where( t => macroBaseType.IsAssignableFrom( t )
                         && !t.IsInterface && !t.IsAbstract );
        foreach ( var macro in macros )
            services.AddSingleton( macro );
    }

    public void Dispose()
    {
    }

    protected T Svc<T>()
    {
        return serviceProvider.GetService<T>();
    }
}