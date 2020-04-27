using UnityEngine;
using UnityEngine.Video;

public class VHSEffect : MonoBehaviour
{
    [SerializeField] private Shader shader;
    [SerializeField] private VideoClip VHSClip;

    private float scanLineY;
    private float scanLineX;
    private Material material = null;
    private VideoPlayer videoPlayer;

    void Start()
    {
        material = new Material(shader);
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.isLooping = true;
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.clip = VHSClip;
        videoPlayer.Play();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetTexture("_VHSTex", videoPlayer.texture);

        scanLineY += Time.deltaTime * 0.01f;
        scanLineX -= Time.deltaTime * 0.1f;

        if (scanLineY >= 1)
        {
            scanLineY = Random.value;
        }
        if (scanLineX <= 0 || Random.value < 0.05)
        {
            scanLineX = Random.value;
        }
        material.SetFloat("_yScanline", scanLineY);
        material.SetFloat("_xScanline", scanLineX);
        Graphics.Blit(source, destination, material);
    }

    protected void OnDisable()
    {
        if (material)
        {
            DestroyImmediate(material);
        }
    }
}