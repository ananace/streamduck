using System;

namespace OBSPlugin; 

[AttributeUsage(AttributeTargets.Class)]
internal class OpcodeAttribute : Attribute
{
  public int Opcode { get; private set; }

  public OpcodeAttribute(int opcode)
  {
    Opcode = opcode;
  }
}
