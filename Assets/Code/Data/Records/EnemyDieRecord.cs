
using UnityEngine;

public struct EnemyDieRecord : IRecord
{
    public int SequenceId { get; set; }

    public Vector3 Position { get; set; }
}