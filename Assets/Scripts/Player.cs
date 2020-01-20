using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int nSpeed = 5;
    //private bool bHInRun = false;  //水平位置是否处于移动状态
    //private bool bVInRun = false;  //垂直位置是否处于移动状态
    //private bool bRuning = false;  //是否处于移动状态
    private Vector3 vDirection = Vector3.up; //移动的方向

    private GameObject goBorn;
    private bool bIsBorn = false;  //是否播放出生动画
    private float nBurnCdTime = 3;  //出生动画播放时间
    private float nBurnTime = 0;   //出生动画计时器

    //声音 
    private bool bIsRun = false;  //是否处于移动中
    private AudioClip audioClipRun; //音效 行走
    private AudioClip audioClipIde; //音效 待机
    private AudioClip audioClipDie; //音效 死亡
    private AudioSource audioSource; 

    private int nLife = 1;   //生命值
    // Start is called before the first frame update

    private bool bIsStopGame = false;  //游戏是否结束
    void Start()
    {
        //nLife = 3;
    }

    private void Update()
    {
       // UpdateProtectEffect();
    }
    private void FixedUpdate()
    {
        //Move();
    }

    public bool IsBurn
    {
        get { return bIsBorn; }
    }

    public Vector3 Direction
    {
        get { return vDirection; }
        set { vDirection = value; }
    }

    public int Life
    {
        get { return nLife; }
        set { nLife = value; }
    }

    public int Speed
    {
        set { nSpeed = value; }
    }

    public bool IsRun
    {
        get { return bIsRun; }
    }

    public bool IsStopGame
    {
        get { return bIsStopGame; }
        set { bIsStopGame = value; }
    }

    public void Move()
    {
        bool bRuning = true;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 && v == 0)
        {
            //Debug.Log("Move: " + h);
            Vector3 vDir = Vector3.right;
            if(h< 0)
            {
                vDir = Vector3.left;
            }
            if (vDirection != vDir)
            {
                vDirection = vDir;
                ChangeDirection();
            }
            MoveObj(h);
        }
        else if(v != 0){
            //Debug.Log("Move: " + v);
            Vector3 vDir = Vector3.up;
            if (v < 0)
            {
                vDir = Vector3.down;
            }
            if (vDirection != vDir)
            {
                vDirection = vDir;
                ChangeDirection();
            }
            MoveObj(v);
        }
        else
        {
            bRuning = false;
        }
        if(bRuning != bIsRun)
        {
            bIsRun = bRuning;
            SetAudio();
        }
    }

    //根据指定方向移动
    public void MoveByDir(Vector3 vecDir)
    {
        if (vecDir != Vector3.zero && vecDir != vDirection)
        {
            vDirection = vecDir;
            ChangeDirection();
        }
        Debug.Log("MoveByDir");
        MoveObj(0.5f);
        if (!bIsRun)
        {
            Debug.Log("bIsRun true");
            bIsRun = true;
            SetAudio();
        }
    }

    //结束移动
    public void MoveEnd()
    {
        if (bIsRun)
        {
            Debug.Log("bIsRun false");
            bIsRun = false;
            SetAudio();
        }

    }
    public void MoveObj(float nH )
    {
        transform.Translate(vDirection * Mathf.Abs(nH) * Time.deltaTime * nSpeed, Space.World);
    }

    //改变方向
    public void ChangeDirection()
    {
        int z = 0;
        if (vDirection == Vector3.up)
        {
            z = 0;
        }
        else if (vDirection == Vector3.down)
        {
            z = 180;
        }
        else if (vDirection == Vector3.right)
        {
            z = -90;
        }
        else if (vDirection == Vector3.left)
        {
            z = 90;
        }
        transform.rotation = Quaternion.Euler(0,0,z);
    }

    //播放出生动画
    public void CreateProtectEffect()
    {
        GameObject prefabsProtect = (GameObject)Resources.Load("Prefabs/effectBorn");
        goBorn = Instantiate(prefabsProtect, transform.position,Quaternion.Euler(Vector3.zero), transform);
        InitProtectEffect();
    }

    //初始化保护特效
    public void InitProtectEffect()
    {
        bIsBorn = true;
        goBorn.SetActive(bIsBorn);
        nBurnTime = 0;
    }

    //创建保护特效
    public void UpdateProtectEffect()
    {
        if (bIsBorn)
        {
            if (nBurnTime > nBurnCdTime)
            {
                bIsBorn = false;
                goBorn.SetActive(bIsBorn);
            }
            else
            {
                nBurnTime += Time.deltaTime;
            }
        }
    }

    //是否播放子弹音效
    public GameObject CreateBullet(bool bAudio = false)
    {
        GameObject goParent = GameObject.Find("objBullet");
        GameObject pbBullet = (GameObject)Resources.Load("Prefabs/Bullet");
        Vector3 dir = Direction * 0.7f;
        Vector3 pos = transform.position + dir;
        GameObject goBullet = Instantiate(pbBullet, pos, Quaternion.Euler(transform.eulerAngles), goParent.transform);
        if (bAudio)
        {
            AudioSource audioSource = goBullet.GetComponent<AudioSource>();
            if (audioSource)
            {
                audioSource.Play();
            }
        }
        return goBullet;
    }

    public void InitAudio()
    {
        audioClipRun = (AudioClip)Resources.Load("AudioSource/EngineDriving", typeof(AudioClip)); //行走音效
        audioClipIde = (AudioClip)Resources.Load("AudioSource/EngineIdle", typeof(AudioClip)); //待机音效
        audioClipDie = (AudioClip)Resources.Load("AudioSource/Die", typeof(AudioClip)); //死亡音效
        audioSource = transform.GetComponent<AudioSource>();
    }

    private void SetAudio()
    {
        if (bIsRun)
        {
            audioSource.clip = audioClipRun;
        }
        else
        {
            audioSource.clip = audioClipIde;
        }
        audioSource.Play();
    }

    public void StopGame(bool stopGame)
    {
        bIsStopGame = stopGame;
        PlayerAudip(!stopGame);
    }
    public void PlayerAudip(bool bPlay)
    {
        if (audioSource)
        {
            if (bPlay)
            {
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }
    }

    public void Dead()
    {
        audioSource.clip = audioClipDie;
        audioSource.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Enemy" || collision.gameObject.name == "Bound")
        {

        }
    }

}
