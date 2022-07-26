# Udp Send / Receive class for unity

The writing of this class has been triggered through [this thread]( https://forum.unity.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/ "this thread"), so kudos to the OP and everyone contributing. Throughout different projects the code has been adapted and hopefully improved.


### useage

###### sending
To send a UDP Message just init the port and start sending, like so:
```csharp
    private UdpSender _udpSender = new UdpSender("127.0.0.1", 8000, 8001);
    void Start()
    {
        sender.sendString("Hello from Start. " + Time.realtimeSinceStartup);
    }
```



###### receiving
Since receiving happens in a separate thread, the receiving thread can queue the incoming packets, or discard them, depending on how one initializes the receiver.
```csharp
    private UdpReveiver _udpReceiver1 = new UdpReceiver(9000, UdpReceiveMode.Discard);
    private UdpReceiver _udpReceiver2 = new UdpReceiver(9001, UdpReceiveMode.Queue);

    void Start()
    {
        _udpReveiver.ReceivedNewData += OnMessageReceived;
    }

    void OnMessageReceived(byte[] data)
    {
       Debug.Log(System.Text.Encoding.UTF8.GetString(data));
    }
```
When receive mode is set to Queue, in the Update loop it can be used as follows:
```csharp
    void Update()
    {
        if (_udpReceiver2.LastReceivedUdpPacket > 0)
            Debug.Log(System.Text.Encoding.UTF8.GetString(_udpReceiver2.LastReceivedUdpPacket));
 
        Queue<byte[]> myPacketList = receiver2.GetQueuedUDPPackets();
        Debug.Log(myPacketList.Count);     
    }
```
It's important to close the ports.
```csharp
    private void OnDisable()
    {
        _udpReceiver1.CloseReceving();
        _udpReceiver1.CloseReceving();
    }
```

