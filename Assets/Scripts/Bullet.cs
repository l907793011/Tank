using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int nBulletType; //子弹类型 1、角色 2、敌人
    private int nBulletStrength = 1; //子弹强度 1、打砖块 2、打铁块
    private int nSpeed = 8;
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
        transform.Translate(Vector3.up * nSpeed * Time.deltaTime, Space.Self);
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
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Player":
                if (nBulletType == 2)
                {
                    collision.SendMessage("Dead");
                    Destroy(transform.gameObject);
                }
                break;
            case "Enemy":
                if (nBulletType == 1)
                {
                    collision.SendMessage("Dead");
                    Destroy(transform.gameObject);
                }
                break;
            case "Brick":
                if (nBulletType == 1)
                {
                    collision.SendMessage("Dead");
                }
                Destroy(transform.gameObject);
                break;
            case "Iron":
                if (nBulletType == 1 && nBulletStrength == 2)
                {
                    collision.SendMessage("Dead");
                }
                Destroy(transform.gameObject);
                break;
            case "Boss":
                collision.SendMessage("Dead");
                Destroy(transform.gameObject);
                break;
            case "Grass":
                break;
            case "River":
                break;
            default:
                break;
        }
    }
}
