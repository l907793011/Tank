using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private int nLife = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int Life
    {
        get { return nLife; }
        set { nLife = value; }
    }

    private void Dead()
    {
        nLife--;
        if (nLife <= 0)
        {
            Destroy(transform.gameObject);
        }
    }
    private void SetLife(int n)
    {
        nLife = n;
    }
}
