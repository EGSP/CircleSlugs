
public struct IncreaseLevelRecord : IRecord
{
    public int SequenceId { get; set; }

    /// <summary>
    /// Добавление уровня
    /// </summary>
    public int Increase { get; set; }

    public IncreaseLevelRecord(int increase)
    {
        SequenceId = int.MinValue;
        Increase = increase;
    }
}