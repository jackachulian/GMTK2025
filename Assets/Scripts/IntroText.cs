using TMPro;
using UnityEngine;

public class IntroText : MonoBehaviour
{
    [SerializeField] float _delay;
    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = (GameManager.Instance.CurrentLevelIndex + 1).ToString("D2");
        gameObject.SetActive(true);
        Invoke(nameof(Hide), _delay);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
