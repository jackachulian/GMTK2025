using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayPressed()
    {
        GameManager.Instance.StartGame();
    }
}