using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OBSPlugin.Client.Messages;

[Opcode(5)]
public class Event : Message
{
  [JsonRequired]
  public string? EventType { get; set; }
  [JsonRequired]
  public EventSubscription EventIntent { get; set; }

  public Dictionary<string, object>? EventData { get; set; }

  public T ToEvent<T>() where T : OBSEvent
  {
    var data = JsonSerializer.Serialize(EventData);

    var serializeOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var ev= JsonSerializer.Deserialize<T>(data, serializeOpts);
    if (ev== null)
      throw new Exception();
    if (EventType != ev.OBSEventType)
      throw new Exception();

    return ev;
  }
}
