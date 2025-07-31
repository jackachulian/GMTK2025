using System.Collections;
using UnityEngine;

public class KillOnCollide : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision);
        if (collision.gameObject.TryGetComponent<Player2D>(out var p)) 
            p.Respawn();
    }

}
