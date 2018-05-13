namespace IdentityServer.Migrations.ScopeConfiguration
{
    using IdentityServer3.Core.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<IdentityServer3.EntityFramework.ScopeConfigurationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\ScopeConfiguration";
        }

        protected override void Seed(IdentityServer3.EntityFramework.ScopeConfigurationDbContext context)
        {
            //Providing standard scopes
            foreach (var standardScope in StandardScopes.All)
            {
                if (!context.Scopes.Any(s => s.Name == standardScope.Name))
                    context.Scopes.Add(standardScope.ToEntity());
            }
            if (!context.Scopes.Any(s => s.Name == StandardScopes.Roles.Name))
                context.Scopes.Add(StandardScopes.Roles.ToEntity());
            if (!context.Scopes.Any(s => s.Name == StandardScopes.OfflineAccess.Name))
                context.Scopes.Add(StandardScopes.OfflineAccess.ToEntity());
        }
    }
}
