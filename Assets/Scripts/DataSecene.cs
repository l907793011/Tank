﻿using System.Collections;
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
    public string difficult;
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

    public ArrayList GetDifficultList()
    {
        ArrayList list = new ArrayList();
        string[] arry = difficult.Split('|');
        foreach (string str in arry)
        {
            ArrayList alsMap = new ArrayList();
            string[] ar = str.Split(';');
            foreach (string strAr in ar)
            {
                int x = Convert.ToInt32(strAr);
                alsMap.Add(x);
            }
            list.Add(alsMap);
        }
        return list;
    }

    public int GetDifficultAllNum()
    {
        int nAllNum = 0;
        ArrayList arl = GetDifficultList();
        foreach (ArrayList ar in arl)
        {
            nAllNum += (int)ar[1];
        }
        return nAllNum;
    }
}

public class DataDifficult{
    public List<ObjDifficult> difficult = new List<ObjDifficult>();

    public ObjDifficult GetDifficultByType(int nType)
    {
        foreach (ObjDifficult objDif in difficult)
        {
            if (objDif.Id() == nType)
            {
                return objDif;
            }
        }
        return null;
    }

}

public class ObjDifficult
{
    public string id;
    public string speed;
    public string life;
    public string bulletStrength;

    public int Id()
    {
        return  Convert.ToInt32(id);
    }

    public int Speed()
    {
        return Convert.ToInt32(speed);
    }

    public int Life()
    {
        return Convert.ToInt32(life);
    }

    public int BulletStrength()
    {
        return Convert.ToInt32(bulletStrength);
    }

}