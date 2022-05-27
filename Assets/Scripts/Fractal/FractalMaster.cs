using System;
using System.Linq;
using UnityEngine;

[ImageEffectAllowedInSceneView]
public class FractalMaster : MonoBehaviour {

    public GameObject LeftHandPos;
    public GameObject RightHandPos;
    public ComputeShader fractalShader;
    public TestMicDecode MicDecode;

    public GameObject BubblesParent;

    [Range (1, 20)]
    public float fractalPower = 10;
    public float darkness = 70;

    [Header ("Colour mixing")]
    [Range (0, 1)] public float blackAndWhite;
    [Range (0, 1)] public float redA;
    [Range (0, 1)] public float greenA;
    [Range (0, 1)] public float blueA = 1;
    [Range (0, 1)] public float redB = 1;
    [Range (0, 1)] public float greenB;
    [Range (0, 1)] public float blueB;

    RenderTexture target;
    Camera cam;
    Light directionalLight;

    [Header ("Animation Settings")]
    public float powerIncreaseSpeed = 1.2f;

    void Start() {
        Application.targetFrameRate = 60;
        _bubbles = new Vector4[BubblesParent.transform.childCount];
    }

    Vector4[] _bubbles;
    
    void Init () {
        cam = Camera.current;
        directionalLight = FindObjectOfType<Light> ();
    }

    // Animate properties
    void Update () {
            fractalPower += powerIncreaseSpeed*Time.deltaTime;
    }

    void OnRenderImage (RenderTexture source, RenderTexture destination) {
        Init ();
        InitRenderTexture ();
        SetParameters ();

        int threadGroupsX = Mathf.CeilToInt (cam.pixelWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt (cam.pixelHeight / 8.0f);
        fractalShader.Dispatch (0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit (target, destination);
    }

    void SetParameters () {
        fractalShader.SetTexture(0, "Destination", target);
        fractalShader.SetFloat ("power", (Mathf.Sin(fractalPower)*0.5f+0.5f)*3.0f+5.0f);
        fractalShader.SetFloat ("darkness", darkness);
        fractalShader.SetFloat ("blackAndWhite", blackAndWhite);
        fractalShader.SetVector ("colourAMix", new Vector3 (redA, greenA, blueA));
        fractalShader.SetVector ("colourBMix", new Vector3 (redB, greenB, blueB));

        fractalShader.SetMatrix ("_CameraToWorld", cam.cameraToWorldMatrix);
        fractalShader.SetMatrix ("_CameraInverseProjection", cam.projectionMatrix.inverse);
        fractalShader.SetVector("_LightDirection", directionalLight.transform.forward);

        fractalShader.SetVector("_LeftHandPos", LeftHandPos.transform.position);
        fractalShader.SetVector("_RightHandPos", RightHandPos.transform.position);
        fractalShader.SetVector("_Time", Shader.GetGlobalVector("_Time"));
        Debug.Log("samplesCount " + MicDecode._smoothedSamples.Length);
        fractalShader.SetVectorArray("_AudioSamples", MicDecode._smoothedSamples.Select(el => new Vector4(el, el, el, el)).ToArray());

        int i = 0;
        foreach (Transform child in BubblesParent.transform)
        {
            _bubbles[i] = child.position;
            i++;
        }    

        fractalShader.SetVectorArray("_Bubbles", _bubbles);
    }

    void InitRenderTexture () {
        if (target == null || target.width != cam.pixelWidth || target.height != cam.pixelHeight) {
            if (target != null) {
                target.Release ();
            }
            target = new RenderTexture (cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite = true;
            target.Create ();
        }
    }
}