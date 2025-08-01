using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision!");
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "PlayerProjectile")
        {
            Debug.Log("Destroy destructable!");
            Destroy(gameObject);
        }
    }
}
