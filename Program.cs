using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using webapi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPaymentService, MockPaymentService>();

// التعديل هنا: خليه يقرأ من الـ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? "Data Source=./resto.db"; // لو ملقتش في الإعدادات، دور عليه في الفولدر الرئيسي

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", b => {
        b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var jwtKey = builder.Configuration["JwtSettings:Key"];

if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
{
    jwtKey = "iloveyouforeveriloveyouforeverireallyloveyou"; 
    Console.WriteLine("Warning: JWT Key is missing, using a temporary key.");
}

var keyBytes = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// التعديل هنا: خليه يفتح Swagger في كل الحالات عشان تعرف تجرب براحتك
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
// امسح الـ HttpsRedirection لو لسه فيه مشاكل في الـ SSL على الاستضافة المجانية
// app.UseHttpsRedirection();

app.UseAuthentication(); // Who are you?
app.UseAuthorization();  // What are you allowed to do?
app.MapControllers();

app.Run();