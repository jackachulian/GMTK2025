using System;
using UnityEngine;

public class LevelWrap : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (!GameManager.currentLevel) return;
        
        if (GameManager.Instance.IsScrollingLevel)
            transform.position = WrapPositionAdaptive(transform.position);
        else
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

    public static Vector2 WrapPositionAdaptive(Vector2 pos)
    {
        if (!GameManager.currentLevel || !GameManager.IsWarping) return pos;

        var levelSize = GameManager.currentLevel.levelSize;

        var vertExtent = 8.4375f;	
    	var horzExtent = vertExtent * Screen.width / Screen.height;

        var minX = Camera.main.transform.position.x - horzExtent;
        var maxX = Camera.main.transform.position.x + horzExtent;

        var minY = Camera.main.transform.position.y - vertExtent;
        var maxY = Camera.main.transform.position.y + vertExtent;

        var x = pos.x;
        var y = pos.y;
        if (x < minX)
            x += horzExtent * 2;
        else if (x > maxX)
            x -= horzExtent * 2;
        if (y < minY)
            y += vertExtent * 2;
        else if (y > maxY)
            y -= vertExtent * 2;

        return new Vector2(x, y);
    }
}