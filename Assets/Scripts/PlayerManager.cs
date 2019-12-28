using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//玩家管理器
public class PlayerManager : MonoBehaviour
{
    private bool bLeftDead = false;     //左侧玩家是否死亡
    private bool bRightDead = false;    //右侧玩家是否死亡
    private float nDeadCdTime = 3;      //死亡间隔cd
    private float nLeftDeadTime = 0;    //左侧玩家死亡时间
    private float nRightDeadTime = 0;   //右侧玩家死亡时间


    private GameObject goBornLeft;
    private GameObject goBornRight;

    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (bLeftDead)
        {
            if (nLeftDeadTime >= nDeadCdTime)
            {
                bLeftDead = false;
                CreatePlayer(1);
            }
        }

        if (bRightDead)
        {
            if (nRightDeadTime >= nDeadCdTime)
            {
                bRightDead = false;
                CreatePlayer(2);
            }
        }

    }

    //创建指定位置的角色1、左侧 2、右侧
    public void CreatePlayer(int nType)
    {
        GameObject prefab = null;
        Vector3 vecPos = Vector3.zero;
        switch(nType)
        {
            case 1:
                if (goBornLeft)
                {
                    Destroy(goBornLeft);
                }

                prefab = (GameObject)Resources.Load("Prefabs/player1", typeof(GameObject));
                vecPos = new Vector3(-2, -8, 0);
                break;
            case 2:
                if (goBornRight)
                {
                    Destroy(goBornRight);
                }
                prefab = (GameObject)Resources.Load("Prefabs/player2",typeof(GameObject));
                vecPos = new Vector3(2, -8, 0);
                break;
            default:
                break;
        }
        //Scene scene = SceneManager.GetActiveScene();
        GameObject go =  Instantiate(prefab, vecPos, Quaternion.Euler(Vector3.zero));
        go.transform.SetParent(null);
    }

    //死亡后创建新角色
    public void CreateNewPlayer(int nType)
    {
        if (nType == 1)
        {
            //bLeftDead = true;
           // nLeftDeadTime = 0;
            Invoke("CreatLeftPlayer", nDeadCdTime);
        }
        else if(nType == 2)
        {
            //bRightDead = true;
            //nRightDeadTime = 0;
            Invoke("CreatRightPlayer", nDeadCdTime);
        }
    }

    private void CreatLeftPlayer()
    {
        CreatePlayer(1);
    }

    private void CreatRightPlayer()
    {
        CreatePlayer(2);
    }

    public void InitBornEffect(int nType)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/Effect/Born", typeof(GameObject));
        Vector3 vecPos = new Vector3(-2, -8, 0);
        goBornLeft = Instantiate(prefab, vecPos, Quaternion.Euler(Vector3.zero));

        string strMoth = "CreatLeftPlayer";
        if(nType == 2)
        {
            strMoth = "CreatRightPlayer";
            vecPos = new Vector3(2, -8, 0);
            goBornRight = Instantiate(prefab, vecPos, Quaternion.Euler(Vector3.zero));
        }
        Invoke(strMoth, 0.9f);
    }

    //public void CreateNewPlayer(int nType)
    //{
    //    Invoke("CreatePlayer", 2f);
    //}

}
