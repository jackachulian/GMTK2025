using System.Threading.Tasks;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public Vector2 forceToApply = Vector2.up * 5f;
    private Collider2D collider;
    public Animator animator;
        
    async void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Player2D>(out var player))
        {
            Debug.Log("Bounce Pad");
            player.ApplyForce(forceToApply);
            collider.enabled = false;
            animator.Play("Bounce");
            await Task.Delay(250);
            collider.enabled = true;
        }
    }
}