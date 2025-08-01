using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayPressed()
    {
        GameManager.Instance.StartGame();
    }

    public void SetScene(string s)
    {
        SceneManager.LoadScene(s);
    }
}