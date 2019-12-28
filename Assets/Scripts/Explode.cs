using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    private float nTime = 0;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Dead");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerAudio()
    {
        audioSource.Play();
    }

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(0.168f); //等待1秒
        Destroy(transform.gameObject);
}
}
