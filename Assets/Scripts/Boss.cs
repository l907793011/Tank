using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public SpriteRenderer sRBoss;
    public Sprite spriteBoss;

    private int nLife = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetLife(int n)
    {
        nLife = n;
    }

    private void Dead()
    {
        sRBoss.sprite = spriteBoss;
        GameManager.Instance.EndGame();
        AudioSource audio = transform.GetComponent<AudioSource>();
        if (audio)
        {
            audio.Play();
        }
    }
}
