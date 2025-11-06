
using UnityEngine;
using UnityEngine.InputSystem;

public class DrawCrosshairSystem : GameSystem
{
    public GameObject Crosshair;

    private bool _showCrosshair = true;

    public InputAction PointerPosition { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        ShowCrosshair(_showCrosshair);

        PointerPosition = InputSystem.actions.FindAction("PointerPosition"); 
    }

    private void LateUpdate()
    {
        if (_showCrosshair)
            Crosshair.transform.position = PointerPosition.ReadValue<Vector2>();
    }

    public void ShowCrosshair(bool show)
    {
        if (Crosshair == null) return;

        _showCrosshair = show;
        Crosshair.SetActive(show);
    }
}