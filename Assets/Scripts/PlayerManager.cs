using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//玩家管理器
public class PlayerManager : MonoBehaviour
{
    public Button btnUp;
    public Button btnDown;
    public Button btnLeft;
    public Button btnRight;
    public Button btnAttack;
    public Button btnStop;
    public Button btnStart;
    public Text txtLeftLife;
    public Text txtRightLife;


    private bool bLeftDead = false;     //左侧玩家是否死亡
    private bool bRightDead = false;    //右侧玩家是否死亡
    private float nDeadCdTime = 3;      //死亡间隔cd
    private float nLeftDeadTime = 0;    //左侧玩家死亡时间
    private float nRightDeadTime = 0;   //右侧玩家死亡时间

    private int nLeftMaxLife = 1;
    private int nLeftCurLife = 0;
    private int nRightMaxLife = 0;
    private int nRightCurLife = 0;


    private GameObject goBornLeft;
    private GameObject goBornRight;

    private GameObject goPlayer;

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
        nLeftCurLife = nLeftMaxLife;
        nRightCurLife = nRightMaxLife;
        txtLeftLife.text = nLeftCurLife.ToString();
        txtRightLife.text = nRightCurLife.ToString();


        btnAttack.onClick.AddListener(BtnAttackClick);
        btnStop.onClick.AddListener(BtnStopClick);
        btnStart.onClick.AddListener(BtnStartClick);
        EventTrigger triggetBtnUp = btnUp.GetComponent<EventTrigger>();

        CreateEntryByButton(btnUp, BtnUpDownClick, EndMove);
        CreateEntryByButton(btnDown, BtnDownDownClick, EndMove);
        CreateEntryByButton(btnLeft, BtnLeftDownClick, EndMove);
        CreateEntryByButton(btnRight, BtnRightDownClick, EndMove);
    }

    private void CreateEntryByButton(Button btn,UnityEngine.Events.UnityAction<BaseEventData> callDown, UnityEngine.Events.UnityAction<BaseEventData> callUp)
    {
        EventTrigger triggerBtn = btn.GetComponent<EventTrigger>();
        CreateEntry(triggerBtn,EventTriggerType.PointerDown,callDown);
        CreateEntry(triggerBtn,EventTriggerType.PointerUp,callUp);

    }
    private void CreateEntry(EventTrigger triggerBtn, EventTriggerType nType, UnityEngine.Events.UnityAction<BaseEventData> call)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = nType;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(call);
        triggerBtn.triggers.Add(entry);
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
        goPlayer =  Instantiate(prefab, vecPos, Quaternion.Euler(Vector3.zero));
        //go.transform.SetParent(null);
    }

    //死亡后创建新角色
    public void CreateNewPlayer(int nType)
    {
        if (nType == 1)
        {
            //bLeftDead = true;
            // nLeftDeadTime = 0;
            if (nLeftCurLife > 0)
            {
               Invoke("CreatLeftPlayer", nDeadCdTime);
            }
        }
        else if(nType == 2)
        {
            //bRightDead = true;
            //nRightDeadTime = 0;
            if (nRightCurLife > 0)
            {
                Invoke("CreatRightPlayer", nDeadCdTime);
            }
        }

        if (nLeftCurLife <= 0 && nRightCurLife <= 0)
        {
            GameObject boss = GameObject.FindGameObjectWithTag("Boss");
            boss.SendMessage("Dead");
            //GameManager.Instance.EndGame();
        }
    }

    private void CreatLeftPlayer()
    {
        nLeftCurLife--;
        txtLeftLife.text = nLeftCurLife.ToString();
        CreatePlayer(1);
    }

    private void CreatRightPlayer()
    {
        nRightCurLife--;
        txtRightLife.text = nRightCurLife.ToString();
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

    private void EndMove(BaseEventData eventData)
    {
        //Debug.Log("Manager EndMove");
        if (goPlayer)
        {
            goPlayer.SendMessage("MoveEnd");
        }
    }

    private void BtnUpDownClick(BaseEventData eventData)
    {
        //Debug.Log("Manager BtnUpDownClick");
        if (goPlayer)
        {
            goPlayer.SendMessage("MoveByDir", Vector3.up);
        }
    }

    private void BtnDownDownClick(BaseEventData eventData)
    {
        if (goPlayer)
        {
            goPlayer.SendMessage("MoveByDir", Vector3.down);
        }
    }

    private void BtnLeftDownClick(BaseEventData eventData)
    {
        if (goPlayer)
        {
            goPlayer.SendMessage("MoveByDir", Vector3.left);
        }
    }

    private void BtnRightDownClick(BaseEventData eventData)
    {
        if (goPlayer)
        {
            goPlayer.SendMessage("MoveByDir", Vector3.right);
        }
    }

    private void BtnAttackClick()
    {
        if (goPlayer)
        {
            goPlayer.SendMessage("Attack");
        }
    }
    private void BtnStopClick()
    {
        GameManager.Instance.StopGame(true);
        btnStop.gameObject.SetActive(false);
        btnStart.gameObject.SetActive(true);
    }
    private void BtnStartClick()
    {
        GameManager.Instance.StopGame(false);
        btnStop.gameObject.SetActive(true);
        btnStart.gameObject.SetActive(false);
    }

}
