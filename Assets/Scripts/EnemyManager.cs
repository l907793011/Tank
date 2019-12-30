using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//敌人管理器
public class EnemyManager : MonoBehaviour
{
    private Vector3[] lsPos = {new Vector3(-10,8,0), new Vector3(0, 8, 0), new Vector3(10, 8, 0) };
    private int nLastPos = 0; //上次随机的位置
    private int nEnemyAllNum = 1; //敌人总数量
    private int nRemainNum = 0;//剩余敌人数量
    private int nCurNum = 0;   //当前的敌人数量
    private int nNumInTime = 6; //当前同时在线的敌人数量
    public GameObject[] objEnemySimple; //预制件列表 10、普通 11、普通红色 
    public GameObject[] objEnemyFast; //预制件列表  20、快速 21、快速红色 
    public GameObject[] objEnemyHard; //预制件列表   30、快速红色 31、高级白色  32、高级红色 33、高级黄色 34、高级绿色
    private int nAllRandomNum = 0; //随机总数
    private ObjBarrier objBarrier;//当前关卡数据
    private ArrayList arlDifficult; //当前关卡难度列表

    private float nCdTime = 3; //创建敌人的间隔时间
    private float nCurTime = 0;//创建敌人的计时器

    //敌人图标信息
    public float nStartX = -34;
    public float nStartY = -63;
    public float nSpace = 18;
    private ArrayList arIcon = new ArrayList();


    public Transform tfParent;
    public Transform tfParentIcon;//敌人图标父节点

    // Start is called before the first frame update

    private static EnemyManager instance;
    public static EnemyManager Instance
    {
        get { return instance; }
        set { instance = value; }
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitEnemyInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (nCurNum < nNumInTime)
        {
            if (nCurTime >= nCdTime)
            {
                CreateEnemy();
                nCurTime = 0;
            }
            else
            {
                nCurTime += Time.deltaTime;
            }
        }
    }

    public void InitEnemyInfo()
    {
        nRemainNum = nEnemyAllNum;
        nCurNum = 0;
        InitEnemyIcon();
    }

    private void InitEnemyIcon()
    {
        GameObject prefIcon = (GameObject)Resources.Load("Prefabs/UI/ImgEnemy");
        for (int i = 0;i<nEnemyAllNum;i++)
        {
            Vector3 vecPos = Vector3.zero;
            vecPos.x = nStartX + i % 4 * nSpace;
            vecPos.y = nStartY - i / 4 * nSpace;
            GameObject goIcon = Instantiate(prefIcon, vecPos,Quaternion.Euler(Vector3.zero),tfParentIcon);
            arIcon.Add(goIcon);
        }
    }

    //获取随机位置
    private Vector3 GetPos()
    {
        Vector3 vecPos = Vector3.zero;
        int nRang = Random.Range(0, 3);
        while (nLastPos == nRang)
        {
            nRang = Random.Range(0, 3);
        }
        vecPos = lsPos[nRang];
        nLastPos = nRang;
        return vecPos;
    }

    public void SetBarrier(ObjBarrier barrier)
    {
        objBarrier = barrier;
        nAllRandomNum = objBarrier.GetDifficultAllNum();
        arlDifficult = objBarrier.GetDifficultList();
    }

    //初始化敌人
    public void InitCreateEnemy()
    {
        for (int i = 0; i < 3; i++)
        {
            CreateEnemy();
        }
     }


    //创建敌人
    public void CreateEnemy()
    {
        int n = Random.Range(0, nAllRandomNum);
        foreach (ArrayList arDifficult in arlDifficult)
        {
            int nType = (int)arDifficult[0];
            int nDifficult = (int)arDifficult[1];
            if (n < nDifficult)
            {
                CreateEnemyByType(nType);
                nCurNum++;
                break;
            }
            else
            {
                n -= nDifficult;
            }
        }
    }

    private void CreateEnemyByType(int nType)
    {
        GameObject go =  GetGameObjectByType(nType);
    }

    private GameObject GetGameObjectByType(int nType)
    {
        GameObject goPrefab = null;
        int nHardType = nType / 10;  //大类型
        int nIndex = nType % 10;  //类型中的序号
        switch (nHardType)
        {
            case 1:  //简单 10、普通 11、普通红色
                goPrefab = objEnemySimple[nIndex];
                break;
            case 2: //快速  20、快速 21、快速红色 
                goPrefab = objEnemyFast[nIndex];

                break;
            case 3: //皮厚 30、高级白色  31、高级红色 32、高级黄色 33、高级绿色
                goPrefab = objEnemyFast[nIndex];
                break;
            default:
                goPrefab = objEnemySimple[nIndex];
                break;
        }
        Vector3 vecPos = GetPos();
        GameObject go = Instantiate(goPrefab, vecPos, Quaternion.Euler(Vector3.back * 180), tfParent);
        go.SendMessage("SetColorType", nType);

        ObjDifficult objDiffcult = GameManager.Instance.GetObjDifficultByType(nType);
        if (objDiffcult != null)
        {
            go.SendMessage("SetLife", objDiffcult.Life());
            go.SendMessage("SetSpeed", objDiffcult.Speed());
        }
        
        return go;
    }

    public void ChangeColor(GameObject go,int nType)
    {
        string strName = "Enemy1";
        switch (nType)
        {
            case 0:  //简单 0、普通 1、普通红色
            case 1:
                strName = "Enemy1";
                break;
            case 2: //快速  2、快速 3、快速红色 
            case 3:
                strName = "Enemy2";
                break;
            case 4: //皮厚 4、高级白色  5、高级红色 6、高级黄色 7、高级绿色
            case 5:
            case 6:
            case 7:
                strName = "Enemy3";
                break;
            default:
                strName = "Enemy1";
                break;
        }

    }

    //全部怪物死亡
    public void EnemyDead()
    {
        nCurNum--;
        nRemainNum--;
        GameObject go = (GameObject)arIcon[nRemainNum];
        if (go)
        {
            go.SetActive(false);
        }
        if (nRemainNum <= 0)
        {
            Debug.Log("恭喜你，通关了");
            GameManager.Instance.CreateNewGame();
            return;
        }
    }
}
