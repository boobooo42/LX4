## Database
The database can always be improved. It is doubtful that our initial database
schema will be perfect.

### Database Migration
This is not a priority early on in development, but it would be nice to be able
to support database migration. [This article by Fredrik
Normen](https://weblogs.asp.net/fredriknormen/using-entity-framework-4-3-database-migration-for-any-project)
demonstrates how to use Entity Framework to do database migration, without the
ORM functionality. Some of his instructions will need to be adapted to dotnet
core; the project management console has been replaced with the Nuget package
management console and all Entity Framework commands take the form "dotnet ef".
