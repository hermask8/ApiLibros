var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string corsMAM = "_corsMAM";
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddCors(Options =>
{
    Options.AddPolicy(name: corsMAM,
        builder =>
        {
            builder.WithOrigins("http://localhost", "http://localhost:3000", "*").AllowAnyHeader().AllowCredentials().WithMethods("PUT", "DELETE", "GET", "POST");
        });
});

IConfiguration configuration = builder.Configuration;
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseRouting();




app.UseCors(corsMAM);
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

