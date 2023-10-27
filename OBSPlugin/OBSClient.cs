using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace OBSPlugin;

public class OBSClient : IDisposable
{
  ClientWebSocket? _ws;

  public async void Connect(string URL, string? Password = null)
  {
    Uri uri = new(URL);
    _ws = new();
    await _ws.ConnectAsync(uri, default);

    var hello = await ReadMessage() as Messages.Hello;
    if (hello == null)
      throw new Exception();

    if (hello.NeedsAuthentication)
    {
      var auth = hello.Authentication;
      if (auth == null)
        throw new Exception();
      if (Password == null)
        throw new Exception();

      await SendMessage(new Messages.Identify(Password, auth) { RPCVersion = hello.RPCVersion });
    }
    else
      await SendMessage(new Messages.Identify() { RPCVersion = hello.RPCVersion });

    var identified = await ReadMessage() as Messages.Identified;
    if (identified == null)
      throw new Exception();

    Console.WriteLine("Connected to WS with protocol {}", identified.NegotiatedRPCVersion);

    // StartReaderLoop();
  }

  public void Dispose()
  {
    _ws?.Dispose();
    _ws = null;
  }

  async Task<Message> ReadMessage()
  {
    StringBuilder data = new StringBuilder();
    while (true)
    {
      var bytes = new byte[1024];
      var wsResult = await _ws.ReceiveAsync(bytes, default);
      if (wsResult.MessageType != WebSocketMessageType.Text)
        throw new Exception();

      string res = Encoding.UTF8.GetString(bytes, 0, wsResult.Count);
      data.Append(res);
      if (wsResult.EndOfMessage)
        break;
    }

    var serializeOpts = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    };
    var msg = JsonSerializer.Deserialize<Message>(data.ToString(), serializeOpts);
    if (msg == null)
      throw new Exception();

    return msg;
  }

  async Task SendMessage(Message message)
  {
    if (_ws.State != WebSocketState.Open)
      throw new Exception();

    var serializeOpts = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    var json = JsonSerializer.Serialize<Message>(message, serializeOpts);

    var bytes = Encoding.UTF8.GetBytes(json);
    await _ws.SendAsync(bytes, WebSocketMessageType.Text, true, default);
  }

  public async Task<T> SendRPC<T>(RPCRequest request) where T : RPCResponse
  {
    var reqMsg = new Messages.Request { RequestType = request.RPCRequestType, RequestData = request };
    await SendMessage(reqMsg);

    // TODO: Reader loop

    var msg = await ReadMessage() as Messages.RequestResponse;
    if (msg == null)
      throw new Exception();

    if (!msg.IsSuccess)
      throw new Exception();
    if (msg.RequestID != reqMsg.RequestID)
      throw new Exception();

    return msg.ToRPCResponse<T>();
  }
}
