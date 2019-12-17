using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int nSpeed = 1;
    private bool bHInRun = false;  //水平位置是否处于移动状态
    private bool bVInRun = false;  //垂直位置是否处于移动状态
    private bool bRuning = false;  //是否处于移动状态
    private Vector3 vDirection = Vector3.up; //移动的方向

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 && v == 0)
        {
            Debug.Log("Move: " + h);
            Vector3 vDir = Vector3.up;
            if(h< 0)
            {
                vDir = Vector3.down;
            }
            if (vDirection != vDir)
            {
                vDirection = vDir;
                ChangeDirection();

            }
        }
        else if(v != 0){
            Debug.Log("Move: " + v);
        }
    }

    //向上
    private void MoveUp()
    {
        transform.Translate(Vector3.up * Time.deltaTime * nSpeed, Space.World);
    }

    //向下
    private void MoveDown()
    {
        transform.Translate(Vector3.down * Time.deltaTime * nSpeed, Space.World);
    }

    public void ChangeDirection()
    {

    }
}
