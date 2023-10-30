using System.Reflection;
using System.Text.Json.Serialization;

namespace OBSPlugin.Client;

public class OBSEvent
{
  [JsonIgnore]
  public string OBSEventType => this.GetType().GetCustomAttribute<OBSEventAttribute>()?.EventName ?? "";
}
