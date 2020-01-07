using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using LitJson;
using System.IO;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GameManager : MonoBehaviour
{
    //public GameManager Instance;

    //private int nBarrier = 1;
    public GameObject[] lsGo;
    public Transform trImgEndGame;
    //public AudioSource audioSource;
    //public AudioClip audioClipStart;
    //public AudioClip audioClipEnd;

    private DataSecene dataScene;
    private DataDifficult dataDifficult;
    private ArrayList alsBarrier;

    //（-10,8）
    public int nRowStart = -10; //行 起始点
    public int nColStart = 8; //列 起始点

    private int nBarrier = 1; //关卡数
    private int nAllBarrierNum = 0;//关卡总数

    public GameObject objParent = null;

    //单例
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    private void Awake()
    {
        instance = this;
        //获取关卡
        nBarrier = UnityEngine.PlayerPrefs.GetInt("Barrier");
        if (nBarrier == 0)
        {
            nBarrier = 1;
        }
    }


    //一个测试类
    class testJson
    {
        public string Name;

        public testJson(string m_name)
        {
            Name = m_name;
        }
    }

    List<testJson> jsonList = new List<testJson>();

    void Start()
    {
        //objParent = GameObject.Find("objGame");
        Debug.Log("11111111111111111111111");
        string strPath1 = "/Static/map.json";
        string strText = "";
        string url = Application.streamingAssetsPath + strPath1;
#if UNITY_EDITOR
        strText = File.ReadAllText(url);
#elif UNITY_ANDROID
        WWW www = new WWW(url);
        while (!www.isDone) { }
        strText = www.text //www.text.Split(new string[] {"\r\n"}, StringSplitOptions.None);
#endif
        Debug.Log("2222222222222222222");
        if (!string.IsNullOrEmpty(strText))
        {
            //StreamReader sr = new StreamReader(strPath);
            //string strJson = sr.ReadToEnd();//获取json文件里面的字符串
            Debug.Log("GameManager: Start  map: " + strText);
            JObject jo = (JObject)JsonConvert.DeserializeObject(strText);
            dataScene = JsonUtility.FromJson<DataSecene>(strText);
            //dataScene = JsonMapper.ToObject<DataSecene>(strText);
            Debug.Log("GameManager: Start  map Count: " + dataScene.map.Count);
            //JsonData id = JsonMapper.ToObject(strJson);
            //Debug.Log("id: " + id);
        }

        //Debug.Log("11111111111111111111111");
        //FileInfo m_file = new FileInfo(Application.persistentDataPath + "/map.json");
        //Debug.Log("222222222222222222222222222");
        ////判断   Exists 会返回是否存在
        //if (m_file.Exists)
        //{ 
        //    Debug.Log("333333333333333333333333");
        //    StreamReader sr = new StreamReader(Application.persistentDataPath + "/map.json");
        //    string nextLine;
        //    while ((nextLine = sr.ReadLine()) != null)
        //    {
        //        Debug.Log("44444:    "+ nextLine);
        //        jsonList.Add(JsonUtility.FromJson<testJson>(nextLine));
        //    }
        //    sr.Close();
        //}
        //else
        //{
        //    Debug.Log("55555555555555555555555");
        //}





        string strPath = "/Resource/Static/map.json";
//#if UNITY_EDITOR && !UNITY_ANDROID
        //Debug.Log("Application.dataPath: "+ Application.dataPath);
        //Debug.Log("Application.dataPath strPath: " + strPath);
//#endif

//#if !UNITY_EDITOR && UNITY_ANDROID
        //Debug.Log("Application.persistentDataPath: " + Application.persistentDataPath);
        //Debug.Log("Application.temporaryCachePath: "+ Application.temporaryCachePath);
        //Debug.Log("Application.streamingAssetsPath: " + Application.streamingAssetsPath);
        //#endif


        //GameObject go =  (GameObject)Resources.Load("Static/map");
        //读取map表
        string strJson = Util.UtilFile.Instance.GetDataFromFile(strPath);
        StreamReader reader = new StreamReader(Application.persistentDataPath + strPath);
        //string strJson = reader.ReadToEnd();
        if (!string.IsNullOrEmpty(strJson))
        {
            //StreamReader sr = new StreamReader(strPath);
            //string strJson = sr.ReadToEnd();//获取json文件里面的字符串
            Debug.Log("GameManager: Start  map: " + strJson);
            dataScene = JsonMapper.ToObject<DataSecene>(strJson);
            //JsonData id = JsonMapper.ToObject(strJson);
            //Debug.Log("id: " + id);
        }
        //读取Difficult表
        string strDifficultPath = "/Resource/Static/difficult.json";
        strJson = Util.UtilFile.Instance.GetDataFromFile(strDifficultPath);
        if ( !string.IsNullOrEmpty(strJson))
        {
            //StreamReader sr = new StreamReader(strDifficultPath);
            //string strJson = sr.ReadToEnd();//获取json文件里面的字符串
            Debug.Log("GameManager: Start  strDifficultPath: " + strJson);
            dataDifficult = JsonMapper.ToObject<DataDifficult>(strJson);
        }

        nAllBarrierNum = dataScene.map.Count;
        InitBarrier(nBarrier);
        PlayerManager.Instance.InitBornEffect(1);
    }

    public void InitBarrier(int n)
    {
        ObjBarrier obj = dataScene.GetBarrier(n);
        alsBarrier =  obj.GetBarrierList();
        CreateGameObject(alsBarrier);

        //创建敌人
        EnemyManager.Instance.SetBarrier(obj);
        EnemyManager.Instance.InitCreateEnemy();
    }

    public ObjDifficult GetObjDifficultByType(int nType)
    {
        return dataDifficult.GetDifficultByType(nType);
    }

    public void CreateGameObject(ArrayList alsBerrier)
    {
        for (int i = 0; i < alsBerrier.Count; i++)
        {
            ArrayList alsRow =(ArrayList)alsBerrier[i];
            for (int j = 0; j < alsRow.Count;j++ )
            {
                int nType = (int)alsRow[j];
                if (nType > 0)
                {
                    GameObject prefabObj = lsGo[nType -1];
                    Vector3 vecPos = new Vector3(j - 10, 8 - i, 0);
                    GameObject go =  Instantiate(prefabObj, vecPos, Quaternion.Euler(Vector3.zero), objParent.transform);

                    int nLife = 1;
                    if (nType == 3)//铁块
                    {
                        nLife = 2;
                    }
                    //Debug.Log("Type: " + nType);
                    go.SendMessage("SetLife", nLife);
                }
            }
        }
    }

    //创建新游戏
    public void CreateNewGame()
    {
        nBarrier++;
        if (nBarrier < nAllBarrierNum)
        {
            InitBarrier(nBarrier);
        }
        else
        {
            Debug.Log("通关全部关卡");
        }

    }

    public void EndGame()
    {
        StopGame(true);
        PlayrEndGameAction();
    }

    //播放停止游戏动画
    public void PlayrEndGameAction()
    {
        trImgEndGame.gameObject.SetActive(true);
        trImgEndGame.DOLocalMoveY(-15f, 5.0f).From(true);
        Invoke("LoadScene", 5f);
    }

    //暂停游戏
    public void StopGame(bool bIsStop)
    {
        GameObject[] arEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] arPlayer = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] arBullet = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject go in arEnemy)
        {
            go.SendMessage("StopGame", bIsStop);
        }
        foreach (GameObject go in arPlayer)
        {
            go.SendMessage("StopGame", bIsStop);
        }
        foreach (GameObject go in arBullet)
        {
            go.SendMessage("StopGame", bIsStop);
        }
    }
    private void LoadScene()
    {
        SceneManager.LoadScene(0);
    }
}