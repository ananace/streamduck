using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OBSPlugin.Messages;

[Opcode(5)]
public class Event : Message
{
  [JsonRequired]
  public string? EventType { get; set; }
  [JsonRequired]
  public EventSubscription EventIntent { get; set; }
  public Dictionary<string, object>? EventData { get; set; }
}
