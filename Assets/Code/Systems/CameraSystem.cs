
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSystem : GameSystem
{
    public Camera Camera
    {
        get
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }
            return _camera;
        }
    }
    private Camera _camera;

    protected override void Awake()
    {
        _camera = GetComponent<Camera>();

        GameManager.Instance.TickRegistry.Register<CameraSystem>(this);
        base.Awake();
    }

    
}