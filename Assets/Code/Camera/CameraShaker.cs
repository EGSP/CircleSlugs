
using Unity.Cinemachine;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public float AmplitudeMaxGain = 0.21f;
    public float FrequencyMaxGain = 1f;
    public float DecreaseTime = 1f;
    public AnimationCurve PowerDecrease;

    public CinemachineBasicMultiChannelPerlin Noise;

    private float _powerLerp = 0;

    private void Update()
    {
        var fallbackSpeed = 1 / DecreaseTime;
        _powerLerp = Mathf.MoveTowards(_powerLerp, 0, Time.deltaTime * fallbackSpeed);
        UpdateNoise();
    }

    public void Shake()
    {
        _powerLerp = 1;
        UpdateNoise();
    }

    private void UpdateNoise()
    {
        var power = PowerDecrease.Evaluate(_powerLerp);
        Noise.AmplitudeGain = AmplitudeMaxGain * power;
        Noise.FrequencyGain = FrequencyMaxGain * power;
    }

    void Reset()
    {
        Noise = FindAnyObjectByType<CinemachineBasicMultiChannelPerlin>();
    }
}