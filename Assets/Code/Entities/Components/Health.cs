using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public bool Eternal = false;

    public float Max = 100f;

    public float Current
    {
        get => _current;
        set
        {
            var delta = value - _current;
            var old = _current;
            _current = value;
            OnHealthChanged.Invoke(this, old, delta);
        }
    }
    private float _current;

    public bool IsDead => Current <= 0f;

    /// <summary>
    /// Invoked when the health changes.
    /// 1 float - previous value
    /// 2 float - delta
    /// </summary>
    public UnityEvent<Health, float, float> OnHealthChanged = new();
    

    private void Awake()
    {
        Current = Max;
    }
}