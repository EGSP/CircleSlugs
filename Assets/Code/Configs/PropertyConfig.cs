
using UnityEngine;

[CreateAssetMenu(fileName = "PropertyConfig", menuName = "Configs/PropertyConfig")]
public class PropertyConfig : ScriptableObject
{
    public string Property;

    public string ValueS;

    public float ValueF;

    public int ValueI;
}