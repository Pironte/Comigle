using Comigle.Data;
using Comigle.Hubs;
using Comigle.Model;
using Comigle.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Banco de dados
var connectionString = builder.Configuration["ConnectionStrings:COMIGLE"];
builder.Services.AddDbContext<ComigleDbContext>(opts =>
{
    opts.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// Configuração do identity
builder.Services
    .AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ComigleDbContext>()
    .AddDefaultTokenProviders();

#endregion

#region Autenticação
var symetricSecurityKey = builder.Configuration["SymmetricSecurityKey"];
if (symetricSecurityKey == null)
    throw new ApplicationException("symetricSecurityKey não informado nos secrets contidos no csproj da aplicação");

builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symetricSecurityKey)),
        ValidateAudience = false,
        ValidateIssuer = false,
        ClockSkew = TimeSpan.Zero
    };
});
#endregion

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://127.0.0.1:5500", "http://localhost:4200") // Substitua com a URL do seu cliente
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Sempre utilizar o Cors antes do MapHub
app.UseCors("AllowSpecificOrigin");

app.MapHub<ChatHub>("/chatHub");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
