using UnityEngine;

public class Orientation
{
    private Vector3 _direction;
    private bool _isVisible;

    public Orientation(Vector3 direction, bool isVisible)
    {
        _direction = direction;
        _isVisible = isVisible;
    }

    public Vector3 GetDirection()
    {
        return _direction;
    }

    public void SetVisibility(bool isVisible)
    {
        _isVisible = isVisible;
    }

    public bool GetVisibility()
    {
        return _isVisible;
    }
}