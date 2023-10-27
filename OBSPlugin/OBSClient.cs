using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using OBSPlugin.Util;

namespace OBSPlugin;

public class OBSClient : IDisposable
{
  ClientWebSocket? _ws;
  Thread? _readLoop;
  Dictionary<Guid, TaskCompletionSource<Client.Messages.RequestResponse>> _rpcInFlight = new Dictionary<Guid, TaskCompletionSource<Client.Messages.RequestResponse>>();

  public async void Connect(string URL, string? Password = null, CancellationToken cToken = default)
  {
    if (_ws?.State == WebSocketState.Open)
    {
      await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, default, cToken);
      _ws.Dispose();
    }

    Uri uri = new(URL);
    _ws = new();
    await _ws.ConnectAsync(uri, cToken);

    var hello = await ReadMessage(cToken) as Client.Messages.Hello;
    if (hello == null)
      throw new Exception();

    if (hello.NeedsAuthentication)
    {
      var auth = hello.Authentication;
      if (auth == null)
        throw new Exception();
      if (Password == null)
        throw new Exception();

      await SendMessage(new Client.Messages.Identify(Password, auth) { RPCVersion = hello.RPCVersion }, cToken);
    }
    else
      await SendMessage(new Client.Messages.Identify() { RPCVersion = hello.RPCVersion }, cToken);

    var identified = await ReadMessage(cToken) as Client.Messages.Identified;
    if (identified == null)
      throw new Exception();

    Console.WriteLine("Connected to WS with protocol {}", identified.NegotiatedRPCVersion);

    _readLoop = new Thread(new ThreadStart(ReadLoop));
    _readLoop.Start();
  }

  public void Dispose()
  {
    _readLoop = null;
    _rpcInFlight.Clear();
    _ws?.Dispose();
    _ws = null;
  }

  void ReadLoop()
  {
    while (_ws?.State == WebSocketState.Open)
    {
      var msg = ReadMessage();
      msg.Wait();

      if (msg.Result is Client.Messages.RequestResponse rpcResponse && rpcResponse.RequestID != null)
      {
        var waiting = _rpcInFlight[rpcResponse.RequestID.Value];
        if (waiting != null)
          waiting.SetResult(rpcResponse);
      }
    }
  }

  async Task<Client.Message> ReadMessage(CancellationToken cToken = default)
  {
    if (_ws?.State != WebSocketState.Open)
      throw new Exception();

    StringBuilder data = new StringBuilder();
    while (!cToken.IsCancellationRequested)
    {
      var bytes = new byte[1024];
      var wsResult = await _ws.ReceiveAsync(bytes, cToken);
      if (wsResult.MessageType != WebSocketMessageType.Text)
        throw new Exception();

      string res = Encoding.UTF8.GetString(bytes, 0, wsResult.Count);
      data.Append(res);
      if (wsResult.EndOfMessage)
        break;
    }
    cToken.ThrowIfCancellationRequested();

    var serializeOpts = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    };
    var msg = JsonSerializer.Deserialize<Client.Message>(data.ToString(), serializeOpts);
    if (msg == null)
      throw new Exception();

    return msg;
  }

  async Task SendMessage(Client.Message message, CancellationToken cToken = default)
  {
    if (_ws?.State != WebSocketState.Open)
      throw new Exception();

    var serializeOpts = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    var json = JsonSerializer.Serialize<Client.Message>(message, serializeOpts);

    var bytes = Encoding.UTF8.GetBytes(json);
    await _ws.SendAsync(bytes, WebSocketMessageType.Text, true, cToken);
  }

  public async Task<T> SendRPC<T>(Client.RPCRequest request, CancellationToken cToken = default) where T : Client.RPCResponse
  {
    var reqMsg = new Client.Messages.Request { RequestType = request.RPCRequestType, RequestData = request };
    var inFlightTask = new TaskCompletionSource<Client.Messages.RequestResponse>();

    try
    {
      _rpcInFlight[reqMsg.RequestID] = inFlightTask;
      await SendMessage(reqMsg, cToken);

      await inFlightTask.WaitAsync(cToken);
      cToken.ThrowIfCancellationRequested();

      var msg = await inFlightTask.Task;
      if (msg == null)
        throw new Exception();

      if (!msg.IsSuccess)
        throw new Exception();
      if (msg.RequestID != reqMsg.RequestID)
        throw new Exception();

      return msg.ToRPCResponse<T>();
    }
    finally
    {
      _rpcInFlight.Remove(reqMsg.RequestID);
    }
  }
}
