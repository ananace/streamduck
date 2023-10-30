using System.Text.Json.Serialization;

namespace OBSPlugin.Client.Requests;

[RPCRequest("GetSceneItemList")]
public class GetSceneItemListRequest : RPCRequest
{
  public string? SceneName { get; set; }
}

[RPCRequest("GetSceneItemList")]
public class GetSceneItemListResponse : RPCResponse
{
  public string[]? SceneItems { get; set; }
}
