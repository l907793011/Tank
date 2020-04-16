using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Player
{
    private float fChangeDirCdTime = 2; //改变方向cd时间
    private float fCurChangeTime = 0;   //改变方向当前累计时间
    private float fAttackCdTime = 3;   //发射子弹cd时间
    private float fAttackTime = 0;
    private int nColorType = 0;  //颜色类型 0、普通 1、普通红色 2、快速 3、快速红色 4、高级白色  5、高级红色 6、高级黄色 7、高级绿色

    //private Vector3 vecDirection = Vector3.down;
    // Start is called before the first frame update
    void Start()
    {
        Direction = Vector3.down;
        Life = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //ChangDir();
        //Attack();
        //MoveObj(0.3f);
    }

    private void FixedUpdate()
    {
        if (!IsStopGame)
        {
            ChangDir();
            Attack();
            MoveObj(0.3f);
        }
       
    }

    //nType  1、必须转向
    void ChangDir(int nType = 0)
    {
        if(fCurChangeTime >= fChangeDirCdTime)
        {
            fCurChangeTime = 0;
            Vector3 vecDir = RandomDir(nType);
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

    //随机一个方向 1、必须转向
    private Vector3 RandomDir(int nType)
    {
        Vector3 vecDir = Vector3.zero;
        do
        {
            int n = Random.Range(1, 11);
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
            //Debug.Log("nType: "+ nType+ "Vector3.zero vecDir:  "+ vecDir.ToString() + "   Direction:  "+ Direction.ToString());
        } while ((nType == 0 && vecDir == Vector3.zero) || (nType == 1 && vecDir == Direction));
        
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

    public void SetColorType(int nType)
    {
        nColorType = nType;
    }

    public void SetLife(int nLife)
    {
        Life = nLife;
    }

    public void SetSpeed(int nSpeed)
    {
        Speed = nSpeed;
    }

    public void Dead(bool bIsAllDead = false)
    {
        Life--;
        if (Life <= 0)
        {
            CreateExplode();
            EnemyManager.Instance.EnemyDead(nColorType); //销毁
            if (!bIsAllDead)
            {
                EnemyManager.Instance.EnemyRemove(transform.gameObject);
            }
            Destroy(transform.gameObject);
        }
        else if(Life == 1) //只剩下最后一滴血的时候，变成白色
        {
            Debug.Log("改变颜色");
            //EnemyManager.Instance.ChangeColor(transform.gameObject, nColorType);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Bound")
        {
            //Debug.Log("OnCollisionEnter2D  Enemy  ChangeDirection");
            fCurChangeTime += fChangeDirCdTime;
            ChangDir(1);
        }
    }
}
