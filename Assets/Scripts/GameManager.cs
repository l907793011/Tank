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
    public Transform tfImgEndGame;
    //public AudioSource audioSource;
    //public AudioClip audioClipStart;
    //public AudioClip audioClipEnd;
    public Transform tfImgThrough;
    public UnityEngine.UI.Text txtNum1;
    public UnityEngine.UI.Text txtNum2;
    public UnityEngine.UI.Text txtNum3;
    public UnityEngine.UI.Text txtScore1;
    public UnityEngine.UI.Text txtScore2;
    public UnityEngine.UI.Text txtScore3;
    public UnityEngine.UI.Text txtStageNum;
    public UnityEngine.UI.Text txtSorceNum;
    public UnityEngine.UI.Text txtTotalNum;

    public UnityEngine.UI.Button btnNext;

    private DataSecene dataScene;
    private DataDifficult dataDifficult;
    private ArrayList alsBarrier;

    //（-10,8）
    public int nRowStart = -10; //行 起始点
    public int nColStart = 8; //列 起始点

    private int nBarrier = 1; //关卡数
    private int nAllBarrierNum = 0;//关卡总数

    public GameObject objParent = null;


    //结算界面相关
    private int nScoreSimple = 0;
    private int nScoreFast = 0;
    private int nScoreHard = 0;

    private int nCurSimple = 0;
    private int nCurFast = 0;
    private int nCurHard = 0;

    private int nDifNum = 1;

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
        btnNext.onClick.AddListener(BtnNextClick);
    }

    void Start()
    {
        //objParent = GameObject.Find("objGame");

        string strPath1 = "Static/map";
        //string strPath = "Resources/Static/map.json";
        TextAsset go =  (TextAsset)Resources.Load("Static/map");
        //读取map表
        string strJson = Util.UtilFile.Instance.GetDataFromFile(strPath1);
        //Byte[] byteJson = Scx.FileUtils.Instance.GetDataFromFile(strPath);
        Debug.Log("GameManager strJson: " + strJson);
        //StreamReader reader = new StreamReader(Application.persistentDataPath + strPath);
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
        string strDifficultPath = "Static/difficult";
        strJson = Util.UtilFile.Instance.GetDataFromFile(strDifficultPath);
        Debug.Log("GameManager strJson: " + strJson);
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
                    CreateGameObjectByType(nType, j - 10, 8 - i);
                }
            }
        }
    }

    public void CreateGameObjectByType(int nType,float x,float y)
    {
        GameObject prefabObj = lsGo[nType - 1];
        Vector3 vecPos = new Vector3(x, y, 0);
        GameObject go = Instantiate(prefabObj, vecPos, Quaternion.Euler(Vector3.zero), objParent.transform);

        int nLife = 1;
        if (nType == 3)//铁块
        {
            nLife = 2;
        }
        //Debug.Log("Type: " + nType);
        go.SendMessage("SetLife", nLife);
    }

    //获取boss城墙,修改为铁块
    public ArrayList GetArBossBound(int nType)
    {
        string strTag = "BrickAll";
        if (nType == 1)
        {
            strTag = "Iron";
        }
        ArrayList arBossBound = new ArrayList();
        GameObject[] arBound = GameObject.FindGameObjectsWithTag(strTag);

        foreach (GameObject go in arBound)
        {
            Vector3 vecPos = go.transform.position;
            if (vecPos.x > -3 && vecPos.x < 3 && vecPos.y < -6)
            {
                arBossBound.Add(go);
            }
        }
        return arBossBound;
    }

    public void ChangeBossBoundToType(int nType,float nTime = 0)
    {
        ArrayList arBossBound = GetArBossBound(nType);
        foreach (GameObject go in arBossBound)
        {
            Vector3 vecPos = go.transform.position;
            CreateGameObjectByType(nType, vecPos.x, vecPos.y);
            Destroy(go);
        }
        if (nTime > 0 )
        {
            StopCoroutine("RecoverBossBound");
            Invoke("RecoverBossBound", nTime);
        }
    }

    public void RecoverBossBound()
    {
        ChangeBossBoundToType(1);
    }

    //创建新游戏
    public void CreateNewGame()
    {
        tfImgThrough.gameObject.SetActive(false);
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
        Debug.Log("EndGame: true");
        StopGame(true);
        PlayrEndGameAction();
    }

    //播放停止游戏动画
    public void PlayrEndGameAction()
    {
        tfImgEndGame.gameObject.SetActive(true);
        tfImgEndGame.DOLocalMoveY(-15f, 5.0f).From(true);
        Invoke("LoadScene", 5f);
    }

    //暂停游戏
    public void StopGame(bool bStopEnemy,bool bStopPlayer = true, bool bStopBullet = true,float nTime = 0)
    {
        GameObject[] arEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] arPlayer = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] arBullet = GameObject.FindGameObjectsWithTag("Bullet");

        foreach (GameObject go in arEnemy)
        {
            go.SendMessage("StopGame", bStopEnemy);
        }
        foreach (GameObject go in arPlayer)
        {
            go.SendMessage("StopGame", bStopPlayer);
        }
        foreach (GameObject go in arBullet)
        {
            go.SendMessage("StopGame", bStopBullet);
        }

        if(nTime > 0)
        {
            Invoke("StartGame", nTime);
        }
    }

    public void StartGame()
    {
        Debug.Log("StartGame: false");
        StopGame(false);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(0);
    }

    //通关游戏
    public void ThroughGame()
    {
        tfImgThrough.gameObject.SetActive(true);
        Debug.Log("ThroughGame: true");
        StopGame(true);
        OnShowThroughInfo();
    }

    //显示通关信息
    private void OnShowThroughInfo()
    {
        int nNumSimple = EnemyManager.Instance.NumSimple;
        int nNumFast = EnemyManager.Instance.NumFast;
        int nNumHard = EnemyManager.Instance.NumHard;
        nScoreSimple = nNumSimple * 100;
        nScoreFast = nNumFast * 200;
        nScoreHard = nNumHard * 300;
        //private int nCurSimple = 0;
        //private int nCurFast = 0;
        //private int nCurHard = 0;

        txtStageNum.text = nBarrier.ToString();
        txtSorceNum.text = (nScoreSimple + nScoreFast+ nScoreHard).ToString();
        txtTotalNum.text = (nNumSimple + nNumFast + nNumHard).ToString();

        txtNum1.text = nNumSimple.ToString();
        txtNum2.text = nNumFast.ToString();
        txtNum3.text = nNumHard.ToString();
        txtScore1.text = nCurSimple.ToString();
        txtScore2.text = nCurFast.ToString();
        txtScore3.text = nCurHard.ToString();

        InvokeRepeating("OnShowScoreSimple", 0.01f, 0.01f);
        InvokeRepeating("OnShowScoreNumFast", 0.02f, 0.01f);
        InvokeRepeating("OnShowScoreHard", 0.03f, 0.01f);
    }

    private void OnShowScoreSimple()
    {
        if (nCurSimple >= nScoreSimple)
        {
            CancelInvoke("OnShowScoreSimple");
        }
        else
        {
            nCurSimple += nDifNum;
            txtScore1.text = nCurSimple.ToString();
        }

    }

    private void OnShowScoreNumFast()
    {
        if (nCurFast >= nScoreFast)
        {
            CancelInvoke("OnShowScoreNumFast");
        }
        else
        {
            nCurFast += nDifNum;
            txtScore2.text = nCurFast.ToString();
        }
    }

    private void OnShowScoreHard()
    {
        if (nCurHard >= nScoreHard)
        {
            CancelInvoke("OnShowScoreHard");
        }
        else
        {
            nCurHard += nDifNum;
            txtScore3.text = nCurHard.ToString();
        }
    }

    private void BtnNextClick()
    {
        CreateNewGame();
    }
}