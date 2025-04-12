using Business;
using Data;
using Entity.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar clases de Aprendiz
builder.Services.AddScoped<AprendizData>();
builder.Services.AddScoped<AprendizBusiness>();

// Registrar clases de AprendizProcessInstructor
builder.Services.AddScoped<AprendizProcessInstructorData>();
builder.Services.AddScoped<AprendizProcessInstructorBusiness>();

// Registrar clases de AprendizProgram
builder.Services.AddScoped<AprendizProgramData>();
builder.Services.AddScoped<AprendizProgramBusiness>();

// Registrar clases de Boss
builder.Services.AddScoped<BossData>();
builder.Services.AddScoped<BossBusiness>();

// Registrar clases de Center
builder.Services.AddScoped<CenterData>();
builder.Services.AddScoped<CenterBusiness>();

// Registrar clases de ChangeLog
builder.Services.AddScoped<ChangeLogData>();

// Registrar clases de Concept
builder.Services.AddScoped<ConceptData>();
builder.Services.AddScoped<ConceptBusiness>();

// Registrar clases de Enterprise
builder.Services.AddScoped<EnterpriseData>();
builder.Services.AddScoped<EnterpriseBusiness>();

// Registrar clases de Form
builder.Services.AddScoped<FormData>();
builder.Services.AddScoped<FormBusiness>();

// Registrar clases de FormModule
builder.Services.AddScoped<FormModuleData>();
builder.Services.AddScoped<FormModuleBusiness>();

// Registrar clases de Instructor
builder.Services.AddScoped<InstructorData>();
builder.Services.AddScoped<InstructorBusiness>();

// Registrar clases de Instructor
builder.Services.AddScoped<InstructorProgramData>();
builder.Services.AddScoped<InstructorProgramBusiness>();

// Registrar clases de Module
builder.Services.AddScoped<ModuleData>();
builder.Services.AddScoped<ModuleBusiness>();

// Registrar clases de Person
builder.Services.AddScoped<PersonData>();
builder.Services.AddScoped<PersonBusiness>();

// Registrar clases de Process
builder.Services.AddScoped<ProcessData>();
builder.Services.AddScoped<ProcessBusiness>();

// Registrar clases de Program
builder.Services.AddScoped<ProgramData>();
builder.Services.AddScoped<ProgramBusiness>();

// Registrar clases de Regional
builder.Services.AddScoped<RegionalData>();
builder.Services.AddScoped<RegionalBusiness>();

// Registrar clases de RegisterySofia
builder.Services.AddScoped<RegisterySofiaData>();
builder.Services.AddScoped<RegisterySofiaBusiness>();

// Registrar clases de Rol
builder.Services.AddScoped<RolData>();
builder.Services.AddScoped<RolBusiness>();

// Registrar clases de RolForm
builder.Services.AddScoped<RolFormData>();
builder.Services.AddScoped<RolFormBusiness>();

// Registrar clases de Sede
builder.Services.AddScoped<SedeData>();
builder.Services.AddScoped<SedeBusiness>();

// Registrar clases de State
builder.Services.AddScoped<StateData>();
builder.Services.AddScoped<StateBusiness>();

// Registrar clases de TypeModality
builder.Services.AddScoped<TypeModalityData>();
builder.Services.AddScoped<TypeModalityBusiness>();

// Registrar clases de User
builder.Services.AddScoped<UserData>();
builder.Services.AddScoped<UserBusiness>();

// Registrar clases de UserRol
builder.Services.AddScoped<UserRolData>();
builder.Services.AddScoped<UserRolBusiness>();

// Registrar clases de UserSede
builder.Services.AddScoped<UserSedeData>();
builder.Services.AddScoped<UserSedeBusiness>();

// Registrar clases de Verification
builder.Services.AddScoped<VerificationData>();
builder.Services.AddScoped<VerificationBusiness>();



// Agregar CORS 
//mecanismo de seguridad que permite o restringe las solicitudes de recursos que se originan desde un dominio diferente al del servidor.
var OrigenesPermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")!.Split(",");
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(politica =>
    {
        politica.WithOrigins(OrigenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });
});

// Agregar DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=localhost"));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();