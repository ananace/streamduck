using System;
using System.Linq;
using System.Text.Json;

namespace OBSPlugin.Tests;

[TestFixture]
public class MessageTest
{
  [Test]
  public void TestHelloMessage()
  {
    var inputData = """
    {
      "op": 0,
      "d": {
        "obsWebSocketVersion": "5.1.0",
        "rpcVersion": 1,
        "authentication": {
          "challenge": "+IxH4CnCiqpX1rM9scsNynZzbOe4KhDeYcTNS3PDaeY=",
          "salt": "lM1GncleQOaCu9lT1yeUZhFYnqhsLLP1G5lAGo3ixaI="
        }
      }
    }
    """;

    var serializeOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var message = JsonSerializer.Deserialize<Message>(inputData, serializeOpts);

    Assert.That(message, Is.InstanceOf<Messages.Hello>(), "Deserialized message did not result in Hello");

    var hello = message as Messages.Hello;
    Assert.That(hello.Authentication.Challenge, Is.EqualTo("+IxH4CnCiqpX1rM9scsNynZzbOe4KhDeYcTNS3PDaeY="));
    Assert.That(hello.Authentication.Salt, Is.EqualTo("lM1GncleQOaCu9lT1yeUZhFYnqhsLLP1G5lAGo3ixaI="));

    var identify = new Messages.Identify("supersecretpassword", hello.Authentication) { RPCVersion = 1 };

    serializeOpts = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    var outputData = JsonSerializer.Serialize<Message>(identify, serializeOpts);
    var expectedOutputData = """
    {
      "op": 1,
      "d": {
        "rpcVersion": 1,
        "authentication": "1Ct943GAT\u002B6YQUUX47Ia/ncufilbe6\u002BoD6lY\u002B5kaCu4="
      }
    }
    """;
    var trimmedOutput = String.Concat(expectedOutputData.Where(c => !Char.IsWhiteSpace(c)));

    Assert.That(outputData, Is.EqualTo(trimmedOutput), "Serialized Identify did not match expected output");
  }
}
