using System.Text.Json;
using lib_energy_dao.DBModels;
using Microsoft.EntityFrameworkCore;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(policy =>
{
    policy.AddPolicy(name: MyAllowSpecificOrigins, opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().DisallowCredentials());
});

builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<Energy_DBContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("DBContext")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());
    options.CustomSchemaIds(s => s.FullName!.Replace("+", "."));
});
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseRouting();
// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = "swagger";
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/swagger")
    {
        context.Response.Redirect("/", permanent: true);
        return;
    }
    await next();
});

app.UseHttpsRedirection();


app.UseAuthorization();
app.UseCors(MyAllowSpecificOrigins);
app.MapControllers();

app.Run();