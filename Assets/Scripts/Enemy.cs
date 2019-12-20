using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Player
{
    private float fChangeDirCdTime = 2; //改变方向cd时间
    private float fCurChangeTime = 0;   //改变方向当前累计时间
    private float fAttackCdTime = 3;   //发射子弹cd时间
    private float fAttackTime = 0;

    //private Vector3 vecDirection = Vector3.down;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangDir();
        Attack();
    }

    private void Dead()
    {
        Debug.Log("Enemy Dead");
    }

    void ChangDir()
    {
        if(fCurChangeTime >= fChangeDirCdTime)
        {
            fCurChangeTime = 0;
            Vector3 vecDir = RandomDir();
            if (vecDir != Direction)
            {
                Direction = vecDir;
                ChangeDirection();
            }
        }
        else
        {
            fCurChangeTime += Time.deltaTime;
        }
    }

    //随机一个方向
    private Vector3 RandomDir()
    {
        int n = Random.Range(1, 11);
        Vector3 vecDir = Vector3.down;
        if (n >= 1 && n < 3) //12
        {
            vecDir = Vector3.up;
        }
        else if (n >= 3 && n < 7)//3456
        {
            vecDir = Vector3.down;
        }
        else if (n >= 7 && n < 9)//78
        {
            vecDir = Vector3.left;
        }
        else //9 10
        {
            vecDir = Vector3.right;
        }
        return vecDir;
    }

    private void Attack()
    {
        if (fAttackTime >= fAttackCdTime)
        {
            fAttackTime = 0;
            GameObject goBullet = CreateBullet();
            goBullet.SendMessage("SetBulletType", 2);
        }
        else
        {
            fAttackTime += Time.deltaTime;
        }
    }
}
