using BusinessObjects.DTOs;
using BusinessObjects.DTOs.Response;
using Google.Apis.Auth.AspNetCore3;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using MuTote.API.AppStart;
using MuTote.API.Utility;
using Newtonsoft.Json;
using PRN231.CPR.API.Mapper;
using Repository;
using Repository.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var odata = new ODataConventionModelBuilder();
//odata.EntitySet<Car>("Cars");
odata.EntitySet<AccountResponse>("Accounts");
odata.EntitySet<SubjectResponse>("Subjects");
odata.EntitySet<TopicResponse>("Topics");
odata.EntitySet<SemesterResponse>("Semesters");
odata.EntitySet<SpecializationResponse>("Specialization");
odata.EntitySet<TopicView>("TopicView");
;

builder.Services.AddControllers()
    .AddOData(conf =>
    {
        conf.Select().Filter().OrderBy().Expand().AddRouteComponents("odata",odata.GetEdmModel());
        
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISpecializationRepository, SpecializationRepository>();
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<ITopicViewRepository, TopicViewRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IGroupProjectRepository, GroupPojectRepository>();
builder.Services.AddAutoMapper(typeof(Mapping));
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
        "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
        "Example: \"Bearer 12345abdcef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header,
        },
        new List<string>()
        }
    });
});
builder.Services.ConfigureHangfireServices(builder.Configuration);
builder.Services.AddMemoryCache();
//start JWT
var key = builder.Configuration.GetValue<string>("ApiSetting:Secret");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
});
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            RequireSignedTokens = true,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireRole("Admin");
        policy.RequireAssertion(context =>
        {
            var jwtClaimValue = context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (jwtClaimValue != null)
            {
                ICacheService cacheService = new CacheService();
                var cache = cacheService.GetData<string>($"{jwtClaimValue}");
                var utcCreatedDate = long.Parse(context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Iat).Value);
                var expiredDate = DateTimeOffset.FromUnixTimeSeconds(utcCreatedDate).DateTime;
                TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                if (expiredDate < vietnamTime)
                    return false;
                else if (cache != null)
                {
                    return true;
                }
            }
            return false;
        });
    });
    options.AddPolicy("Lecturer", policy => {

        policy.RequireRole("Lecturer");
        policy.RequireAssertion(context =>
        {
            var jwtClaimValue = context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (jwtClaimValue != null)
            {
                ICacheService cacheService = new CacheService();
                var cache = cacheService.GetData<string>($"{jwtClaimValue}");
                var utcCreatedDate = long.Parse(context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Iat).Value);
                var expiredDate = DateTimeOffset.FromUnixTimeSeconds(utcCreatedDate).DateTime;
                if (expiredDate < DateTime.Now)
                    return false;
                if (cache != null)
                {
                    return true;
                }
            }
            return false;
        });
    } );
    options.AddPolicy("Student", policy => {

        policy.RequireRole("Student");
        policy.RequireAssertion(context =>
        {
            var jwtClaimValue = context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (jwtClaimValue != null)
            {
                ICacheService cacheService = new CacheService();
                var cache = cacheService.GetData<string>($"{jwtClaimValue}");
                var utcCreatedDate = long.Parse(context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Iat).Value);
                var expiredDate = DateTimeOffset.FromUnixTimeSeconds(utcCreatedDate).DateTime;
                if (expiredDate < DateTime.Now)
                    return false;
                if (cache != null)
                {
                    return true;
                }
            }
            return false;
        });
    });
});
//end JWT
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
app.UseHangfireDashboard();
app.UseMiddleware(typeof(GlobalErrorHandlingMiddleware));
app.MapControllers();
app.Run();
