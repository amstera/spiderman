using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutScenePlayer : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public RawImage CutScene;
    public VideoClip GoblinClip;
    public VideoClip EndClip;
    public GameObject ControlsPanel;
    public SpiderMan Spiderman;
    public AudioSource GoblinScream;

    private float _timeSinceVidPlay;
    private int _cutscenesPlayed;

    private void Awake()
    {
        ClearTexture();
        CutScene.enabled = true;
        VideoPlayer.Prepare();
        Spiderman = FindObjectOfType<SpiderMan>();
    }

    void Update()
    {
        if (!VideoPlayer.isPlaying && CutScene.enabled && (Time.time - _timeSinceVidPlay > 1))
        {
            if (_cutscenesPlayed == 2)
            {
                SceneManager.LoadScene(1);
                return;
            }
            if (_cutscenesPlayed == 1)
            {
                Spiderman.PlayDialog(2);
            }
            else if (_cutscenesPlayed == 0)
            {
                ControlsPanel.SetActive(true);
                Spiderman.PlayDialog(1);
            }
            _cutscenesPlayed++;

            CutScene.enabled = false;
            ClearTexture();
        }
    }

    public void PlayGoblinClip()
    {
        PlayClip(GoblinClip);
    }

    public void PlayEndClip()
    {
        GoblinScream.Play();
        PlayClip(EndClip);
    }

    private void ClearTexture()
    {
        RenderTexture.active = VideoPlayer.targetTexture;
        GL.Clear(true, true, Color.black);
    }

    private void PlayClip(VideoClip clip)
    {
        _timeSinceVidPlay = Time.time;
        ClearTexture();
        CutScene.enabled = true;
        VideoPlayer.clip = clip;
        VideoPlayer.Prepare();
        VideoPlayer.Play();
    }
}
