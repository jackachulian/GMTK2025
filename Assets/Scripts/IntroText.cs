using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroText : MonoBehaviour
{
    [SerializeField] float _delay;
    private float _timer;

    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = (GameManager.Instance.CurrentLevelIndex + 1).ToString("D2");
        gameObject.SetActive(true);
        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "End") 
            gameObject.SetActive(false);
        else
            _timer = _delay;
    }

    private void Update()
    {
        _timer -= Time.unscaledDeltaTime;

        if (_timer < 0)
        {
            gameObject.SetActive(false);
        }
    }
}
