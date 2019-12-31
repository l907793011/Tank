﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //public GameManager Instance;

    //private int nBarrier = 1;
    public GameObject[] lsGo;
    public Transform trImgEndGame;
    //public AudioSource audioSource;
    //public AudioClip audioClipStart;
    //public AudioClip audioClipEnd;

    private DataSecene dataScene;
    private DataDifficult dataDifficult;
    private ArrayList alsBarrier;

    //（-10,8）
    public int nRowStart = -10; //行 起始点
    public int nColStart = 8; //列 起始点

    private int nBarrier = 1; //关卡数
    private int nAllBarrierNum = 0;//关卡总数

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
        //获取关卡
        nBarrier = UnityEngine.PlayerPrefs.GetInt("Barrier");
        if (nBarrier == 0)
        {
            nBarrier = 1;
        }
    }

    void Start()
    {
        objParent = GameObject.Find("objGame");

        //读取map表
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
        //读取Difficult表
        string strDifficultPath = Application.dataPath + "/Resources/Static/map.json";
        if (File.Exists(strDifficultPath))
        {
            StreamReader sr = new StreamReader(strDifficultPath);
            string strJson = sr.ReadToEnd();//获取json文件里面的字符串
            dataDifficult = JsonMapper.ToObject<DataDifficult>(strJson);
        }

        nAllBarrierNum = dataScene.map.Count;
        InitBarrier(nBarrier);
        //var prefab = (GameObject)Resources.Load("Prefabs/player1", typeof(GameObject));
        //GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
        //go.transform.SetParent(null);
        PlayerManager.Instance.InitBornEffect(1);
    }

    public void InitBarrier(int n)
    {
        ObjBarrier obj = dataScene.GetBarrier(n);
        alsBarrier =  obj.GetBarrierList();
        CreateGameObject(alsBarrier);
        //创建敌人
        EnemyManager.Instance.SetBarrier(obj);
        EnemyManager.Instance.InitCreateEnemy();
    }

    public ObjDifficult GetObjDifficultByType(int nType)
    {
        return dataDifficult.GetDifficultByType(nType);
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

    //创建新游戏
    public void CreateNewGame()
    {
        nBarrier++;
        if (nBarrier < nAllBarrierNum)
        {
            InitBarrier(nBarrier);
        }
        else
        {
            Debug.Log("通关全部关卡");
        }

    }

    public void EndGame()
    {
        StopGame(true);
        PlayrEndGameAction();
    }

    //播放停止游戏动画
    public void PlayrEndGameAction()
    {
        trImgEndGame.gameObject.SetActive(true);
        trImgEndGame.DOLocalMoveY(-15f, 5.0f).From(true);
        Invoke("LoadScene", 5f);
    }

    //暂停游戏
    public void StopGame(bool bIsStop)
    {
        GameObject[] arEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] arPlayer = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] arBullet = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject go in arEnemy)
        {
            go.SendMessage("StopGame", bIsStop);
        }
        foreach (GameObject go in arPlayer)
        {
            go.SendMessage("StopGame", bIsStop);
        }
        foreach (GameObject go in arBullet)
        {
            go.SendMessage("StopGame", bIsStop);
        }
    }
    private void LoadScene()
    {
        SceneManager.LoadScene(0);
    }
}