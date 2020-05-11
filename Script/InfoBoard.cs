using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InfoBoard : MonoBehaviour
{
    [SerializeField]
    private AudioClip countSound;
    [SerializeField]
    private AudioClip startSound;
    [SerializeField]
    AudioClip endSound;
    [SerializeField]
    AudioClip fadeSound;

    private Material materialP1;
    private Material materialP2;

    private AudioSource audioSource;
    private Text text;
    private Text killCountText;
    private int killCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        materialP1 = transform.parent.Find("pillar1").GetComponent<Renderer>().material;
        materialP2 = transform.parent.Find("pillar2").GetComponent<Renderer>().material;
        audioSource = GetComponent<AudioSource>();
        text = transform.Find("InfoBoard").Find("Main").GetComponent<Text>();
        killCountText = transform.Find("InfoBoard").Find("KillCount").GetComponent<Text>();
        text.text = "GVS Battle";
    }

    public void StartGame()
    {
        float emission = 0;
        DOVirtual.Float(0f, 10f, 3f, value =>
        {
            emission = value;
            materialP1.SetFloat("_EmissionRate", emission);
            materialP2.SetFloat("_EmissionRate", emission);
            emission = value;
        }).OnComplete(FadeIn);
    }
    public void Count(int time)
    {
        StartCoroutine(CountCrtn(time));
    }
    public void EndGame(string loser)
    {
        StartCoroutine(EndGameCrtn(loser));
    }
    public void UpdateKillCount()
    {
        killCount += 1;
        killCountText.text = "Count: " + killCount.ToString("D2");
    }
    public void FadeIn()
    {
        PlaySound(fadeSound);
        transform.DOScaleY(1, 0.5f);
    }
    public void FadeOut()
    {
        PlaySound(fadeSound);
        transform.DOScaleY(0, 0.5f);
    }
    private void PlaySound(AudioClip ac)
    {
        audioSource.clip = ac;
        audioSource.Play();
    }

    private IEnumerator CountCrtn(int time)
    {
        for(int t = time; t > 0; t--)
        {
            text.text = t.ToString();
            PlaySound(countSound);
            yield return new WaitForSeconds(1f);
        }
        text.text = "Fight";
        PlaySound(startSound);
        yield return new WaitForSeconds(0.5f);
        //FadeOut();

    }
    private IEnumerator EndGameCrtn(string loser)
    {
        if(loser == "Enemy")
        {
            text.text = "You Win!";
        }
        else
        {
            text.text = "You Lose...";
        }

        PlaySound(endSound);
        yield return new WaitForSeconds(1f);
        FadeIn();
    }
}
