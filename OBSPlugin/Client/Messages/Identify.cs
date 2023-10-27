using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace OBSPlugin.Client.Messages;

[Opcode(1)]
public class Identify : Message
{
  [JsonPropertyName("rpcVersion")]
  public uint RPCVersion { get; set; }
  public string? Authentication { get; set; }
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public EventSubscription? EventSubscriptions { get; set; }

  public Identify() {}
  public Identify(string password, Hello.AuthenticationData auth)
  {
      using var sha256 = SHA256.Create();

      var saltedPasswordBytes = Encoding.UTF8.GetBytes(password + auth.Salt);
      var secretString = Convert.ToBase64String(sha256.ComputeHash(saltedPasswordBytes));
      var saltedSecretBytes = Encoding.UTF8.GetBytes(secretString + auth.Challenge);

      Authentication = Convert.ToBase64String(sha256.ComputeHash(saltedSecretBytes));
  }
}
