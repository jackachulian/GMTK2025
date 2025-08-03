using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroText : MonoBehaviour
{
    [SerializeField] float _delay;
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = (GameManager.Instance.CurrentLevelIndex + 1).ToString("D2");
        gameObject.SetActive(true);
        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "End") 
            gameObject.SetActive(false);
        else
            Invoke(nameof(Hide), _delay);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
