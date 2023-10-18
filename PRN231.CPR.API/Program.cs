using BusinessObjects.DTOs.Response;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using PRN231.CPR.API.Mapper;
using Repository;
using System.Net;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var odata = new ODataConventionModelBuilder();
//odata.EntitySet<Car>("Cars");
odata.EntitySet<AccountResponse>("Accounts");
odata.EntitySet<SubjectResponse>("Subjects");
odata.EntitySet<TopicResponse>("Topics");
odata.EntitySet<SemesterResponse>("Semesters");
odata.EntitySet<StudentInGroupResponse>("StudentInGroups").EntityType.HasKey(x=>x.Id);
odata.EntitySet<TopicOfGroupResponse>("TopicOfGroups").EntityType.HasKey(x => new {x.TopicId,x.GroupProjectId});
;
var issue = odata.EntitySet<GroupProjectResponse>("GroupProjects").EntityType.HasKey(x => x.Id);
issue.HasMany<StudentInGroupResponse>(x => x.StudentInGroups);
issue.HasMany<TopicOfGroupResponse>(x => x.TopicOfGroups);
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
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
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
            ValidateAudience = false,
        };
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

app.MapControllers();

app.Run();
