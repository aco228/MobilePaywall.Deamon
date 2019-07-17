using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MobilePaywall.Deamon.Core
{
  public class SignalRConnector
  {
    private static string SIGNALR_URL = "http://services.app.mobilepaywall.com/";
    private static DateTime _referenceTime;
    private static HubConnection _connection;
    private static IHubProxy _hubProxy;

    public enum MessageType { unknown, to_server, run }

    public SignalRConnector()
    {
      Connect();
    }

    private static void Connect()
    {
      _connection = new HubConnection(SIGNALR_URL);
      _hubProxy = _connection.CreateHubProxy("DeamonHub");
      _connection.Start().Wait();
      _connection.StateChanged += Connection_StateChanged;
      _connection.Closed += Connection_Closed;
      _connection.ConnectionSlow += Connection_ConnectionSlow;
      _connection.Error += Connection_Error;

      _referenceTime = DateTime.Now;
      _hubProxy.On("Update", OnReceiveMessage);
      SendMessage(MessageType.to_server, "Task.Deamon started");
    }

    private static void Connection_Error(Exception obj)
    {
      //Program.Connection.Stop();
      Console.WriteLine(string.Format("CONNECTION FATAL. "));
      //System.Threading.Thread.Sleep(SECONDS_REFERENCE_TIME_FOR_RECONNECT * 1000);
      //Connect();
      int a = 0;
    }

    private static void Connection_ConnectionSlow()
    {
      Console.WriteLine(string.Format("CONNECTION SLOW"));
    }

    private static void Connection_Closed()
    {
      _connection.Stop();
      Connect();
    }

    private static void Connection_StateChanged(StateChange obj)
    {
      Console.WriteLine(string.Format("ConnectionState Changed: {0}", obj.NewState.ToString()));
    }

    // SUMMARY: On receive notification from server
    static void OnReceiveMessage(dynamic message)
    {
      string tag = message.tag;
      string data = message.message;

      if (tag.Equals(MessageType.to_server.ToString()))
        return;

      Console.WriteLine(string.Format("{0} <- SignalR message received ({1} - {2})", DateTime.Now, tag, data));

      if (tag.Equals(MessageType.run.ToString()))
        DeamonManager.OnDemand(data);

      _referenceTime = DateTime.Now;
    }

    public static void SendMessage(string message)
    {
      SendMessage(MessageType.to_server, message);
    }

    public static void SendMessage(MessageType tag, string message)
    {
      Console.WriteLine(string.Format("{0} -> SignalR ({1} - {2})", DateTime.Now, tag.ToString(), message));
      System.Threading.Tasks.Task.Run(() => SendMessageAsync(tag.ToString(), message));
    }

    public static void SendMessageAsync(string tag, string message)
    {
      string url = string.Format(SIGNALR_URL + "deamontask/Send?tag={0}&message={1}", tag, message);
      try
      {
        using (WebClient client = new WebClient())
        {
          string response = client.DownloadString(url);
          if (response.Equals("nok"))
            Console.WriteLine("Server returned NOK");
        }
      }
      catch (Exception e)
      {
        Console.WriteLine("Could not send request to server on url: " + url);
      }
    }


  }
}
