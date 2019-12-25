using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//敌人管理器
public class EnemyManager : MonoBehaviour
{
    private Vector3[] lsPos = {new Vector3(-10,8,0), new Vector3(0, 8, 0), new Vector3(10, 8, 0) };
    private int nLastPos = 0;
    private int nEnemyAllNum = 20; //敌人总数量
    public GameObject[] objEnemy; //预制件列表 0、普通 1、普通红色 2、快速 3、快速红色 4、高级白色  5、高级红色 6、高级黄色 7、高级绿色
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //获取随机位置
    private Vector3 GetPos()
    {
        Vector3 vecPos = Vector3.zero;
        int nRang = Random.Range(1, 4);
        while (nLastPos == nRang)
        {
            nRang = Random.Range(1, 4);
        }
        vecPos = lsPos[nRang];
        nLastPos = nRang;
        return vecPos;
    }

    //创建敌人
    public void CreateEnemy()
    {


    }
}
