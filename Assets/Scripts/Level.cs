using System;
using UnityEngine;

public class Level : MonoBehaviour
{
    private Player2D _player;

    private void Start()
    {
        _player = GameObject.FindFirstObjectByType<Player2D>();
    }
}