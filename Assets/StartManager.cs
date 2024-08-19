using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public Button StartButton;

    private void Start()
    {
        StartButton.onClick.AddListener(OnStartButton);
    }

    void OnStartButton()
    {
        SceneManager.LoadScene("Level-1");
    }
}
