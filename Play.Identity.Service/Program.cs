using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Play.Common.Settings;
using Play.Identity.Service.Areas.Identity.Entites;
using Play.Identity.Service.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
var mongoSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<ApplicationRole>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
        mongoSettings.ConnectionString,
        serviceSettings.ServiceName
    );

//setup identity server
var idSettings = builder.Configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();

builder.Services.AddIdentityServer(options => {
        options.Events.RaiseSuccessEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseErrorEvents = true;
    })
    .AddAspNetIdentity<ApplicationUser>()
    .AddInMemoryApiScopes(idSettings.ApiScopes)
    .AddInMemoryApiResources(idSettings.ApiResources)
    .AddInMemoryClients(idSettings.Clients)
    .AddInMemoryIdentityResources(idSettings.Resources)
    .AddDeveloperSigningCredential();

//Protecting itself
builder.Services.AddLocalApiAuthentication();

builder.Services.AddControllers();
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

app.UseHttpsRedirection();

app.UseStaticFiles();


app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//need to add this
app.MapRazorPages();

app.Run();
