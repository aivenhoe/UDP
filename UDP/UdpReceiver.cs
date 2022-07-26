//Inspired by this thread: https://forum.unity.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/
//thanks OP la1n
//thanks MattijsKneppers for letting me know that I also need to lock my queue while enqueuing
//adapted during projects according to my needs


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
public enum UdpReceiveMode { Discard, Queue };

/// <summary>
/// General purpose UDP receiver class. see also: <see cref="UdpSender"/>
/// </summary>
public class UdpReceiver
{
    public int Port { get; private set; }

    public UdpReceiveMode Mode {get; private set;}
    public bool ReceiveThreadIsRunning { get; private set; }

    public delegate void ReceivedData(byte[] data);
    public event ReceivedData ReceivedNewData;

    public byte[] LastReceivedUdpPacket = new byte[0];
    public DateTime LastReceivedPacketTimestamp;

    public int QueuedPacketsCount {
        get
        {
            return _queueOfReceivedUDPpackets.Count;
        }
    }
    private readonly Queue<byte[]> _queueOfReceivedUDPpackets = new Queue<byte[]>();


    private UdpClient _client;
    private Thread _receiveThread;

    //Since receiving is running in a separte thread at maximum speed, incomming messages are queued up, so they can be analyzed in the main update loop.
    public UdpReceiver(int localPort, UdpReceiveMode mode)
    {     
        Port = localPort; 
        Mode = mode;


        Debug.Log($"Listening on UDP port: {Port} with {nameof(UdpReceiveMode)} {Mode}");

        ReceiveThreadIsRunning = true;
        _receiveThread = new Thread(new ThreadStart(ReceiveData));
        _receiveThread.IsBackground = true;
        _receiveThread.Start();

    }

    private void ReceiveData()
    {
        _client = new UdpClient(Port);
        while (ReceiveThreadIsRunning)
        {
            try
            {             
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = _client.Receive(ref anyIP);
                if (data.Length > 0)
                {
                    LastReceivedUdpPacket = data.ToArray();      

                    LastReceivedPacketTimestamp = DateTime.Now;

                    ReceivedNewData.Invoke(LastReceivedUdpPacket);

                    //Debug.Log("ReceivedUDP Data >> " + lastReceivedUDPPacket + " with timestamp: " + timestamp);

                    if (Mode == UdpReceiveMode.Queue)
                    {
                        lock (_queueOfReceivedUDPpackets)
                        {
                            _queueOfReceivedUDPpackets.Enqueue(LastReceivedUdpPacket);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.Log(err.ToString());      
            }
        }
    }
    public Queue<byte[]> GetQueuedUDPPackets() 
    {
      lock(_queueOfReceivedUDPpackets)
        {
            Queue<byte[]> packetQueue = new Queue<byte[]>(_queueOfReceivedUDPpackets);
            _queueOfReceivedUDPpackets.Clear();
            return packetQueue;
        }
    }
    public void CloseReceving()
    {
        Debug.Log($"closing receiving UDP on port: {Port}"); 

        if (_receiveThread != null)
            _receiveThread.Abort();

        _client.Close();
        ReceiveThreadIsRunning = false;
    }
    ~UdpReceiver()
    {
        CloseReceving();
    }
}