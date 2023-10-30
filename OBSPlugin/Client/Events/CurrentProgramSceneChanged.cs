namespace OBSPlugin.Client.Events;

[OBSEvent("CurrentProgramSceneChanged")]
public class CurrentProgramSceneChanged : OBSEvent
{
  public string? SceneName { get; set; }
}
