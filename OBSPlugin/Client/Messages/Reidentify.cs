using System.Text.Json.Serialization;

namespace OBSPlugin.Client.Messages;

[Opcode(3)]
public class Reidentify : Message
{
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public EventSubscription? EventSubscriptions { get; set; }
}
