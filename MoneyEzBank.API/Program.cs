using Microsoft.AspNetCore.Mvc;
using MoneyEzBank.API;
using MoneyEzBank.API.Middlewares;
using MoneyEzBank.Services.BusinessModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var basePath = builder.Configuration.GetValue<string>("ApiSettings:BasePath") ?? "/moneyez-bank";

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(m => m.Value.Errors.Count > 0)
            .SelectMany(m => m.Value.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        var response = new BaseResultModel
        {
            Status = StatusCodes.Status400BadRequest,
            Message = string.Join("; ", errors)
        };

        return new BadRequestObjectResult(response);
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddWebAPIService(builder);
builder.Services.AddInfractstructure(builder.Configuration);

var app = builder.Build();

// Configure the base path before other middleware
app.UsePathBase(basePath);
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint($"{basePath}/swagger/v1/swagger.json", "MoneyEzBank API V1");
});

app.UseCors("app-cors");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();
