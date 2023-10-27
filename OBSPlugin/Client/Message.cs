using System.Reflection;
using System.Text.Json.Serialization;

namespace OBSPlugin.Client;

[JsonConverter(typeof(MessageConverter))]
public class Message
{
  [JsonIgnore]
  public int OpCode => this.GetType().GetCustomAttribute<OpcodeAttribute>()?.Opcode ?? -1;
}
