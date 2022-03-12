using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Structuralist.M2.Output;

public class ModuleM2JsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => 
        objectType.IsSubclassOf(typeof(Module))
        || objectType == typeof(Module);

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        /*if (value is null) return;
        JToken jToken = JToken.FromObject(value);
        JObject jObject = (JObject)jToken;
        if (value is RectangleModule rectangleModule)
        {
            jObject.AddFirst(new JProperty("west", rectangleModule.))
        }*/
        throw new NotImplementedException();
    }
}