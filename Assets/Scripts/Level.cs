using System;
using UnityEngine;

public class Level : MonoBehaviour
{
    private Player2D _player;

    public Vector2 levelSize = new Vector2(20f, 11.25f);

    private void Start()
    {
        _player = GameObject.FindFirstObjectByType<Player2D>();
        
        GameManager.currentLevel = this;
    }
}