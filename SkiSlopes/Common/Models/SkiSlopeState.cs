using Common.Enums;
using System.Runtime.Serialization;

namespace Common.Models;

public class SkiSlopeState
{
    // Properties

    [DataMember]
    public Guid Id { get; set; }
    [DataMember]
    public string Place { get; set; }
    [DataMember]
    public string Date { get; set; }
    [DataMember]
    public int Number { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public SkiSlopeCondition Condition { get; set; }
    [DataMember]
    public string Details { get; set; }

    // Constructors

    public SkiSlopeState(string place, string date, int number, string name, SkiSlopeCondition condition, string details)
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