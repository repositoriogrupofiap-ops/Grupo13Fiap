
dotnet ef migrations add UpdateDescriptionMaxLength --project src\Grupo13Fiap.Infrastructure --startup-project src\Grupo13Fiap.WebApi --context DBContextGrupo13Fiap 2>&1

dotnet ef migrations add InitialCreate --project src\Grupo13Fiap.Identity --startup-project src\Grupo13Fiap.WebApi --context IdentityDataContext 2>&1

dotnet ef database update --project src\Grupo13Fiap.Infrastructure --startup-project src\Grupo13Fiap.WebApi --context DBContextGrupo13Fiap 2>&1

dotnet ef database update --project src\Grupo13Fiap.Identity --startup-project src\Grupo13Fiap.WebApi --context IdentityDataContext 2>&1 
