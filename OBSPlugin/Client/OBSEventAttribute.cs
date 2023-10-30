using System;

namespace OBSPlugin.Client;

[AttributeUsage(AttributeTargets.Class)]
public class OBSEventAttribute : Attribute
{
  public string EventName { get; private set; }

  public OBSEventAttribute(string name)
  {
    EventName = name;
  }
}
