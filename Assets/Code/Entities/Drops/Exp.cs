
using UnityEngine;

public class Exp: Drop
{
    public int Value;

    protected override void RegisterToTickSystem()
    {
        GameManager.Instance.TickRegistry.GetOrCreateCategory<Exp>().Add(this);
    }
}