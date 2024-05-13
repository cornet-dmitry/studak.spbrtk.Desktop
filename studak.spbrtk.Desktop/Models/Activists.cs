using System;

namespace studak.spbrtk.Desktop.Models;

public class Activists
{
    public int Id { get; set; }
    
    public string? Surname { get; set; }

    public string? Name { get; set; }

    public string? Patronymic { get; set; }
    
    public string? Group { get; set; }
    
    public string? Status { get; set; }
    
    public int? Kpi { get; set; }
    
    public DateTime? DateBirth { get; set; }
    
    public string? Phone { get; set; }

    public string? Email { get; set; }
    
    public string? VkLink { get; set; }

    public string? TgLink { get; set; }
}