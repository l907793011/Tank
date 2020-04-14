using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Assets.Util
{
    class GameNetWork
    {
    }

    [System.Serializable]
    public class ServerDataList
    {
        public List<ServerData> list = new List<ServerData>();

        internal void Add(ServerData serverData)
        {
            list.Add(serverData);
        }

        internal void Remove(ServerData serverData)
        {
            list.RemoveAll((ServerData item) =>
            {
                return serverData.IP == item.IP && serverData.port == item.port;
            });
        }

        internal void Update(ServerData serverData)
        {
            foreach (var item in list)
            {
                if (serverData.IP == item.IP && serverData.port == item.port)
                {
                    item.name = serverData.name;
                    item.number = serverData.number;
                    item.maxNumber = serverData.maxNumber;
                }
            }
        }
    }


    [System.Serializable]
    public class ServerData
    {
        public string name;
        public int number;
        public int maxNumber;
        public string IP;
        public int port;
        /// <summary>
        /// 随机种子
        /// </summary>
        public int randomSeed;


        public ServerData()
        {
            randomSeed = new System.Random().Next(0, 99999);
        }




        public ServerData(string name, int maxNumber)
        {
            this.name = name;
            this.maxNumber = maxNumber;
            randomSeed = new System.Random().Next(0, 99999);
        }
    }
    /// <summary>
    /// 游戏数据包
    /// </summary>
    [System.Serializable]
    public struct GameNetPacket
    {
        public string name;
        public string data;
    }


    public interface IGameNetWorkBase
    {
        void Close();
    }




    public interface IGameServer : IGameNetWorkBase
    {


    }
    public interface IGameClient : IGameNetWorkBase
    {
        void CheckOutServerList(Action<ServerDataList> OnCheckOut);
        void ConnectServer(ServerData serverData, Action<ServerData> OnConnect, Action<string> OnReceive);
        void SendMessageData(string data);
    }
    /// <summary>
    /// 游戏服务器
    /// </summary>
    public class GameServer : IGameServer
    {
        public ServerData serverData;
        /// <summary>
        /// 服务器套接字
        /// </summary>
        Socket ServerSocket;
        /// <summary>
        /// 客户端的套接字
        /// </summary>
        List<Socket> ClientSockets = new List<Socket>();
        const int minPort = 2725;
        const int maxPort = 2730;
        const int radiateMinPort = 2730;
        const int radiateMaxPort = 2735;
        private bool open;
        /// <summary>
        /// 循环客户端的信息
        /// </summary>
        /// <param name="socket"></param>
        private void LoopClientMessage(Socket socket)
        {
            ClientSockets.Add(socket);

            //监听客户端发送信息，并且进行广播
            new Thread(() =>
            {
                while (open)
                {
                    try
                    {
                        byte[] receive = new byte[1024];
                        //等待获取信息
                        socket.Receive(receive);
                        //进行广播发送
                        foreach (var item in ClientSockets)
                        {
                            item.Send(receive);
                        }
                    }
                    catch (Exception)
                    {
                        Debug.Log("断开连接...");
                        Close();
                    }
                }
            }).Start();
        }
        public GameServer(ServerData serverData)
        {
            this.serverData = serverData;

            open = true;
            //服务器初始化
            // 侦听所有网络客户接口的客活动
            //使用指定的地址簇协议、套接字类型和通信协议
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            BindServerSocket(minPort);
            //设定最多玩家数量
            ServerSocket.Listen(this.serverData.maxNumber);

            // string ipStr = ipAddress.ToString();
            Debug.Log("服务器初始化完成");
            SendServerDataToAll("UpdateServer");

            //接受广播信息
            Socket receiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint sep = BindReceiverSocket(receiveSocket, radiateMinPort);
            EndPoint ep = (EndPoint)sep;




            //接收广播线程
            new Thread(() =>
            {
                while (open)
                {
                    byte[] message = new byte[1024];//设置缓冲数据流
                    receiveSocket.ReceiveFrom(message, ref ep);//接收数据,并确把数据设置到缓冲流里面
                    string data = Encoding.Unicode.GetString(message).TrimEnd('\u0000');

                    GameNetPacket receiveData = JsonUtility.FromJson<GameNetPacket>(data);

                    switch (receiveData.name)
                    {
                        case "Is Room?":
                            GameNetPacket gameNetPacket = new GameNetPacket();
                            gameNetPacket.name = "NewServer";
                            gameNetPacket.data = JsonUtility.ToJson(serverData);
                            SendReceiveTo(JsonUtility.ToJson(gameNetPacket));
                            break;
                        default:
                            break;
                    }
                }
            }).Start();



            //监听客户端连接线程
            new Thread(() =>
            {
                while (open)
                {
                    try
                    {
                        //与客户的排队建立连接(堵塞)
                        Socket socket = ServerSocket.Accept();

                        LoopClientMessage(socket);

                        Debug.Log("连接成功");
                        ////进行广播更新房间信息
                        SendServerDataToAll("UpdateServer");
                    }
                    catch (Exception)
                    {
                        Debug.Log("停止等待客户端连接");
                        open = false;
                    }
                }
            }).Start();




        }

        private void SendServerDataToAll(string receiveType)
        {
            GameNetPacket gameNetPacket = new GameNetPacket();
            gameNetPacket.name = receiveType;
            gameNetPacket.data = JsonUtility.ToJson(serverData);
            SendReceiveTo(JsonUtility.ToJson(gameNetPacket));
        }

        private static void SendReceiveTo(string data)
        {
            //发送广播等待传输回来房间信息
            //初始化一个发送广播和指定端口的网络端口实例
            Socket sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);




            //设置该scoket实例的发送形式
            sendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            for (int i = radiateMinPort; i < radiateMaxPort; i++)
            {
                IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, i);
                byte[] buffer = Encoding.Unicode.GetBytes(data);
                //发送
                sendSocket.SendTo(buffer, iep);
            }
        }
        private static IPEndPoint BindReceiverSocket(Socket receiveSocket, int port)
        {
            IPEndPoint sep = null;
            try
            {
                sep = new IPEndPoint(IPAddress.Any, port);//初始化一个侦听局域网内部所有IP和指定端口
                receiveSocket.Bind(sep);//绑定这个实例
            }
            catch (Exception)
            {
                port++;
                if (port < radiateMaxPort)
                {
                    BindReceiverSocket(receiveSocket, port);
                }
                else
                {
                    throw;
                }
            }
            return sep;
        }


        private void BindServerSocket(int port)
        {
            try
            {
                IPEndPoint IPEndPoint = new IPEndPoint(IPAddress.Any, port);
                //绑定IP地址和端口号
                ServerSocket.Bind(IPEndPoint);


                string hostName = Dns.GetHostName();   //获取本机名
                IPAddress[] localhosts = Dns.GetHostAddresses(hostName);
                foreach (var item in localhosts)
                {
                    if (item.ToString().IndexOf("192.168") != -1)
                    {
                        serverData.IP = item.ToString();
                        break;
                    }
                }
                serverData.port = port;
            }
            catch (Exception)
            {
                port++;
                if (port < maxPort)
                {
                    BindServerSocket(port);
                }
                else
                {
                    throw;
                }
            }
        }


        public void Close()
        {

            open = false;


            //发送关闭房间信号
            GameNetPacket gameNetPacket = new GameNetPacket();
            gameNetPacket.name = "CloseServer";
            gameNetPacket.data = JsonUtility.ToJson(serverData);
            SendReceiveTo(JsonUtility.ToJson(gameNetPacket));

            //关闭服务器socket
            ServerSocket.Close();
            //关闭所有客户端的socket
            foreach (var item in ClientSockets)
            {
                item.Close();
            }
        }
    }




    /// <summary>
    /// 客户端  封装部分
    /// </summary>
    public partial class GameClient
    {
        /// <summary>
        /// 广播端口
        /// </summary>
        const int receiveMinPort = 2730;
        const int receiveMaxPort = 2735;


        /// <summary>
        /// 广播接收socket
        /// </summary>
        private Socket radiateSocket;
        /// <summary>
        /// 客户端的socket
        /// </summary>
        private Socket clientSocket;
        private EndPoint ep;
        private bool openCheckOutServer = true;




        /// <summary>
        /// 广播发送一条信息
        /// </summary>
        /// <param name="data"></param>
        private static void SendReceiveTo(string data)
        {
            //发送广播等待传输回来房间信息
            //初始化一个发送广播和指定端口的网络端口实例
            Socket sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);




            //设置该scoket实例的发送形式
            sendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            for (int i = receiveMinPort; i < receiveMaxPort; i++)
            {
                IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, i);
                byte[] buffer = Encoding.Unicode.GetBytes(data);
                //发送
                sendSocket.SendTo(buffer, iep);
            }
        }




        private void CloseCheckOutServer()
        {
            openCheckOutServer = false;
            radiateSocket.Close();
            radiateSocket = null;
        }




        private static IPEndPoint BindReceiverSocket(Socket receiveSocket, int port)
        {
            IPEndPoint sep = null;
            try
            {
                sep = new IPEndPoint(IPAddress.Any, port);//初始化一个侦听局域网内部所有IP和指定端口
                receiveSocket.Bind(sep);//绑定这个实例




            }
            catch (Exception)
            {
                port++;
                if (port < receiveMaxPort)
                {
                    BindReceiverSocket(receiveSocket, port);
                }
                else
                {
                    throw;
                }
            }
            return sep;
        }
    }



    /// <summary>
    /// 客户端 接口实现部分
    /// </summary>
    public partial class GameClient : IGameClient
    {
        private bool connecting;

        public GameClient()
        {
            radiateSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//初始化一个Scoket协议
            IPEndPoint sep = BindReceiverSocket(radiateSocket, receiveMinPort);//初始化一个侦听局域网内部所有IP和指定端口
            ep = (EndPoint)sep;//绑定这个实例
        }

        /// <summary>
        /// 查看服务器列表
        /// </summary>
        /// <param name="OnCheckOut"></param>
        public void CheckOutServerList(Action<ServerDataList> OnCheckOut)
        {
            openCheckOutServer = true;

            Debug.Log("查找房间");
            //广播发送一个数据
            SendReceiveTo(JsonUtility.ToJson(new GameNetPacket() { name = "Is Room?", data = "" }));

            ServerDataList serverDataList = new ServerDataList();

            //创建进程
            new Thread(() =>
            {
                while (openCheckOutServer)
                {
                    try
                    {
                        byte[] message = new byte[1024];//设置缓冲数据流
                        radiateSocket.ReceiveFrom(message, ref ep);//接收数据,并确把数据设置到缓冲流里面
                        string json = Encoding.Unicode.GetString(message).TrimEnd('\u0000');

                        GameNetPacket gameNetPacket = JsonUtility.FromJson<GameNetPacket>(json);

                        switch (gameNetPacket.name)
                        {
                            case "NewServer":
                                serverDataList.Add(JsonUtility.FromJson<ServerData>(gameNetPacket.data));
                                break;
                            case "CloseServer":
                                serverDataList.Remove(JsonUtility.FromJson<ServerData>(gameNetPacket.data));
                                break;
                            case "UpdateServer":
                                serverDataList.Update(JsonUtility.FromJson<ServerData>(gameNetPacket.data));
                                break;
                            default:
                                break;
                        }
                        OnCheckOut(serverDataList);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

            }).Start();
        }


        /// <summary>
        /// 关闭客户端
        /// </summary>
        public void Close()
        {
            if (openCheckOutServer)
            {
                CloseCheckOutServer();
            }
            if (connecting)
            {
                CloseConnecting();
            }
        }

        private void CloseConnecting()
        {
            connecting = false;
            clientSocket.Close();
        }



        public void SendMessageData(string data)
        {
            if (clientSocket == null)
            {
                Debug.Log("未连接服务器");
                return;
            }
            byte[] message = Encoding.UTF8.GetBytes(data);  //通信时实际发送的是字节数组，所以要将发送消息转换字节
            clientSocket.Send(message);
        }



        /// <summary>
        /// 连接至服务器
        /// </summary>
        /// <param name="serverData"></param>
        /// <param name="OnConnect"></param>
        /// <param name="OnReceive"></param>
        public void ConnectServer(ServerData serverData, Action<ServerData> OnConnect, Action<string> OnReceive)
        {


            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//使用指定的地址簇协议、套接字类型和通信协议
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverData.IP), serverData.port); // 用指定的ip和端口号初始化IPEndPoint实例
            clientSocket.Connect(serverEndPoint);

            new Thread(() =>
            {
                connecting = true;




                byte[] receive = new byte[1024];
                while (connecting)
                {
                    Debug.Log("等待消息");
                    int length = clientSocket.Receive(receive);  // length 接收字节数组长度
                    OnReceive(Encoding.UTF8.GetString(receive));
                }
            }).Start();
        }
    }
}
