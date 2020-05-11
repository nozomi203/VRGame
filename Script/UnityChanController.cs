using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanController : MonoBehaviour
{

    private Animator animator;
    private bool isAnimPlay = false;
    private Vector3 lookPos;

    private IEnumerator playAnimCrtn = null;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        lookPos = Camera.main.transform.position;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayAnim("SLASH_LR");
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayAnim("SLASH_RL");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayAnim("SLASH_TD");
        }
    }

    public void RepelWeapon()
    {
        Debug.Log("Repel");
        InterruptAnim(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name + "_INTR");
    }

    public void Die()
    {
        isAnimPlay = true;
        if (playAnimCrtn != null) StopCoroutine(playAnimCrtn);
        playAnimCrtn = null;
        animator.CrossFadeInFixedTime("DIE", 0.1f);
    }

    private void PlayAnim(string animName)
    {
        if (!isAnimPlay)
        {
            isAnimPlay = true;
            playAnimCrtn = PlayAnimCrtn(animName);
            StartCoroutine(playAnimCrtn);
        }
    }
    private void InterruptAnim(string animName)
    {
        isAnimPlay = true;
        if(playAnimCrtn != null) StopCoroutine(playAnimCrtn);
        playAnimCrtn = null;
        playAnimCrtn = PlayAnimCrtn(animName);
        StartCoroutine(playAnimCrtn);
    }

    private IEnumerator PlayAnimCrtn(string animName)
    {
        animator.CrossFadeInFixedTime(animName, 0.1f);
        yield return new WaitForSeconds(0.1f);
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
        animator.CrossFadeInFixedTime("STAY", 0.1f);
        yield return new WaitForSeconds(0.1f);
        isAnimPlay = false;
    }
    
    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetLookAtWeight(1, 0, 0.5f, 0,0);
        animator.SetLookAtPosition(lookPos);
    }
    
}
