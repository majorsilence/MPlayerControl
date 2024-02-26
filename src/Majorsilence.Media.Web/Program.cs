using System.Text;
using Majorsilence.Media.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Majorsilence.Media.Web", Version = "v1" });
});

builder.Services.AddScoped<Settings>(s => { return builder.Configuration.GetSection("ApiSettings").Get<Settings>(); });

var symmetricSecurityKey =
    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ApiSettings:Jwt:Key"]));
builder.Services.AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = builder.Configuration["ApiSettings:Jwt:Issuer"],
            ValidAudience = builder.Configuration["ApiSettings:Jwt:Audience"],
            IssuerSigningKey = symmetricSecurityKey
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("FileUpload", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
    });
    options.DefaultPolicy = options.GetPolicy("FileUpload");
    options.FallbackPolicy = options.DefaultPolicy;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();