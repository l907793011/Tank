using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using UnityEngine;

namespace Assets.Util
{

    public class GameNetWorkManager : MonoBehaviour
    {
        private GameClient gameClient;
        private ServerData serverData;
        private GameServer gameServer;
        private ServerDataList serverDataList;
        public ServerData ServerData { get => serverData; }
        public GameClient GameClient { get => gameClient; }
        public GameServer GameServer { get => gameServer; }
        public ServerDataList ServerDataList { get => serverDataList; }
        /// <summary>
        /// 当收到服务器消息时
        /// </summary>
        public event System.Action<string> OnReceive;
        /// <summary>
        /// 创建服务器
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="maxNumber"></param>
        public void CreateServer(string serverName, int maxNumber)
        {
            serverData = new ServerData(serverName, maxNumber);
            gameServer = new GameServer(serverData);
            gameClient.ConnectServer(serverData, (ServerData ServerData) =>
            {
                //初始化随机种子
                UnityEngine.Random.InitState(ServerData.randomSeed);
            },
            (string message) =>
            {
                OnReceive(message);
            });
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="serverData"></param>
        public void ConnectServer(ServerData serverData)
        {
            gameClient.ConnectServer(serverData, (ServerData ServerData) =>
            {
                //初始化随机种子
                UnityEngine.Random.InitState(ServerData.randomSeed);
            },
            (string message) =>
            {
                OnReceive(message);
            });
        }
        public void SendData(string message)
        {
            gameClient.SendMessageData(message);
        }
        private void Awake()
        {

            //创建客户端
            gameClient = new GameClient();
            //扫描服务器(即收到广播就进行数据更新)
            GameClient.CheckOutServerList((ServerDataList serverDataList) =>
            {
                this.serverDataList = serverDataList;
            });
        }
        private void OnDestroy()
        {
            if (gameClient != null)
            {
                gameClient.Close();
            }
            if (gameServer != null)
            {
                gameServer.Close();
            }
        }
    }
}
