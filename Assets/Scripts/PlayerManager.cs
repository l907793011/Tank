using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//玩家管理器
public class PlayerManager : MonoBehaviour
{

    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //创建指定位置的角色1、左侧 2、右侧
    public void CreatePlayer(int nType)
    {
        GameObject prefab = null;
        Vector3 vecPos = Vector3.zero;
        switch(nType)
        {
            case 1:
                prefab = (GameObject)Resources.Load("Prefabs/player1", typeof(GameObject));
                vecPos = new Vector3(-2, -8, 0);
                break;
            case 2:
                prefab = (GameObject)Resources.Load("Prefabs/player2",typeof(GameObject));
                vecPos = new Vector3(2, -8, 0);
                break;
            default:
                break;
        }
        //Scene scene = SceneManager.GetActiveScene();
        GameObject go =  Instantiate(prefab, vecPos, Quaternion.Euler(Vector3.zero));
        go.transform.SetParent(null);
    }

}
