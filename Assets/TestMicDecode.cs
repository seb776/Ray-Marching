using JulianSchoenbaechler.MicDecode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMicDecode : MonoBehaviour
{
    public MicDecode Decode;

    public float Power;
    [Range(0.0f, 1.0f)] public float Smooth;
    public GameObject Parent;
    public float[] _smoothedSamples = new float[128];
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 128; ++i)
        {
            var prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prim.transform.SetParent(Parent.transform);
            prim.transform.position = new Vector3(i, 0, 0);
        }
        Decode.StartRecording();
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        foreach (var sample in Decode._spectrum)
        {
            if (i >= Parent.transform.childCount)
                break;
            float dampen = 1.0f;// Midi._faders[(int)(((float)i / 128.0f) * 2.0f)]; // Fader 0 1 or 2 are mapped (the first three columns)
            _smoothedSamples[i] = Mathf.Lerp(_smoothedSamples[i], Decode._spectrum[i] * Power, Smooth);
            var child = Parent.transform.GetChild(i);
            child.transform.position = new Vector3(child.transform.position.x, _smoothedSamples[i], child.transform.position.z);
            ++i;
        }
    }
}
