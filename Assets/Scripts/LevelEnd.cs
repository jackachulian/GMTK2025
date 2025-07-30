using UnityEngine;
using UnityEngine.InputSystem;

public class LevelEnd : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject.Find("Player").GetComponent<PlayerInput>().enabled = false;
        GameManager.Instance.EndLevel();
    }
}