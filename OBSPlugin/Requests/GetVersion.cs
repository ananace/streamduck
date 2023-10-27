using System.Text.Json.Serialization;

namespace OBSPlugin.Requests;

[RPCRequest("GetVersion")]
public class GetVersionRequest : RPCRequest
{

}

[RPCRequest("GetVersion")]
public class GetVersionResponse : RPCResponse
{
  [JsonPropertyName("obsVersion")]
  public string? OBSVersion { get; set; }
  [JsonPropertyName("obsWebSocketVersion")]
  public string? OBSWebSocketVersion { get; set; }
  [JsonPropertyName("rpcVersion")]
  public string? RPCVersion { get; set; }

  public string[]? AvailableRequests { get; set; }
  public string[]? SupportedImageFormats { get; set; }

  public string? Platform { get; set; }
  public string? PlatformDescription { get; set; }
}
