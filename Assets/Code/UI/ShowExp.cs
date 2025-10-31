
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowExp : MonoBehaviour
{
    public Image ExpBar;

    public TMP_Text ExperienceStep;

    public TMP_Text Level;

    private LevelCounter _levelCounter;

    private void Awake()
    {
        _levelCounter = GameManager.Instance.CounterRegistry.GetCounterOrNUll<LevelCounter>();
        _levelCounter.Changed.AddListener(UpdateCounter);
        UpdateCounter();
    }

    private void UpdateCounter()
    {
        float progress = 0;
        if (_levelCounter.ExperienceEnd == _levelCounter.ExperienceStart)
            progress = 0f;
        else
            progress =
            (float)(_levelCounter.Experience - _levelCounter.ExperienceStart) / (_levelCounter.ExperienceEnd - _levelCounter.ExperienceStart);

        ExpBar.fillAmount = progress;

        ExperienceStep.text = $"{_levelCounter.Experience}/{_levelCounter.ExperienceEnd}";
        Level.text = _levelCounter.Level.ToString();
    }
}