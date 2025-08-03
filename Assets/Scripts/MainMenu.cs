using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private bool _resetTime = true;
    void Start()
    {
        AudioManager.Instance.PlayMusic("None");
        GameManager.Instance.IsTiming = false;
        if (_resetTime) GameManager.Instance.PlayTime = -1f;
    }

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