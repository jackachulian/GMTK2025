using System;
using UnityEngine;

public class LevelWrap : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (!GameManager.currentLevel) return;
        
        var levelSize = GameManager.currentLevel.levelSize;

        transform.position = WrapPosition(transform.position);
    }

    public static Vector2 WrapPosition(Vector2 pos)
    {
        if (!GameManager.currentLevel) return pos;

        var levelSize = GameManager.currentLevel.levelSize;

        var x = pos.x;
        var y = pos.y;
        if (x < 0)
            x += levelSize.x;
        else if (x > levelSize.x)
            x -= levelSize.x;
        if (y < 0)
            y += levelSize.y;
        else if (y > levelSize.y)
            y -= levelSize.y;

        return new Vector2(x, y);
    }
}