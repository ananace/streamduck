using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OBSPlugin.Messages;

[Opcode(6)]
public class Request : Message
{
  [JsonRequired]
  public string? RequestType { get; set; }
  [JsonPropertyName("requestId")]
  [JsonRequired]
  public Guid RequestID { get; set; } = Guid.NewGuid();
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public RPCRequest? RequestData { get; set; }
}
