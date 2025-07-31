using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    private Transform _playerTransform;
    private Bounds _levelBounds;

    private Vector2 _minPos;
    private Vector2 _maxPos;

    private Vector3 vel = Vector3.one;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get component references
        _playerTransform = GameObject.Find("Player").transform;

        var level = FindFirstObjectByType<Level>();
        _levelBounds = new Bounds(level.levelSize / 2, level.levelSize);

        // calculate min and max positions
        var vertExtent = 8.4375f;	
    	var horzExtent = vertExtent * Screen.width / Screen.height;

        _minPos.x = _levelBounds.min.x + horzExtent;
        _minPos.y = _levelBounds.min.y + vertExtent;

        _maxPos.x = Mathf.Max(_levelBounds.max.x - horzExtent, _minPos.x);
        _maxPos.y = Mathf.Max(_levelBounds.max.y - vertExtent, _minPos.y);
    }

    // Update is called once per frame
    void Update()
    {
        // only move if not warping 
        if (GameManager.IsWarping) return;
        Vector3 target = transform.position;
        // follow player
        target = _playerTransform.position;

        target.x = Mathf.Clamp(target.x, _minPos.x, _maxPos.x);
        target.y = Mathf.Clamp(target.y, _minPos.y, _maxPos.y);
        
        target.z = -10;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref vel, 0.25f);
    }
}
