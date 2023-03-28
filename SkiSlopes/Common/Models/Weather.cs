using System.Runtime.Serialization;

namespace Common.Models;

public class Weather
{
    // Properties

    [DataMember]
    public string Temperature { get; set; }
    [DataMember]
    public string Clouds { get; set; }
    [DataMember]
    public string WindSpeed { get; set; }

    // Constructors

    public Weather()
    {
    }

    public Weather(double temperature, double clouds, double windSpeed)
    {
        Temperature = temperature.ToString();
        Clouds = clouds.ToString();
        WindSpeed = windSpeed.ToString();
    }
}
