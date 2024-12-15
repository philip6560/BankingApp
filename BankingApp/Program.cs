using BankingApp.Data;
using BankingApp.Data.Entities.Common.Enums;
using BankingApp.Data.UnitOfWok.Abstractions;
using BankingApp.Services.Account;
using BankingApp.Services.Account.Abstraction;
using BankingApp.Services.Authentication;
using BankingApp.Services.Authentication.Abstraction;
using BankingApp.Services.Payment;
using BankingApp.Services.Payment.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUnitOfWork>();

builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();

builder.Services.AddDbContext<BankingAppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"),
        options => options.MapEnum<TransactionStatus>());
});

builder.Services.AddControllers();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options => options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecurityKey"]))
    });


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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
