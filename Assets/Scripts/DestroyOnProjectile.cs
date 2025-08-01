using System.Collections;
using UnityEngine;

public class DestroyOnProjectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem _destroyParticles;
    [SerializeField] private GameObject _objectView;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerProjectile>() == null) return;
        _destroyParticles.gameObject.transform.parent = null; 
        Destroy(gameObject);
        if (_destroyParticles != null) _destroyParticles.Play();
    }

}
