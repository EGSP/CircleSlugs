using UnityEngine;

public struct AttackSpeedRecord : IRecord
{
    public int SequenceId { get; set; }

    /// <summary>
    /// The percentage change. It will be used for sum (value should be like 10% is 0.1f)
    /// </summary>
    public float Change { get; set; }
}

public class AttackSpeedCounter : Counter
{
    public const float AttackSpeedLimit = 0.8f; // Максимальный предел, к которому стремится модификатор
    public const float DecayRate = 1.5f; // Коэффициент "крутизны" замедления (1-5)

    public float AttackSpeedModifier => 1 - AttackSpeed;
    private float AttackSpeed { get; set; }

    private RecordCollection<AttackSpeedRecord> _records;

    public AttackSpeedCounter(RecordCollection<AttackSpeedRecord> records)
    {
        _records = records;
        _records.Records.OnChanged(Calculate);
    }

    protected override void Calculate()
    {
        float change = _records.Records[^1].Change;

        // Гиперболическая формула: добавляем change с учётом близости к пределу
        float proximity = 1f - (AttackSpeed / AttackSpeedLimit); // Насколько далеко от предела
        float adjustedProximity = Mathf.Pow(proximity, DecayRate); // Применяем степень для усиления эффекта

        AttackSpeed += change * adjustedProximity;

        // Защита от превышения лимита
        if (AttackSpeed > AttackSpeedLimit)
        {
            AttackSpeed = AttackSpeedLimit;
        }

        Changed.Invoke();
    }
}


// Без proximity (обычное накопление)
// change = 0.1f каждый уровень

// Уровень | AttackSpeed
// --------|------------
// 0       | 0.00
// 1       | 0.10
// 2       | 0.20
// 5       | 0.50
// 10      | 1.00
// 20      | 2.00
// 50      | 5.00

// decayRate = 1.0 (как proximity без степени)
// Уровень | AttackSpeed | adjustedProximity | Прирост
// --------|-------------|-------------------|--------
// 0       | 0.000       | 1.000             | 0.100
// 1       | 0.100       | 0.899             | 0.090
// 2       | 0.190       | 0.808             | 0.081
// 5       | 0.410       | 0.586             | 0.059
// 10      | 0.651       | 0.342             | 0.034
// 20      | 0.850       | 0.141             | 0.014
// 50      | 0.970       | 0.020             | 0.002


// С adjustedProximity, decayRate = 2.0
// float adjustedProximity = Mathf.Pow(proximity, 2f);

// Уровень | AttackSpeed | proximity | adjustedProximity | Прирост
// --------|-------------|-----------|-------------------|--------
// 0       | 0.000       | 1.000     | 1.000             | 0.100
// 1       | 0.100       | 0.899     | 0.808             | 0.081
// 2       | 0.181       | 0.817     | 0.667             | 0.067
// 5       | 0.382       | 0.614     | 0.377             | 0.038
// 10      | 0.585       | 0.409     | 0.167             | 0.017
// 20      | 0.771       | 0.221     | 0.049             | 0.005
// 50      | 0.931       | 0.059     | 0.003             | 0.000