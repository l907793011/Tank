using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{

    public Button btnPlay1;
    public Button btnPlay2;
    public Button btnEnter;
    public RectTransform rtfPlayer;
    // Start is called before the first frame update
    void Start()
    {
        btnPlay1.onClick.AddListener(OnPlay1);
        btnPlay2.onClick.AddListener(OnPlay2);
        btnEnter.onClick.AddListener(OnEnterGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPlay1()
    {
        rtfPlayer.localPosition = new Vector3(-72.3f, -53.5f, 0);
    }
    private void OnPlay2()
    {
        rtfPlayer.localPosition = new Vector3(-72.3f, -84f, 0);
    }
    private void OnEnterGame()
    {
        SceneManager.LoadScene(1);
    }
}
