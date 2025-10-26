var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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
app.UseAuthorization();
app.MapControllers();

app.Run();

// Now listening on: https://localhost:64381
// Now listening on: http://localhost:5000
