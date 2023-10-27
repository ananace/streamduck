using System.Text.Json.Serialization;

namespace OBSPlugin.Client.Messages;

[Opcode(2)]
public class Identified : Message
{
  [JsonPropertyName("negotiatedRpcVersion")]
  public uint NegotiatedRPCVersion { get; set; }
}
