using Microsoft.EntityFrameworkCore;
using webapi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// التعديل هنا: خليه يقرأ من الـ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? "Data Source=Data/resto.db"; // لو ملقتش في الإعدادات، دور عليه في الفولدر الرئيسي

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", b => {
        b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// التعديل هنا: خليه يفتح Swagger في كل الحالات عشان تعرف تجرب براحتك
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
// امسح الـ HttpsRedirection لو لسه فيه مشاكل في الـ SSL على الاستضافة المجانية
// app.UseHttpsRedirection(); 

app.UseAuthorization();
app.MapControllers();

app.Run();