using System.Text.Json;
using System.Text.Json.Serialization;
using Elastic.Clients.Elasticsearch;

namespace SearchService.DAL.Serializer;

public class GeoLocationConverter : JsonConverter<GeoLocation>
{
    public override GeoLocation? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) return null;
        double? lat = null, lon = null;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) break;
            if (reader.TokenType != JsonTokenType.PropertyName) continue;
            var prop = reader.GetString();
            reader.Read();
            if (prop == "lat") lat = reader.GetDouble();
            if (prop == "lon") lon = reader.GetDouble();
        }
        if (lat.HasValue && lon.HasValue)
            return new LatLonGeoLocation { Lat = lat.Value, Lon = lon.Value };
        return null;
    }

    public override void Write(Utf8JsonWriter writer, GeoLocation value, JsonSerializerOptions options)
    {
        // if (value is LatLonGeoLocation latLon)
        // {
        //     writer.WriteStartObject();
        //     writer.WriteNumber("lat", latLon.Lat);
        //     writer.WriteNumber("lon", latLon.Lon);
        //     writer.WriteEndObject();
        // }
        // else
        // {
        //     writer.WriteNullValue(); // или можно выбросить исключение
        // }
    }
}