using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public Vector2 forceToApply = Vector2.up * 5f;
    public Collider2D collider;
    public Animator animator;
        
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Player2D>(out var player))
        {
            Debug.Log("Bounce Pad");
            player.ApplyForce(forceToApply);
            collider.enabled = false;
            AudioManager.Instance.PlaySfx("Bounce");
            if (animator) animator.Play("Bounce");
            StartCoroutine(ReenableCollider());
        }
    }

    private IEnumerator ReenableCollider()
    {
        yield return new WaitForSeconds(0.25f);
        collider.enabled = true;
    }
}