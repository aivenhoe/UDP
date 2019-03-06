//Inspired by this thread: https://forum.unity.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/
//thanks OP la1n
//thanks MattijsKneppers for letting me know that I also need to lock my queue while enqueuing
//adapted during projects according to my needs


using UnityEngine;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
public enum UDPReceiveMode { Discard, Queue };
public class UDPReceive
{
    Thread receiveThread;
    UdpClient client;
    public int port { get; private set; }

    public UDPReceiveMode mode {get; private set;}
    public bool receiveThreadIsRunning { get; private set; }

    private bool _receivedNew;
    public bool receivedNew //little workaround so if we check on main update loop, receivedNew is being resetted to false..
    {
        get {
            if (_receivedNew)
            {
                _receivedNew = false;
                return true;
            }
            else
            {
                return false;
            }
            }
        set {
            _receivedNew = value;
        }
    }
    public byte[] lastReceivedUDPPacket = new byte[0];
    public int lastReceivedUDPPacketLength;

    public int queuedPacketsCount { get; private set; }
    private readonly Queue<byte[]> queueOfReceivedUDPpackets = new Queue<byte[]>();

    public string timestamp = "0";


    //Since receiving is running in a separte thread at maximum speed, incomming messages are queued up, so they can be analyzed in the main update loop.
    public void init(int LocalPort, UDPReceiveMode Mode)
    {     
        port = LocalPort; 
        Debug.Log("Listening on UDP port: " + port);
        mode = Mode;
        queuedPacketsCount = -1;

        receiveThreadIsRunning = true;
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }
    public static String GetTimestamp(DateTime value)
    {
        return value.ToString("yyyyMMddHHmmssffff");
    }
    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (receiveThreadIsRunning)
        {
            try
            {             
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                if (data.Length > 0)
                {
                    lastReceivedUDPPacketLength = data.Length;
                    lastReceivedUDPPacket = data.ToArray();      

                    timestamp = GetTimestamp(DateTime.Now);                

                    receivedNew = true;

                    //Debug.Log("ReceivedUDP Data >> " + lastReceivedUDPPacket + " with timestamp: " + timestamp);

                    if (mode == UDPReceiveMode.Queue)
                    {
                        lock (queueOfReceivedUDPpackets)
                        {
                            queueOfReceivedUDPpackets.Enqueue(lastReceivedUDPPacket);
                            queuedPacketsCount = queueOfReceivedUDPpackets.Count;
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
      lock(queueOfReceivedUDPpackets)
        {
            Queue<byte[]> packetQueue = new Queue<byte[]>(queueOfReceivedUDPpackets);
            queueOfReceivedUDPpackets.Clear();
            receivedNew = false;
            return packetQueue;
        }
    }
    public void CloseReceving()
    {
        Debug.Log("closing receiving UDP on port: " + port); 

        if (receiveThread != null)
            receiveThread.Abort();

        client.Close();
        receiveThreadIsRunning = false;

    }
}