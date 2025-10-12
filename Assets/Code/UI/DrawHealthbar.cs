

using UnityEngine;
using UnityEngine.UI;

public class DrawHealthbar : MonoBehaviour
{
    public Image Image;

    public Entity Entity
    {
        get => _entity;
        set
        {
            _entity = value;
            _entity.Health.OnHealthChanged.AddListener(Change);
            _entity.OnTerminateMark.AddListener(() => _entity.Health.OnHealthChanged.RemoveListener(Change));
        }
    }
    private Entity _entity;

    private void Change(Health health, float old, float delta)
    {
        var percentage = health.Current / health.Max;
        Image.fillAmount = percentage;
    }
}