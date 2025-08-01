using System;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private ParticleSystem _destroyParticles;
    [SerializeField] private bool isShootable;
    [SerializeField] private bool isPlayerCollidable;
    [SerializeField] private bool destroysProjectileOnCollide;
    [SerializeField] GameObject[] objectsDestroyOnCollect;

    public bool collected;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.gameObject.GetComponent<PlayerProjectile>() != null && isShootable) ||
            (other.gameObject.GetComponent<Player2D>() != null || isPlayerCollidable))
        {
            foreach (GameObject obj in objectsDestroyOnCollect) Destroy(obj); // destroy locked doors on collect
            if (other.gameObject.GetComponent<PlayerProjectile>() != null) Destroy(other.gameObject);
            if (_destroyParticles) _destroyParticles.gameObject.transform.parent = null;
            Destroy(gameObject);
            if (_destroyParticles != null) _destroyParticles.Play();
            collected = true;
            FindFirstObjectByType<LevelEnd>().CheckIfUnlocked();
        }
        
    }
}
