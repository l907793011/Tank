using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System;

public class SceneManager : MonoBehaviour
{
    //public SceneManager Instance;

    //private int nBarrier = 1;
    public GameObject[] lsGo;
    
    private DataSecene dataScene;
    private ArrayList alsBarrier;

    //（-10,8）
    public int nRowStart = -10; //行 起始点
    public int nColStart = 8; //列 起始点
    GameObject objParent = null;


    void Start()
    {
        objParent = GameObject.Find("objGame");

        string strPath = Application.dataPath + "/Resources/Static/map.json";
        if (File.Exists(strPath))
        {
            //1、
            //StreamReader sr = new StreamReader(strPath);
            //string strJson = sr.ReadToEnd();//获取json文件里面的字符串
            //sr.Close();

            //2、
            //string strJson = File.ReadAllText(strPath);
            //Hashtable jd = JsonMapper.ToObject<Hashtable>(strJson);
            //JsonData jd1 = jd["map"] as JsonData;
            //for (int i = 0; i < jd1.Count; i++)
            //{
            //    Debug.Log(jd1[i]["id"]);
            //    Debug.Log(jd1[i]["map"]);
            //}

            //3、
            StreamReader sr = new StreamReader(strPath);
            string strJson = sr.ReadToEnd();//获取json文件里面的字符串
            dataScene = JsonMapper.ToObject<DataSecene>(strJson);
            //JsonData id = JsonMapper.ToObject(strJson);
            //Debug.Log("id: " + id);
            InitBarrier(1);
        }
        CreateBoss();
    }

    public void InitBarrier(int n)
    {
        ObjBarrier obj = dataScene.GetBarrier(n);
        alsBarrier =  obj.GetBarrierList();
        CreateGameObject(alsBarrier);
    }

    public void CreateGameObject(ArrayList alsBerrier)
    {
        for (int i = 0; i < alsBerrier.Count; i++)
        {
            ArrayList alsRow =(ArrayList)alsBerrier[i];
            for (int j = 0; j < alsRow.Count;j++ )
            {
                int nType = (int)alsRow[j];
                GameObject obj = null;
                int nLife = 1;
                switch (nType)
                {
                    case 1:
                        obj = lsGo[0];
                        break;
                    case 2:
                        obj = lsGo[1];
                        break;
                    case 3:
                        obj = lsGo[2];
                        nLife = 2;
                        break;
                    case 4:
                        obj = lsGo[3];
                        break;
                    default:
                        break;
                }
                if (obj != null)
                {
                    GameObject go =  Instantiate(obj, objParent.transform);
                    Vector3 vecPos = new Vector3(j - 10, 8 - i, 0);
                    go.transform.position = vecPos;
                    go.SendMessage("SetLife", nLife);
                }
            }
        }
    }

    public void CreateBoss()
    {
        Vector3 vecBoss = new Vector3(0, -8, 0);
        GameObject goBoss =  Instantiate(lsGo[4], vecBoss,Quaternion.Euler(Vector3.zero), objParent.transform);

        for (int i = -8; i <= -5; i++)
        {
            for (int j = -3; j <= 3; j++)
            {
                if (i<=-7 && j>= -1 && j<=1)
                {
                    continue;
                }
                Vector3 vecPos = new Vector3(j, i, 0);
                Instantiate(lsGo[0], vecBoss, Quaternion.Euler(Vector3.zero), objParent.transform);
            }
        }
    }

}
