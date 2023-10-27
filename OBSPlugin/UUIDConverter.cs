using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OBSPlugin;

internal class UUIDConverter : JsonConverter<Guid>
{
  // public override CanConvert(Type typeToConvert) =>
  //   typeof(Message).IsAssignableFrom(typeToConvert);

  public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var str = reader.GetString();
    reader.read();

    return Guid.Parse(str);
  }

  public override void Write(Utf8JsonWriter writer, Guid uuid, JsonSerializerOptions options)
  {
    writer.WriteStringValue(uuid.ToString());
  }
}
