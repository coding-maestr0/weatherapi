using DataServiceRepository;
using DataServiceRepository.Interfaces;
using Polly.Extensions.Http;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddTransient(typeof(IMemoryCacheService<>), typeof(MemoryCacheService<>));
var baseurl = builder.Configuration["WeatherForecastConfigs:baseurl"];

builder.Services.AddHttpClient("weatherClient", config =>
{
    config.BaseAddress = new Uri(baseurl);
    config.DefaultRequestHeaders.Clear();
}).AddPolicyHandler(GetRetryPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(4, _ => TimeSpan.FromSeconds(3));
}

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.UseCors("corsapp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
