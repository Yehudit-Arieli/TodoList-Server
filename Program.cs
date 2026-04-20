
// using Microsoft.EntityFrameworkCore;
// using TodoApi; 
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
// using System.Text;
// using System.Security.Claims;
// using System.IdentityModel.Tokens.Jwt;
// using Microsoft.OpenApi.Models;
// var builder = WebApplication.CreateBuilder(args);

// var key = Encoding.ASCII.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharactersLong!");

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = new SymmetricSecurityKey(key),
//         ValidateIssuer = false,
//         ValidateAudience = false
//     };
// });

// builder.Services.AddAuthorization();

// // 2. הגדרות DB & CORS
// var connectionString = builder.Configuration["ToDoDB"];
// builder.Services.AddDbContext<ToDoDbContext>(options =>
//     options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAll", policy =>
//         policy.AllowAnyOrigin()
//                .AllowAnyMethod()
//                .AllowAnyHeader());
// });

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         Name = "Authorization",
//         Type = SecuritySchemeType.Http,
//         Scheme = "Bearer",
//         BearerFormat = "JWT",
//         In = ParameterLocation.Header,
//         Description = "Enter your JWT token"
//     });

//     options.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {
//             new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type = ReferenceType.SecurityScheme,
//                     Id = "Bearer"
//                 }
//             },
//             Array.Empty<string>()
//         }
//     });
// });
// var app = builder.Build();

// app.UseSwagger();
// app.UseSwaggerUI();

// app.UseCors("AllowAll"); 

// app.UseAuthentication(); // קודם בודקים מי המשתמש
// app.UseAuthorization();  // אחר כך בודקים אם יש לו הרשאה

// app.MapGet("/items", async (ToDoDbContext db) =>
//     await db.Items.ToListAsync())
// .RequireAuthorization(); // <--- פשוט מוסיפים את זה כאן בסוף הסוגריים

// app.MapPost("/items", async (ToDoDbContext db, Item newItem) =>
// {
//     db.Items.Add(newItem);
//     await db.SaveChangesAsync();
//     return Results.Created($"/items/{newItem.Id}", newItem);
// });

// app.MapPut("/items/{id}", async (ToDoDbContext db, int id, Item inputItem) =>
// {
//     var item = await db.Items.FindAsync(id);
//     if (item is null) return Results.NotFound();
//     item.Name = inputItem.Name;
//     item.IsComplete = inputItem.IsComplete;
//     await db.SaveChangesAsync();
//     return Results.NoContent();
// });

// app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
// {
//     if (await db.Items.FindAsync(id) is Item item)
//     {
//         db.Items.Remove(item);
//         await db.SaveChangesAsync();
//         return Results.Ok(item);
//     }
//     return Results.NotFound();
// });

// // 5. לוגין מתוקן (ללא שגיאות System.SecurityClaims)
// app.MapPost("/login", (User user) => {
//     if (user.Username == "admin" && user.Password == "1234")
//     {
//         var tokenHandler = new JwtSecurityTokenHandler();
//         var tokenDescriptor = new SecurityTokenDescriptor
//         {
//             Subject = new ClaimsIdentity(new[] { new Claim("id", "1"), new Claim(ClaimTypes.Name, user.Username) }),
//             Expires = DateTime.UtcNow.AddDays(7),
//             SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//         };
//         var token = tokenHandler.CreateToken(tokenDescriptor);
//         return Results.Ok(new { token = tokenHandler.WriteToken(token) });
//     }
//     return Results.Unauthorized();
// });

// app.Run();






// (MySQL) הקובץ הזה בונה שרת לניהול משימות מאובטח, שמחבר בין מסד נתונים 
// לממשק משתמש
//  (Token) ומוודא שרק משתמשים מזוהים עם "מפתח דיגיטלי" 
// יוכלו לגשת למידע ולנהל אותו.
using Microsoft.EntityFrameworkCore;
using TodoApi; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. הגדרות אבטחה (Authentication & Authorization) ---

// הגדרת מפתח הצפנה סודי לחתימת הטוקנים (חייב להיות ארוך ומאובטח)
var key = Encoding.ASCII.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharactersLong!");

// הגדרת שירות האימות: אומרים לשרת להשתמש בטוקן מסוג JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // הגדרת פרמטרים לבדיקת הטוקן: אימות המפתח הסודי והתעלמות מכתובת השרת (לצורך פיתוח)
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// הפעלת שירות ההרשאות (Authorization) במערכת
builder.Services.AddAuthorization();

// --- 2. הגדרות מסד נתונים וגישה חיצונית ---

// שליבת מחרוזת החיבור מהקונפיגורציה וחיבור למסד נתונים MySQL
var connectionString = builder.Configuration["ToDoDB"];
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// // הגדרת CORS: מאפשר לאתרים חיצוניים (כמו ה-Frontend) לשלוח בקשות ל-API הזה
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAll", policy =>
//     {
//         policy.WithOrigins("https://todolist-client-zyi1.onrender.com")
//               .AllowAnyMethod()
//               .AllowAnyHeader()
//               .AllowCredentials();
//     });
// });

// --- 3. הגדרות Swagger (תיעוד ה-API) ---

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // הוספת אפשרות להכנסת טוקן (Authorize) בממשק ה-Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    // הגדרה שכל פעולה ב-Swagger תדרוש את הטוקן שהגדרנו למעלה
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// --- 4. הגדרת הצינורות (Middleware) ---

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll"); // הפעלת מדיניות ה-CORS

app.UseAuthentication(); // בדיקה: מי המשתמש? (האם הטוקן תקין)
app.UseAuthorization();  // בדיקה: האם למשתמש הזה מותר לבצע את הפעולה?

// --- 5. נתיבי ה-API (Endpoints) ---

// שליפת כל המשימות - דורש אימות (Token)
app.MapGet("/items", async (ToDoDbContext db) =>
    await db.Items.ToListAsync())
.RequireAuthorization(); 

// הוספת משימה חדשה למסד הנתונים
app.MapPost("/items", async (ToDoDbContext db, Item newItem) =>
{
    db.Items.Add(newItem);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{newItem.Id}", newItem);
});

// עדכון משימה קיימת לפי מזהה (ID)
app.MapPut("/items/{id}", async (ToDoDbContext db, int id, Item inputItem) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();
    item.Name = inputItem.Name;
    item.IsComplete = inputItem.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// מחיקת משימה מהמסד
app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
{
    if (await db.Items.FindAsync(id) is Item item)
    {
        db.Items.Remove(item);
        await db.SaveChangesAsync();
        return Results.Ok(item);
    }
    return Results.NotFound();
});

// --- 6. יצירת הטוקן (Login) ---

// נתיב התחברות: בודק שם וסיסמה ומנפיק כרטיס כניסה (JWT Token) חתום
app.MapPost("/login", (User user) => {
    if (user.Username == "admin" && user.Password == "1234")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            // המידע שנשמר בתוך הטוקן (שם המשתמש ומזהה)
            Subject = new ClaimsIdentity(new[] { new Claim("id", "1"), new Claim(ClaimTypes.Name, user.Username) }),
            Expires = DateTime.UtcNow.AddDays(7), // תוקף הטוקן לשבוע
            // חתימת הטוקן באמצעות המפתח הסודי שלנו
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Results.Ok(new { token = tokenHandler.WriteToken(token) });
    }
    return Results.Unauthorized();
});

app.Run();
