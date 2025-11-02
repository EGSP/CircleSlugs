
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class LevelCounter : Counter
{
    public const float BaseExpGrowth = 5;
    public const float QuadraticExpGrowth = 1.2f;

    private RecordCollection<AddExperienceRecord> _addedExperience = null;
    private RecordCollection<IncreaseLevelRecord> _increaseLevel = null;

    /// <summary>
    /// Overall collected experience
    /// </summary>
    public float Experience { get; private set; }

    public float ExperienceStart => _cachedLevelStartXp;

    public float ExperienceEnd => _cachedLevelEndXp;

    public int Level { get; private set; }

    public LevelCounter()
    {
        _addedExperience = GetRecordCollection<AddExperienceRecord>();
        _increaseLevel = GetRecordCollection<IncreaseLevelRecord>();        

        _addedExperience.Records.OnChanged(Calculate);

        CalculateNextLevelAndBounds();
    }

    protected override void Calculate()
    {
        if (_addedExperience.Records.Count == 0) return;

        Experience += _addedExperience.Records[^1].Value;

        CalculateNextLevelAndBounds();

        Changed.Invoke();
    }

    private void CalculateNextLevelAndBounds()
    {
        int newLevel = GetLevelFromXP();
        if (newLevel == Level) return;

        int diff = newLevel - Level;

        if (diff > 0)
        {
            _increaseLevel.Add(new IncreaseLevelRecord(diff));
        }
        else
        {
            throw new System.Exception("Level decrease not supported");
        }

        Level = newLevel;
    }



    private int _cachedLevel = 0;
    private float _cachedLevelStartXp = 0;
    private float _cachedLevelEndXp = 0;

    private int GetLevelFromXP()
    {
        const int levelLimit = 99999;

        float levelXPStart = _cachedLevelStartXp;
        int level = _cachedLevel;

        int iteration = 0;
        while (true && iteration++ < (int.MaxValue - 1))
        {

            int nextLevel = level + 1;
            float xpForNextLevel = Mathf.Floor(nextLevel * BaseExpGrowth + nextLevel * nextLevel * QuadraticExpGrowth);

            // Если мы преодолели значение текущего собранного опыта, значит мы получили текущий уровень
            if (xpForNextLevel > Experience)
            {
                _cachedLevelEndXp = xpForNextLevel;
                break;
            }

            // Ограничитель чтобы не уйти в бесконечность при ошибках подсчета
            if (level + 1 >= levelLimit)
            {
                break;
            }

            // Поднимаем уровень
            level++;
            levelXPStart = xpForNextLevel;
        }

        // Обновляем кеш
        _cachedLevel = level;
        _cachedLevelStartXp = levelXPStart;

        return level;
    }
}