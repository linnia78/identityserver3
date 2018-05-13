using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.Core.Services.InMemory;
using IdentityServer3.EntityFramework;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityServer.Library
{
    public class ServiceFactory
    {
        //Entityframework setup https://identityserver.github.io/Documentation/docsv2/ef/migrations.html
        //
        //Setup
        /*
            //Enable Migrations
            Enable-Migrations -MigrationsDirectory Migrations\ClientConfiguration -ContextTypeName ClientConfigurationDbContext -ContextAssemblyName IdentityServer3.EntityFramework -ConnectionStringName IdentityServerConnectionString -StartUpProjectName IdentityServer -ProjectName IdentityServer
            Enable-Migrations -MigrationsDirectory Migrations\ScopeConfiguration -ContextTypeName ScopeConfigurationDbContext -ContextAssemblyName IdentityServer3.EntityFramework -ConnectionStringName IdentityServerConnectionString -StartUpProjectName IdentityServer -ProjectName IdentityServer
            Enable-Migrations -MigrationsDirectory Migrations\OperationalConfiguration -ContextTypeName OperationalDbContext -ContextAssemblyName IdentityServer3.EntityFramework -ConnectionStringName IdentityServerConnectionString -StartUpProjectName IdentityServer -ProjectName IdentityServer

            //Add Migrations !!Replace $ProjectRootNamespace$ with namespace!!
            Add-Migration -Name InitialCreate -ConfigurationTypeName $ProjectRootNamespace$.Migrations.ScopeConfiguration.Configuration -ConnectionStringName IdentityServerConnectionString -StartUpProjectName IdentityServer -ProjectName IdentityServer
            Add-Migration -Name InitialCreate -ConfigurationTypeName $ProjectRootNamespace$.Migrations.ClientConfiguration.Configuration -ConnectionStringName IdentityServerConnectionString -StartUpProjectName IdentityServer -ProjectName IdentityServer
            Add-Migration -Name InitialCreate -ConfigurationTypeName $ProjectRootNamespace$.Migrations.OperationalConfiguration.Configuration -ConnectionStringName IdentityServerConnectionString -StartUpProjectName IdentityServer -ProjectName IdentityServer

            //Update Database !!Replace $ProjectRootNamespace$ with namespace!!
            Update-Database -ConfigurationTypeName $ProjectRootNamespace$.Migrations.ClientConfiguration.Configuration -ConnectionStringName IdentityServerConnectionString -StartUpProjectName IdentityServer -ProjectName IdentityServer
            Update-Database -ConfigurationTypeName $ProjectRootNamespace$.Migrations.ScopeConfiguration.Configuration -ConnectionStringName IdentityServerConnectionString -StartUpProjectName IdentityServer -ProjectName IdentityServer
            Update-Database -ConfigurationTypeName $ProjectRootNamespace$.Migrations.OperationalConfiguration.Configuration -ConnectionStringName IdentityServerConnectionString -StartUpProjectName IdentityServer -ProjectName IdentityServer
        */
        public static IdentityServerServiceFactory Configure()
        {
            var efConfig = new EntityFrameworkServiceOptions
            {
                ConnectionString = "IdentityServerConnectionString"
            };

            // these two calls just pre-populate the test DB from the in-memory config
            ConfigureClients(Clients.Get(), efConfig);
            ConfigureScopes(Scopes.Get(), efConfig);

            var factory = new IdentityServerServiceFactory();

            // todo: load from db
            factory.UseInMemoryClients(Clients.Get());
            factory.UseInMemoryScopes(Scopes.Get());
            //factory.RegisterConfigurationServices(efConfig);

            factory.RegisterOperationalServices(efConfig);

            factory.ConfigureClientStoreCache();
            factory.ConfigureScopeStoreCache();

            //UserStore setup https://identityserver.github.io/Documentation/docsv2/advanced/userService.html
            // install-package IdentityServer3.AspNetIdentity
            //factory.UseInMemoryUsers(Users.Get());
            factory.ConfigureUserService(IdentityServerSetting.IDENTITY_SERVER_CONNECTION_STRING);
            factory.ConfigureUserServiceCache();

            //factory.CorsPolicyService = new Registration<IdentityServer3.Core.Services.ICorsPolicyService>(new DefaultCorsPolicyService()
            //{
            //    AllowAll = true
            //});
            factory.CorsPolicyService = new Registration<IdentityServer3.Core.Services.ICorsPolicyService>(new InMemoryCorsPolicyService(Clients.Get()));

            return factory;
        }

        public static void ConfigureClients(IEnumerable<Client> clients, EntityFrameworkServiceOptions options)
        {
            using (var db = new ClientConfigurationDbContext(options.ConnectionString, options.Schema))
            {
                if (!db.Clients.Any())
                {
                    foreach (var c in clients)
                    {
                        var e = c.ToEntity();
                        db.Clients.Add(e);
                    }
                    db.SaveChanges();
                }
            }
        }

        public static void ConfigureScopes(IEnumerable<Scope> scopes, EntityFrameworkServiceOptions options)
        {
            using (var db = new ScopeConfigurationDbContext(options.ConnectionString, options.Schema))
            {
                if (!db.Scopes.Any())
                {
                    foreach (var s in scopes)
                    {
                        var e = s.ToEntity();
                        db.Scopes.Add(e);
                    }
                    db.SaveChanges();
                }
            }
        }
    }
}
 