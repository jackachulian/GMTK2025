using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayPressed()
    {
        SceneManager.LoadScene(GameManager.Instance.levels[0].buildIndex);
    }
}