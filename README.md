# UDP Send / Receive class for unity

The writing of this class has been triggered through [this thread]( https://forum.unity.com/threads/simple-udp-implementation-send-read-via-mono-c.15900/ "this thread"), so kudos to the OP and everyone contributing. Throughout different projects the code has been adapted and hopefully improved.


### useage

###### sending
To send a UDP Message just init the port and start sending, like so:
```csharp
    public UDPSend sender = new UDPSend();
    void Start()
    {
        sender.init("127.0.0.1", 8000, 8001);
        sender.sendString("Hello from Start. " + Time.realtimeSinceStartup);
    }
```



###### receiving
Since receiving happens in a separate thread, the receiving thread can queue the incoming packets, or discard them, depending on how one initializes the receiver.
```csharp
    public UDPReceive receiver1 = new UDPReceive();
    public UDPReceive receiver2 = new UDPReceive();

    void Start()
    {
        Application.targetFrameRate = 60;
        receiver1.init(9000,UDPReceiveMode.Discard);
        receiver2.init(9001, UDPReceiveMode.Queue);
    }
```
In the Update loop it can be used as follows:
```csharp
    void Update()
    {
        if (receiver1.receivedNew)
            Debug.Log(System.Text.Encoding.UTF8.GetString(receiver1.lastReceivedUDPPacket));
 
        Queue<byte[]> myPacketList = receiver2.GetQueuedUDPPackets();
        Debug.Log(myPacketList.Count);     
    }
```
It's important to close the ports.
```csharp
    private void OnDisable()
    {
        receiver1.CloseReceving();
        receiver2.CloseReceving();
    }
```

