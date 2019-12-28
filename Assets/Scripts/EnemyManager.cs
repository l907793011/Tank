using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//敌人管理器
public class EnemyManager : MonoBehaviour
{
    private Vector3[] lsPos = {new Vector3(-10,8,0), new Vector3(0, 8, 0), new Vector3(10, 8, 0) };
    private int nLastPos = 0; //上次随机的位置
    private int nEnemyAllNum = 20; //敌人总数量
    private int nCurNum = 0;
    public GameObject[] objEnemy; //预制件列表 0、普通 1、普通红色 2、快速 3、快速红色 4、高级白色  5、高级红色 6、高级黄色 7、高级绿色
    private int nAllRandomNum = 0; //随机总数
    private ObjBarrier objBarrier;//当前关卡数据
    private ArrayList arlDifficult; //当前关卡难度列表

    public Transform tfParent;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        for (int i = 0; i < 5; i++)
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
        int nIndex = 0;
        int nLife = 1;
        int nSpeed = 8;
        switch (nType)
        {
            case 1:  //简单 0、普通 1、普通红色
                nIndex = Random.Range(0, 2);
                nLife = 1;
                break;
            case 2: //快速  2、快速 3、快速红色 
                nIndex = Random.Range(2, 4);
                nLife = 1;
                nSpeed = 14;
                break;
            case 3: //皮厚 4、高级白色  5、高级红色 6、高级黄色 7、高级绿色
                nIndex = Random.Range(5, 7);
                nLife = 3;
                break;
            default:
                nIndex = 0;
                nLife = 1;
                break;
        }
        goPrefab = objEnemy[nIndex];
        Vector3 vecPos = GetPos();
        GameObject go = Instantiate(goPrefab, vecPos, Quaternion.Euler(Vector3.back * 180), tfParent);
        go.SendMessage("SetColorType", nIndex);
        go.SendMessage("SetLife", nLife);
        go.SendMessage("SetSpeed", nSpeed);
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

        if (nCurNum >= nEnemyAllNum)
        {
            Debug.Log("恭喜你，通关了");
            return;
        }
        else
        {
            CreateEnemy();
        }
    }
}
