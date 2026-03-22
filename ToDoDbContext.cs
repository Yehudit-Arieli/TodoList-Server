
// // using System;
// // using System.Collections.Generic;
// // using Microsoft.EntityFrameworkCore;

// // namespace TodoApi;

// // public partial class ToDoDbContext : DbContext
// // {
// //     public ToDoDbContext()
// //     {
// //     }

// //     public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
// //         : base(options)
// //     {
// //     }

// //     public virtual DbSet<Item> Items { get; set; }
    
// //     // השורה הועברה לכאן - לתוך המחלקה!
// //     public virtual DbSet<User> Users { get; set; } 

// //     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// //         => optionsBuilder.UseMySql("name=ToDoDB", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.45-mysql"));

// //     protected override void OnModelCreating(ModelBuilder modelBuilder)
// //     {
// //         modelBuilder
// //             .UseCollation("utf8mb4_0900_ai_ci")
// //             .HasCharSet("utf8mb4");

// //         modelBuilder.Entity<Item>(entity =>
// //         {
// //             entity.HasKey(e => e.Id).HasName("PRIMARY");
// //             entity.ToTable("items");
// //             entity.Property(e => e.Name).HasMaxLength(100);
// //         });

// //         // הגדרה בסיסית לטבלת המשתמשים
// //         modelBuilder.Entity<User>(entity =>
// //         {
// //             entity.HasKey(e => e.Id).HasName("PRIMARY");
// //             entity.ToTable("Users"); // ודאי שזה השם ב-MySQL Workbench
// //         });

// //         OnModelCreatingPartial(modelBuilder);
// //     }

// //     partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
// // }



// // //(הספרייה שמנהלת את מסד הנתונים) Entity Framework  זהו קובץ ההגדרות של 
// // // ( Item ו-User כמו) C# והוא מתרגם את המחלקות שכתבת ב-
// // //  MySQL -לטבלאות אמיתיות ב
// // // --- הגדרת ה"מגירות" בתוך מסד הנתונים (DbSets) ---

// // // יצירת טבלה בשם Items שתשמור את כל נתוני המשימות
// namespace TodoApi;

// public partial class ToDoDbContext : DbContext
// {
//     // --- הגדרת ה"מגירות" בתוך מסד הנתונים ---
    
//     // יצירת טבלה בשם Items שתשמור את כל נתוני המשימות
//     public virtual DbSet<Item> Items { get; set; }

//     // יצירת טבלה בשם Users שתשמור את פרטי המשתמשים
//     public virtual DbSet<User> Users { get; set; } 

//     // --- הגדרת החיבור למסד הנתונים ---
//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//         => optionsBuilder.UseMySql("name=ToDoDB", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.45-mysql"));

//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         // הגדרת קידוד תווים לתמיכה בעברית
//         modelBuilder
//             .UseCollation("utf8mb4_0900_ai_ci")
//             .HasCharSet("utf8mb4");

//         // הגדרות לטבלת המשימות
//         modelBuilder.Entity<Item>(entity =>
//         {
//             entity.HasKey(e => e.Id).HasName("PRIMARY");
//             entity.ToTable("items");
//             entity.Property(e => e.Name).HasMaxLength(100);
//         });

//         // הגדרות לטבלת המשתמשים
//         modelBuilder.Entity<User>(entity =>
//         {
//             entity.HasKey(e => e.Id).HasName("PRIMARY");
//             entity.ToTable("Users"); 
//         });

//         OnModelCreatingPartial(modelBuilder);
//     }

//     partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
// } // <--- הכל חייב להיגמר כאן! שום דבר לא יכול להופיע אחרי הסוגר הזה.






using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TodoApi;

public partial class ToDoDbContext : DbContext
{
    public ToDoDbContext()
    {
    }

    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {
    }

    // הגדרת הטבלאות במסד הנתונים
    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<User> Users { get; set; } 

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // כאן הקוד יחפש את מחרוזת החיבור שהגדרנו ב-Render
        if (!optionsBuilder.IsConfigured)
        {
            // השורה הזו מאפשרת ל-EF לעבוד עם MySQL
             optionsBuilder.UseMySql("name=ToDoDB", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.45-mysql"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("items");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("Users"); 
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
