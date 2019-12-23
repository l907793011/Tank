using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

//[System.Serializable]
public class ObjBarrier
{
    public int id;
    public string map;
    //public List<int> map = new List<int>();
}
