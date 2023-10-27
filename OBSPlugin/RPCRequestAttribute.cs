using System;

namespace OBSPlugin;

[AttributeUsage(AttributeTargets.Class)]
internal class RPCRequestAttribute : Attribute
{
  public string Request { get; private set; }

  public RPCRequestAttribute(string request)
  {
    Request = request;
  }
}
