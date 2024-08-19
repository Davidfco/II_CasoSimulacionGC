using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tryagain : MonoBehaviour
{
    public Button TryagainbButton;

    private void Start()
    {
        TryagainbButton.onClick.AddListener(OnTryAgainButton);
    }

    void OnTryAgainButton()
    {
        SceneManager.LoadScene("Level-1");
    }
}
