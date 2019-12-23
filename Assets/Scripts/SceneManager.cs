using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class SceneManager : MonoBehaviour
{
    //public SceneManager Instance;

    //private int nBarrier = 1;

    void Start()
    {
        string strPath = Application.dataPath + "Resources/Static/map.json";
        if (File.Exists(strPath))
        {
            StreamReader sr = new StreamReader(strPath);
            string jsonStr = sr.ReadToEnd();//获取json文件里面的字符串
            sr.Close();
            Debug.Log(jsonStr);
            //for (int i = 0; i < jsonStr.Count; i++)//遍历获取数据
            //{

            //}
            //Save save = JsonMapper.ToObject<Save>(jsonStr); //将字符串转为save对象
        }
    }

}
