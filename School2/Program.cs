using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using School2.ApplicationServices.Services;
using School2.Core.Domain;
using School2.Core.ServiceInterface;
using School2.Data;

namespace School2;

public class Program
{
    public static void Main( string[] args )
    {
        var builder = WebApplication.CreateBuilder( args );

        if ( OperatingSystem.IsLinux() )
        {
            builder.Configuration.AddJsonFile(
                "appsettings.Linux.json",
                false,
                true
            );
        }

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        //services
        builder.Services.AddScoped<IEmailingServices, EmailingServices>();
        builder.Services.AddScoped<ILanguageCoursesServices, LanguageCoursesServices>();
        builder.Services.AddScoped<IFilesServices, FilesServices>();
        builder.Services.AddScoped<ILecturersServices, LecturersServices>();
        builder.Services.AddScoped<IStudentsServices, StudentsServices>();
        //apiclients

        //dbcontext
        builder.Services.AddDbContext<School2Context>( options =>
            options.UseSqlServer( builder.Configuration.GetConnectionString( "DefaultConnection" ) ) );

        //identity
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>( options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 8;
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes( 5 );
            } )
            .AddEntityFrameworkStores<School2Context>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>( "CustomEmailConfirmation" );

        //auth

        // swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen( options =>
        {
            options.SwaggerDoc( "v1", new OpenApiInfo { Title = "School2 API", Version = "v1" } );
        } );

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if ( !app.Environment.IsDevelopment() )
        {
            app.UseExceptionHandler( "/Home/Error" );
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        else
        {
            app.UseSwagger();
            app.UseSwaggerUI( opts =>
            {
                opts.SwaggerEndpoint( "/swagger/v1/swagger.json", "School2 API" );
                opts.RoutePrefix = "docs";
            } );
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
                "default",
                "{controller=Home}/{action=Index}/{id?}" )
            .WithStaticAssets();

        app.Run();
    }
}