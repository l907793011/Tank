using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class PlayerLeft : Player
{
    public GameObject effectBurn;
    // Start is called before the first frame update
    void Start()
    {
        InitBurnEffect(effectBurn);
    }

    // Update is called once per frame
    void Update()
    {
        PlayBurn();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Attack()
    {
        //GameObject goParent =  GameObject.Find("objBullet");
        //GameObject pbBullet = (GameObject)Resources.Load("Prefabs/bullet");
        //Vector3 dir = Direction * 0.7f;
        //Vector3 pos = transform.position + dir;
        //GameObject goBullet = Instantiate(pbBullet, pos, Quaternion.Euler(transform.eulerAngles), goParent.transform);
        GameObject goBullet = CreateBullet();
        goBullet.SendMessage("SetBulletType", 1);
    }

    private void Dead()
    {
        Debug.Log("Player Left Dead");
    }
}
