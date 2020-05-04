using UnityEngine;
using UnityEngine.Video;

public class VHSEffect : MonoBehaviour
{
    [SerializeField] private Shader shader;
    [SerializeField] private VideoClip VHSClip;

    private float scanLineY;
    private float scanLineX;
    private Material material;
    private VideoPlayer videoPlayer;
    private static readonly int VhsTex = Shader.PropertyToID("_VHSTex");
    private static readonly int YScanline = Shader.PropertyToID("_yScanline");
    private static readonly int XScanline = Shader.PropertyToID("_xScanline");

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

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetTexture(VhsTex, videoPlayer.texture);

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
        material.SetFloat(YScanline, scanLineY);
        material.SetFloat(XScanline, scanLineX);
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