using Streamduck.Plugins;

namespace OBSPlugin;

public class OBSPlugin : Plugin, IDisposable
{
  private readonly OBSClient _client = new OBSClient();

  public override string Name => "OBSPlugin";

  public string URL { get; set; } = "http://localhost:4455";
  public string? Password { get; set; }

  public void Dispose()
  {
    _client.Dispose();
  }
}
