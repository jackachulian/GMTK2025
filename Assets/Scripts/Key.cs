using System;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private ParticleSystem _destroyParticles;

    public bool collected;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerProjectile>() == null && other.gameObject.GetComponent<Player2D>() == null) return;
        if (_destroyParticles) _destroyParticles.gameObject.transform.parent = null;
        Destroy(gameObject);
        if (_destroyParticles != null) _destroyParticles.Play();
        collected = true;
        FindFirstObjectByType<LevelEnd>().CheckIfUnlocked();
    }
}
