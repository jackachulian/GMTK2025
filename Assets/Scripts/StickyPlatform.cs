using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player") collision.gameObject.transform.parent = transform;
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") transform.DetachChildren();
    }


}
