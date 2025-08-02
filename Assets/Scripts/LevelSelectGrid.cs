using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectGrid : MonoBehaviour
{
    [SerializeField] private GameObject _levelButtonPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < GameManager.Instance.levels.Length; i++)
        {
            int _i = i;
            Button b = Instantiate(_levelButtonPrefab, transform).GetComponent<Button>();
            b.onClick.AddListener(() => {GameManager.Instance.SelectLevel(_i);});
            b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (_i + 1).ToString("D2");
        }
    }
}
