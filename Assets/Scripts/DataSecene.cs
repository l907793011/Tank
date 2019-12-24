using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//[System.Serializable]
public class DataSecene
{
    
    public List<ObjBarrier> map = new List<ObjBarrier>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public ObjBarrier GetBarrier(int n)
    {
        foreach (ObjBarrier obj in map)
        {
            if (obj.Index() == n)
            {
                return obj;
            }
        }
        return null;
    }
}

//[System.Serializable]
public class ObjBarrier
{
    public string id;
    public string map;

    public int Index()
    {
        return Convert.ToInt32(id);
    }
    //public List<int> map = new List<int>();
    //获取关卡场景列表
    public ArrayList GetBarrierList()
    {
        //Array[] arryScene = new Array[17];
        //List<List<int>> lsScene = new List<List<int>>();

        ArrayList list = new ArrayList();
        string[] arry = map.Split('|');
        //int nIndexX = 0;
        foreach (string str in  arry)
        {
            // List<int> ls = new List<int>();

            //int[] arMap = new int[21];
            ArrayList alsMap = new ArrayList();
            string[] ar = str.Split(';');
            //int nIndexY = 0;
            foreach (string strAr in ar)
            {
                int x = Convert.ToInt32(strAr);
                alsMap.Add(x);
                //Debug.Log(nIndexX + " : " + nIndexY + "   " + x);
                //arMap[nIndexY] = x;
                //nIndexY++;
                //ls.Insert(x,x);
            }
            //arryScene[nIndexX] = arMap;
            list.Add(alsMap);
           // nIndexX++;

            //lsScene.Insert(nIndex,ls);
        }

        return list;
    }
}
