using services;

const string ALLOW_ANY_ORIGIN_POLICY = "AllowAnyOrigin";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<ReportService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(ALLOW_ANY_ORIGIN_POLICY,
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(ALLOW_ANY_ORIGIN_POLICY);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
