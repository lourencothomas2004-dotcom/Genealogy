using UnityEngine;
using SQLite;

public class Person
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string BirthName { get; set; }
    public int? BirthFamilyID { get; set; }

    // Stored in database as "YYYY-MM-DD", "YYYY-MM", or "YYYY"
    public string BirthDate { get; set; } 
    public string DeathDate { get; set; }

    public string Biography { get; set; }

    [Ignore]
    public string DisplayBirthDate 
    {
        get 
        {
            if (string.IsNullOrEmpty(BirthDate)) return "Unknown";
            return BirthDate; 
        }
    }
}

public class NuclearFamily
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public int? parent1ID { get; set; }
    public int? parent2ID { get; set; }

    public string weddingDate { get; set; }
}
