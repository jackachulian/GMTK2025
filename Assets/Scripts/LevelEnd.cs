using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private GameObject unlockedParticleEffects;
    [SerializeField] private Collider2D collider;

    private Key[] keys;
    private bool unlocked;

    private void Start()
    {
        keys = GameObject.FindObjectsOfType<Key>();
        CheckIfUnlocked();
        if (!unlocked)
        {
            spriteRenderer.sprite = lockedSprite;
            unlockedParticleEffects.SetActive(false);
            collider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!unlocked) return;
        GameObject.Find("Player").GetComponent<PlayerInput>().enabled = false;
        GameObject.Find("Player").GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        StartCoroutine(EndLevel());
    }

    private IEnumerator EndLevel()
    {
        AudioManager.Instance.PlaySfx("Win");
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.EndLevel();
    }

    public void CheckIfUnlocked()
    {
        foreach (var key in keys)
        {
            // all keys if any must be destroyed (collected) to unlock
            if (key != null && !key.collected) return;
        }

        Unlock();
    }

    public void Unlock()
    {
        unlocked = true;
        spriteRenderer.sprite = openSprite;
        unlockedParticleEffects.SetActive(true);
        collider.enabled = true;
    }
}