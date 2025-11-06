
using Unity.Cinemachine;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public float AmplitudeMaxGain = 0.21f;
    public float FrequencyMaxGain = 1f;

    public float PowerCurveRiseTime = 2f;
    public float PowerCurveFallbackTime = 1f;
    public AnimationCurve Power;
    public float PowerLerpCap = 1f;


    public CinemachineBasicMultiChannelPerlin Noise;

    private float _powerLerp = 0;

    private void Update()
    {
        var fallbackSpeed = 1 / PowerCurveFallbackTime;
        _powerLerp = Mathf.MoveTowards(_powerLerp, 0, Time.deltaTime * fallbackSpeed);
        UpdateNoise();
    }

    public void Shake()
    {
        var riseSpeed = PowerLerpCap/PowerCurveRiseTime;
        _powerLerp = Mathf.MoveTowards(_powerLerp, PowerLerpCap, Time.deltaTime * riseSpeed);
        UpdateNoise();
    }

    private void UpdateNoise()
    {
        var power = Power.Evaluate(_powerLerp);
        Noise.AmplitudeGain = AmplitudeMaxGain * power;
        Noise.FrequencyGain = FrequencyMaxGain * power;
    }

    void Reset()
    {
        Noise = FindAnyObjectByType<CinemachineBasicMultiChannelPerlin>();
    }
}