
using UnityEngine;

public class Exp: Drop
{
    public int Value;

    public bool Triggered { get; set; }

    public float Acceleration { get; set; }

    protected override void RegisterToTickSystem()
    {
        GameManager.Instance.TickRegistry.GetOrCreateCategory<Exp>().Add(this);
    }
}