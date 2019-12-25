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

    private GameObject goBurn;
    private bool bIsBurn = false;  //是否播放出生动画
    private float nBurnCdTime = 3;  //出生动画播放时间
    private float nBurnTime = 0;   //出生动画计时器

    private int nLife = 1;   //生命值
    // Start is called before the first frame update
    void Start()
    {
        nLife = 3;
        InitProtectEffect();
    }

    private void Update()
    {
        //UpdateProtectEffect();
    }
    private void FixedUpdate()
    {
        //Move();
    }

    public bool IsBurn
    {
        get { return bIsBurn; }
    }

    public Vector3 Direction
    {
        get { return vDirection; }
        set { vDirection = value; }
    }

    public int Life
    {
        get{ return nLife; }
        set{ nLife = value; }
    }

    public void Move()
    {
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
    public void InitProtectEffect()
    {
        GameObject prefabsProtect = (GameObject)Resources.Load("Prefabs/bullet");
        GameObject goBurn = Instantiate(prefabsProtect,Vector3.zero,Quaternion.Euler(Vector3.zero), transform);
        bIsBurn = true;
        goBurn.SetActive(bIsBurn);
        nBurnTime = 0;
    }

    //创建保护特效
    public void UpdateProtectEffect()
    {
        if (bIsBurn)
        {
            if (nBurnTime > nBurnCdTime)
            {
                bIsBurn = false;
                goBurn.SetActive(bIsBurn);
            }
            else
            {
                nBurnTime += Time.deltaTime;
            }
        }
    }

    public GameObject CreateBullet()
    {
        GameObject goParent = GameObject.Find("objBullet");
        GameObject pbBullet = (GameObject)Resources.Load("Prefabs/bullet");
        Vector3 dir = Direction * 0.7f;
        Vector3 pos = transform.position + dir;
        GameObject goBullet = Instantiate(pbBullet, pos, Quaternion.Euler(transform.eulerAngles), goParent.transform);
        return goBullet;
    }

    private void Dead()
    {

    }
}
