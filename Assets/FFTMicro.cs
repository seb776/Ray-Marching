using UnityEngine;
using System.Collections;


public class FFTMicro : MonoBehaviour
{
    public GameObject Parent;
    float[] _samples = new float[128];
    AudioSource _source;
    private void Start()
    {
        Debug.Log("Micro is " + Microphone.devices[0]);
        _source = this.gameObject.GetComponent<AudioSource>();
        _source.clip = Microphone.Start(Microphone.devices[0], true, 20, 24000);
        int i = 0;
        foreach (var sample in _samples)
        {
            var prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prim.transform.SetParent(Parent.transform);
            prim.transform.position = new Vector3(i, 0, 0);
            ++i;
        }

    }

    private void Update()
    {
        _source.time += Time.deltaTime;
        _source.Play();
        _source.mute = true;

        _source.GetSpectrumData(_samples, 0, FFTWindow.Hamming);
        int i = 0;
        foreach (var sample in _samples)
        {
            var prim = Parent.transform.GetChild(i);
            var pos = prim.transform.position;
            prim.transform.position = new Vector3(pos.x, _samples[i]*10000.0f*1000.0f, pos.z);
            ++i;
        }
    }
}