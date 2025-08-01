using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public Vector2 forceToApply = Vector2.up * 5f;
        
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Player2D>(out var player))
        {
            Debug.Log("Bounce Pad");
            player.ApplyForce(forceToApply);
        }
    }
}