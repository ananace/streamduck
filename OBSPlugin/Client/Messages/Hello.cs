using System.Text.Json.Serialization;

namespace OBSPlugin.Client.Messages;

[Opcode(0)]
public class Hello : Message
{
  public class AuthenticationData
  {
    public string? Challenge { get; set; }
    public string? Salt { get; set; }
  }

  [JsonPropertyName("obsWebSocketVersion")]
  public string? OBSWebSocketVersion { get; set; }
  [JsonPropertyName("rpcVersion")]
  public uint RPCVersion { get; set; }
  public AuthenticationData? Authentication { get; set; }

  [JsonIgnore]
  public bool NeedsAuthentication => Authentication != null;
}
