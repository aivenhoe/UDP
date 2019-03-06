﻿//Inspired by this thread: https://forum.unity.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/
//thanks OP la1n
//thanks MattijsKneppers for letting me know that I also need to lock my queue while enqueuing
//adapted during projects according to my needs

using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSend 
{
    public string IP { get; private set; }
    public int sourcePort { get; private set; } //sometimes we need to define the source port, since some devices only accept messages coming from a predefined sourceport.
    public int remotePort { get; private set; }

    IPEndPoint remoteEndPoint;
    UdpClient client;

    public void init(string IPAdress, int RemotePort, int SourcePort = -1) //if sourceport is not set, its being chosen randomly by the system
    {
        IP = IPAdress;
        sourcePort = SourcePort;
        remotePort = RemotePort;

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), remotePort);
        if (sourcePort <= -1)
        {
            client = new UdpClient();
            Debug.Log("Sending to " + IP + " : " + remotePort);
        }
        else
        {
            client = new UdpClient(sourcePort);
            Debug.Log("Sending to " + IP + " : " + remotePort + " from Source Port: " + sourcePort);
        }

    }

    // sendData in different ways. Can be extended accordingly
    public void sendString(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);

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
            client.Send(data, data.Length, remoteEndPoint);
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
            client.Send(data, data.Length, remoteEndPoint);
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
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
    }
}
