using Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Common.Models;

public class SkiSlopeState
{
    // Properties
    [Key]
    [DataMember]
    public Guid Id { get; set; }
    [DataMember]
    public string Place { get; set; }
    [DataMember]
    public DateTime Date { get; set; }
    [DataMember]
    public int Number { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public SkiSlopeCondition Condition { get; set; }
    [DataMember]
    public string Details { get; set; }
    [DataMember]
    public Weather Weather { get; set; }

    // Constructors

    public SkiSlopeState(string place, DateTime date, int number, string name, SkiSlopeCondition condition, string details)
    {
        Id = Guid.NewGuid();
        Place = place;
        Date = date;
        Number = number;
        Name = name;
        Condition = condition;
        Details = details;
    }

    public SkiSlopeState()
    {
    }
}