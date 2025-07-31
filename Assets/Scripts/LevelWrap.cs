using System;
using UnityEngine;

public class LevelWrap : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (!GameManager.currentLevel) return;
        
        var levelSize = GameManager.currentLevel.levelSize;

        var x = transform.position.x;
        var y = transform.position.y;
        if (x < 0)
            x += levelSize.x;
        else if (x > levelSize.x)
            x -= levelSize.x;
        if (y < 0)
            y += levelSize.y;
        else if (y > levelSize.y)
            y -= levelSize.y;

        transform.position = new Vector2(x, y);
    }
}