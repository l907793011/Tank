using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{
    //public GameManager Instance;

    //private int nBarrier = 1;
    public GameObject[] lsGo;
    
    private DataSecene dataScene;
    private ArrayList alsBarrier;

    //（-10,8）
    public int nRowStart = -10; //行 起始点
    public int nColStart = 8; //列 起始点
    GameObject objParent = null;

    //单例
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    private void Awake()
    {
        instance = this;
    }

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
        }
        InitBarrier(1);
        //var prefab = (GameObject)Resources.Load("Prefabs/player1", typeof(GameObject));
        //GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
        //go.transform.SetParent(null);
        PlayerManager.Instance.CreatePlayer(1);
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
                if (nType > 0)
                {
                    GameObject prefabObj = lsGo[nType -1];
                    Vector3 vecPos = new Vector3(j - 10, 8 - i, 0);
                    GameObject go =  Instantiate(prefabObj, vecPos, Quaternion.Euler(Vector3.zero), objParent.transform);

                    int nLife = 1;
                    if (nType == 3)//铁块
                    {
                        nLife = 2;
                    }
                    //Debug.Log("Type: " + nType);
                    go.SendMessage("SetLife", nLife);
                }
            }
        }
    }
}