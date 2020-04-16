using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int nBulletType; //子弹类型 1、角色 2、敌人
    private int nBulletStrength = 1; //子弹强度 1、打砖块 2、打铁块
    private int nSpeed = 8;
    private float fExplodeTime = 0;
    private bool bIsStopGame = false;
    // Start is called before the first frame update
    void Start()
    {
        //transform.gameObject.SendMessage("SetBulletType", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!bIsStopGame)
        {
            transform.Translate(Vector3.up * nSpeed * Time.deltaTime, Space.Self);
        }
    }

    public void SetPalyerType()
    {
        SetBulletType(1);
    }

    //设置子弹类型
    private void SetBulletType(int n)
    {
        //Debug.Log("SetBulletType: " + n);
        nBulletType = n;
    }

    //设置子弹速度
    private void SetBulletSpeed(int n)
    {
        nSpeed = n;
    }

    private void SetBulletStrength(int n)
    {
        nBulletStrength = n;
    }

    public void StopGame(bool bStop)
    {
        bIsStopGame = bStop;
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Player":
                if (nBulletType == 2)
                {
                    collision.SendMessage("Dead");
                    CreateExplode();
                }
                break;
            case "Enemy":
                if (nBulletType == 1)
                {
                    collision.SendMessage("Dead");
                    //CreateExplode();
                }
                break;
            case "Brick":
                //if (nBulletType == 1)
                //{
                    collision.SendMessage("Dead");
                //}
                CreateExplode();
                break;
            case "Iron":
                if (nBulletType == 1 && nBulletStrength == 2)
                {
                    collision.SendMessage("Dead");
                }
                CreateExplode();
                break;
            case "Boss":
                collision.SendMessage("Dead");
                CreateExplode(true);
                break;
            case "Grass":
                break;
            case "River":
                break;
            case "Bound": //外围墙直接销毁，不播放爆炸
                Destroy(transform.gameObject);
                break;
            default:
                break;
        }
    }

    //播放爆炸特效
    //bIsAudio:是否播放爆炸音效
    public void CreateExplode(bool bIsAudio = false)
    {
        if(Time.time - fExplodeTime > 0.1)
        {
            fExplodeTime = Time.time;
            GameObject goPrefab = (GameObject)Resources.Load("Prefabs/Effect/Explode");
            GameObject go = Instantiate(goPrefab, transform.position, Quaternion.Euler(Vector3.zero));
            if (bIsAudio)
            {
                go.SendMessage("PlayerAudio");
            }
        }
        Destroy(transform.gameObject);
    }
}
