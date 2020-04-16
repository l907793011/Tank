using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class PlayerLeft : Player
{
    public GameObject effectBurn;
    private float nAttackCdTime = 0.2f; //攻击间隔时间
    private float nAttackTime = 0;  //攻击当前计时
    // Start is called before the first frame update
    void Start()
    {
        InitAudio();
        CreateProtectEffect();
    }

    // Update is called once per frame
    void Update()
    {
        //PlayBurn();
       
    }

    private void FixedUpdate()
    {
        if (!IsStopGame)
        {
            //Move();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Attack();
            }
            UpdateProtectEffect();
            nAttackTime += Time.deltaTime;
        }
        //Debug.Log("FixedUpdate Left" + IsRun.ToString());
        if (IsRun)
        {
            Debug.Log("PlayerLeft");
            MoveByDir(Vector3.zero);
        }
    }

    public void Attack()
    {
        if (nAttackTime < nAttackCdTime)
        {
            return;
        }
        nAttackTime = 0;
        //GameObject goParent =  GameObject.Find("objBullet");
        //GameObject pbBullet = (GameObject)Resources.Load("Prefabs/Bullet");
        //Vector3 dir = Direction * 0.7f;
        //Vector3 pos = transform.position + dir;
        //GameObject goBullet = Instantiate(pbBullet, pos, Quaternion.Euler(transform.eulerAngles), goParent.transform);
        GameObject goBullet = CreateBullet(true);
        goBullet.SendMessage("SetBulletType", 1);
    }

    private void Dead()
    {
        base.Dead();
        if (IsProtect)//保护期
        {
            return;
        }
        else
        {
            Life--;
            if (Life <= 0)
            {
                PlayerManager.Instance.CreateNewPlayer(1);
                Destroy(transform.gameObject);
            }
        }
        Debug.Log("Player Left Dead");
    }
    //碰撞检车
    public void OnCollisionEnter2D(Collision2D collision)
    {
        string strName = collision.gameObject.name;
        switch (strName)
        {
            case "Bomb": //炸弹
                EnemyManager.Instance.AllEnemyDead();
                Destroy(collision.gameObject);
                break;
            case "IronBuff"://boss保护墙变铁块
                break;
            case "AddLife"://生命加1
                Life++;
                break;
            case "Protect"://添加保护罩
                break;
            case "Star"://子弹强化
                break;
            case "Stop"://怪物暂停
                break;
        }
    }
}
