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

    public void PlaySfx(string s)
    {
        AudioManager.Instance.PlaySfx(s);
    }

    public void Quit()
    {
        Application.Quit();
    }
}