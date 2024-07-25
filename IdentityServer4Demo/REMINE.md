Add-Migration InitialIdentityServerPersistedGrantDbMigration -Context PersistedGrantDbContext -Output Migrations/PersistedGrantDb

Update-Database  -Context PersistedGrantDbContext


Add-Migration InitialIdentityServerConfigurationDbMigration -Context ConfigurationDbContext  -Output Migrations/ConfigurationDb

Update-Database  -Context ConfigurationDbContext


Add-Migration ApplicationDbMigration -Context ApplicationDbContext  -Output Migrations/ApplicationDb

Update-Database  -Context ApplicationDbContext

