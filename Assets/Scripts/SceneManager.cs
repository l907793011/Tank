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

    private DataSecene dataSecene;
       
    void Start()
    {
        string strPath = Application.dataPath + "/Resources/Static/map.json";
        if (File.Exists(strPath))
        {
            //StreamReader sr = new StreamReader(strPath);
            //string jsonStr = sr.ReadToEnd();//获取json文件里面的字符串
            //sr.Close();
            string strJson = File.ReadAllText(strPath);
            Hashtable jd = JsonMapper.ToObject<Hashtable>(strJson);
            JsonData jd1 = jd["map"] as JsonData;
            for (int i = 0; i < jd1.Count; i++)
            {
                Debug.Log(jd1[i]["id"]);
                Debug.Log(jd1[i]["map"]);
            }

            //JsonData id = JsonMapper.ToObject(strJson);
            //Debug.Log("id: " + id);
        }
    }

}
