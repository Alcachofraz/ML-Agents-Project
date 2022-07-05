using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAnalysis : MonoBehaviour
{
    static float[] samples = new float[1024];
    static float samplingFrequency = AudioSettings.outputSampleRate;

    public static float[] GetSamples(AudioSource audioSource)
    {
        audioSource.GetOutputData(samples, 0);
        return samples;
    }

    public static float[] GetSpectrum(AudioSource audioSource)
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Hamming);
        return samples;
    }

    public static float ConcentrationAroundPeak(float peak) {
        int index = (int)(peak * samples.Length / (0.5f * samplingFrequency));
        int centredBand = samples.Length / 200;
        float total = 0;
        float insideBand = 0;
        for (int i = 0; i < samples.Length - 5; i++) {
            total += samples[i] * samples[i];
            if (Mathf.Abs(i - index) < centredBand)
                insideBand += samples[i] * samples[i];
        }
        return insideBand / total;
    }

    public static float ComputeSpectrumPeak(AudioSource audioSource, bool interpolate)
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Hamming);
        float max = 0;
        int index = 0;
        for (int i = 0; i < 3 * samples.Length / 4; i++) {
            if (samples[i] > max) {
                max = samples[i];
                index = i;
            }
        }
        if (interpolate && index > 0 && index < samples.Length - 1) {
            float dL = samples[index] - samples[index - 1];
            float dR = samples[index] - samples[index + 1];
            index += (int) ((dL - dR) / (2 * (dL + dR)));
        }
        float peak = index * 0.5f * samplingFrequency / samples.Length;
        return peak;
    }

    public static float MeanEnergy(AudioSource audioSource)
    {
        audioSource.GetOutputData(samples, 0);
        float sum = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        return sum / samples.Length;
    }

    public static float ComputeRMS (AudioSource audioSource)
    {
        float meanEnergy = MeanEnergy(audioSource);
        return Mathf.Sqrt(meanEnergy);
    }

    public static float ConvertToDB(float val)
    {
        float reference = 1e-7f;
        if (val < reference) val = reference;
        return 10f * Mathf.Log10(val / reference);
    }

    public static float[] FrequencyBands(AudioSource audioSource)
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Hamming);
            int nBands = 7; // (int) Mathf.Log(samples.Length, 2) - 3;
        float[] bands = new float[nBands];
        int nSamples = 8 * samples.Length / 1024;
        int initialSample = 0;
        for (int i = 0; i < nBands; i++)
        {
            float val = 0;
            for (int j = initialSample; j < initialSample + nSamples; j++)
            {
                val += samples[j] * samples[j];
            }
            bands[i] = ConvertToDB(val/nSamples);
            initialSample += nSamples;
            nSamples *= 2;
        }
        return bands;
    }
}
