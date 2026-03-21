// הקובץ הזה מגדיר את מבנה הנתונים של המשתמש במערכת, ומשמש כ"תעודת זהות" דיגיטלית
// (Login) המכילה את שם המשתמש והסיסמה לצורך תהליך ההתחברות 
//  והנפקת הטוקן.
namespace TodoApi;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}