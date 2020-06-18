using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    public Button playPutton;
    void Start()
    {
        Screen.SetResolution(576, 1024, true);

        Button btn = playPutton.GetComponent<Button>();
        btn.onClick.AddListener(StarGame);
    }

    public void StarGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
