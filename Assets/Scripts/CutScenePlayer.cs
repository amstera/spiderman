using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutScenePlayer : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public RawImage CutScene;
    public VideoClip GoblinClip;

    private float _timeSinceVidPlay;

    private void Awake()
    {
        ClearTexture();
        CutScene.enabled = true;
        VideoPlayer.Prepare();
    }

    void Update()
    {
        if (!VideoPlayer.isPlaying && (Time.time - _timeSinceVidPlay > 2))
        {
            CutScene.enabled = false;
            ClearTexture();
            VideoPlayer.Stop();
            VideoPlayer.clip = GoblinClip;
            VideoPlayer.Prepare();
        }
    }

    public void PlayGoblinClip()
    {
        _timeSinceVidPlay = Time.time;
        VideoPlayer.Play();
        CutScene.enabled = true;
    }

    private void ClearTexture()
    {
        RenderTexture.active = VideoPlayer.targetTexture;
        GL.Clear(true, true, Color.black);
    }
}
