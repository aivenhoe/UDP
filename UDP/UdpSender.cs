//Inspired by this thread: https://forum.unity.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/
//thanks OP la1n
//thanks MattijsKneppers for letting me know that I also need to lock my queue while enqueuing
//adapted during projects according to my needs

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/// <summary>
/// General purpose UDP receiver class. see also: <see cref="UdpReceiver"/>
/// </summary>
public class UdpSender 
{
    public string IpAddress { get; private set; }
    public int SourcePort { get; private set; } //sometimes we need to define the source port, since some devices only accept messages coming from a predefined sourceport.
    public int RemotePort { get; private set; }

    private IPEndPoint _remoteEndPoint;
    private UdpClient _client;

    public UdpSender(string ipAddress, int remotePort, int sourcePort = -1) //if sourceport is not set, its being chosen randomly by the system
    {
        IpAddress = ipAddress;
        SourcePort = sourcePort;
        RemotePort = remotePort;

        _remoteEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), RemotePort);
        if (this.SourcePort <= -1)
        {
            _client = new UdpClient();
            Debug.Log($"Sending to {IpAddress}:{RemotePort}");
        }
        else
        {
            _client = new UdpClient(this.SourcePort);
            Debug.Log($"Sending to {IpAddress}:{RemotePort} from Source Port:{SourcePort}");
        }

    }

    ~UdpSender()
    {
        _client.Close();
    }
    // sendData in different ways. Can be extended accordingly
    public void SendData(byte[] data)
    {
        try
        {
            _client.Send(data, data.Length, _remoteEndPoint);
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
    }
    public void sendString(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            _client.Send(data, data.Length, _remoteEndPoint);

        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
    }

    public void sendInt32(Int32 myInt)
    {
        try
        {  
            byte[] data = BitConverter.GetBytes(myInt);
            _client.Send(data, data.Length, _remoteEndPoint);
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
    }
    public void sendInt32Array(Int32[] myInts)
    {
        try
        {
            byte[] data = new byte[myInts.Length * sizeof(Int32)];
            Buffer.BlockCopy(myInts, 0, data, 0, data.Length);
            _client.Send(data, data.Length, _remoteEndPoint);
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
    }
    public void sendInt16Array(Int16[] myInts)
    {
        try
        {
            byte[] data = new byte[myInts.Length * sizeof(Int16)];
            Buffer.BlockCopy(myInts, 0, data, 0, data.Length);
            _client.Send(data, data.Length, _remoteEndPoint);
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
    }
}
