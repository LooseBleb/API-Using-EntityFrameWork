using ControleFinanceiro.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using ControleFinanceiro.DAL.Interfaces;
using ControleFinanceiro.DAL.Repositories;
using FluentValidation;
using ControleFinanceiro.BLL.Models;
using ControleFinanceiro_API.Validation;
using FluentValidation.AspNetCore;
using ControleFinanceiro_API.ViewModels;
using ControleFinanceiro_API.Extensions;
using System.Text;
using ControleFinanceiro_API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors();

builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AcessDBString")));

builder.Services.AddIdentity<User, Function>().AddEntityFrameworkStores<Context>();
builder.Services.ConfigPasswordUser();

builder.Services.AddScoped<DbContext, Context>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITipoRepository, TipoRepository>();
builder.Services.AddScoped<IFunctionRepository, FunctionRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();


builder.Services.AddTransient<IValidator<Category>, CategoryValidator>();
builder.Services.AddTransient<IValidator<FunctionsViewModel>, FunctionValidator>();
builder.Services.AddTransient<IValidator<RegisterVM>, RegisterVMValidator>();
builder.Services.AddTransient<IValidator<LoginVM>, LoginVMValidator>();


// Add services to the container.

builder.Services.AddSpaStaticFiles(diretorio =>
{
    diretorio.RootPath = "ControleFinanceiro-UI";
});

var key = Encoding.ASCII.GetBytes(Settings.KeySecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddControllers()
    .AddFluentValidation()
    .AddJsonOptions(opcoes =>
    {
        opcoes.JsonSerializerOptions.IgnoreNullValues = true;
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(opcoes => opcoes.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapControllers();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseHttpsRedirection();

app.UseSpaStaticFiles();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = Path.Combine(Directory.GetCurrentDirectory(), @"C:\Users\Yago Felipe\Desktop\CotroleFinanceiro\ControleFinanceiro-UI");
    if (app.Environment.IsDevelopment())
    {
        spa.UseAngularCliServer(npmScript: "start");
        spa.UseProxyToSpaDevelopmentServer($"http://localhost:4200/");
    }
});


app.Run();
