using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public SpriteRenderer sRBoss;
    public Sprite spriteBoss;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Dead()
    {
        sRBoss.sprite = spriteBoss;
    }
}
