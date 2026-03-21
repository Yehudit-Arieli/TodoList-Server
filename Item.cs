//של המשימה, והוא קובע אילו שדות נתונים יהיו לכל משימה במערכת  (Entity)  הקובץ הזה מגדיר את המודל 
// (מזהה, שם וסטטוס ביצוע) כדי שהשרת ומסד הנתונים ידעו איך לטפל בה.
using System;
using System.Collections.Generic;

namespace TodoApi;

public partial class Item
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? IsComplete { get; set; }
}
