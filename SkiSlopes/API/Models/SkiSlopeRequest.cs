using Common.Enums;

namespace API.Models;

public record SkiSlopeRequest(string Place, int Number, string Name, SkiSlopeCondition Condition, string Details);
