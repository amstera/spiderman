using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutScenePlayer : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public RawImage CutScene;
    public VideoClip GoblinClip;
    public GameObject ControlsPanel;

    public AudioSource Dialog1;
    public AudioSource Dialog2;

    private float _timeSinceVidPlay;
    private int _cutscenesPlayed;

    private void Awake()
    {
        ClearTexture();
        CutScene.enabled = true;
        VideoPlayer.Prepare();
    }

    void Update()
    {
        if (!VideoPlayer.isPlaying && CutScene.enabled && (Time.time - _timeSinceVidPlay > 2))
        {
            CutScene.enabled = false;
            ClearTexture();
            VideoPlayer.Stop();
            VideoPlayer.clip = GoblinClip;
            VideoPlayer.Prepare();

            if (_cutscenesPlayed == 1)
            {
                Dialog2.Play();
            }
            else
            {
                ControlsPanel.SetActive(true);
                Dialog1.Play();
            }
            _cutscenesPlayed++;
        }
    }

    public void PlayGoblinClip()
    {
        ControlsPanel.SetActive(false);
        _timeSinceVidPlay = Time.time;
        CutScene.enabled = true;
        VideoPlayer.Play();
    }

    private void ClearTexture()
    {
        RenderTexture.active = VideoPlayer.targetTexture;
        GL.Clear(true, true, Color.black);
    }
}
