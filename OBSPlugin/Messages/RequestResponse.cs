using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OBSPlugin.Messages;

[Opcode(7)]
public class RequestResponse : Message
{
  public class Status
  {
    public bool Result { get; set; }
    public int Code { get; set; }
    public string? Comment { get; set; }
  }

  public string? RequestType { get; set; }
  [JsonPropertyName("requestId")]
  public Guid? RequestID { get; set; }
  public Status? RequestStatus { get; set; }

  // Here handled as a general dictionary to avoid complex deserialization code
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public Dictionary<string, object>? ResponseData { get; set; }

  [JsonIgnore]
  public bool IsSuccess => RequestStatus?.Result ?? false;

  public T ToRPCResponse<T>() where T : RPCResponse
  {
    if (!IsSuccess)
      throw new Exception();

    var data = JsonSerializer.Serialize(ResponseData);

    var serializeOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    return JsonSerializer.Deserialize<T>(data, serializeOpts);
  }
}
