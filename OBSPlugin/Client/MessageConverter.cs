using System.Text.Json;
using System.Text.Json.Serialization;

namespace OBSPlugin.Client;

internal class MessageConverter : JsonConverter<Message>
{
  // public override CanConvert(Type typeToConvert) =>
  //   typeof(Message).IsAssignableFrom(typeToConvert);

  public override Message Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.StartObject)
      throw new JsonException();
    reader.Read();

    if (reader.TokenType != JsonTokenType.PropertyName)
      throw new JsonException();
    string? propertyName = reader.GetString();
    reader.Read();

    if (propertyName != "op")
      throw new JsonException();

    if (reader.TokenType != JsonTokenType.Number)
      throw new JsonException();

    int op = reader.GetInt32();
    reader.Read();

    Message msg = op switch
    {
      0 => JsonSerializer.Deserialize<Messages.Hello>(ref reader, options)!,
      1 => JsonSerializer.Deserialize<Messages.Identify>(ref reader, options)!,
      2 => JsonSerializer.Deserialize<Messages.Identified>(ref reader, options)!,
      3 => JsonSerializer.Deserialize<Messages.Reidentify>(ref reader, options)!,
      5 => JsonSerializer.Deserialize<Messages.Event>(ref reader, options)!,
      6 => JsonSerializer.Deserialize<Messages.Request>(ref reader, options)!,
      7 => JsonSerializer.Deserialize<Messages.RequestResponse>(ref reader, options)!,
      _ => throw new JsonException()
    };
    if (reader.TokenType != JsonTokenType.EndObject)
      throw new JsonException();
    reader.Read();
    return msg;
  }

  public override void Write(Utf8JsonWriter writer, Message message, JsonSerializerOptions options)
  {
    writer.WriteStartObject();
    writer.WriteNumber("op", message.OpCode);
    writer.WritePropertyName("d");
    JsonSerializer.Serialize(writer, message, message.GetType(), options);
    writer.WriteEndObject();
  }
}
