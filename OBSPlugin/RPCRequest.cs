using System.Reflection;
using System.Text.Json.Serialization;

namespace OBSPlugin;

public class RPCRequest
{
  [JsonIgnore]
  public string RPCRequestType => this.GetType().GetCustomAttribute<RPCRequestAttribute>()?.Request ?? "";
}
