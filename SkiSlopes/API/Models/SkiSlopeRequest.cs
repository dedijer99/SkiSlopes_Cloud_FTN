using Common.Enums;

namespace API.Models;

public record SkiSlopeRequest(string Place, string Date, int Number, string Name, SkiSlopeCondition Condition, string Details);
