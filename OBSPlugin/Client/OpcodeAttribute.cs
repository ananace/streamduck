using System;

namespace OBSPlugin.Client;

[AttributeUsage(AttributeTargets.Class)]
internal class OpcodeAttribute : Attribute
{
  public int Opcode { get; private set; }

  public OpcodeAttribute(int opcode)
  {
    Opcode = opcode;
  }
}
