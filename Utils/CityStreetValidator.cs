using Domain.models;

namespace utils;

public static class CityStreetValidator
{
    public static bool IsValidStreetForCity(CityName city, StreetName street)
    {
        return _cityStreets.ContainsKey(city) && _cityStreets[city].Contains(street);
    }

    private static readonly Dictionary<CityName, List<StreetName>> _cityStreets = new()
    {
        {
            CityName.Timisoara,
            new List<StreetName>
            {
                StreetName.BulevardulMihaiViteazu,
                StreetName.StradaFeldioara,
                StreetName.StradaCluj,
                StreetName.BulevardulLiviuRebreanu
            }
        },
        {
            CityName.Bucharest,
            new List<StreetName>
            {
                StreetName.BulevardulMirceaEliade,
                StreetName.BulevardulPrimaverii,
                StreetName.StradaRacari,
                StreetName.CaleaDorobantilor,
                StreetName.BulevardulCamilRessu,
            }
        }
    };
}