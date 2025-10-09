using UnityEngine;

public class Health : MonoBehaviour
{
    public bool Eternal = false;

    public float Max = 100f;

    public float Current { get; set; }

    public bool IsDead => Current <= 0f;

    private void Awake()
    {
        Current = Max;
    }
}