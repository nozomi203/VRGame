using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour {

    [Serializable]
    public struct AnimationProb
    {
        public string name;
        [Range(0, 1)]
        public float prob;
    }
    [SerializeField]
    protected AnimationProb[] animationProb;

    protected GameObject player;
    protected Animator animator;
    protected EnemyGenerator enemyGenerator = null;
    protected bool isAnimPlay = false;
    protected IEnumerator playAnimCrtn;

    private void Start()
    {
        Init();
        MoveToPlayer(1.8f, 1, SelectAnim);
    }

    public void Die()
    {
        enemyGenerator.EnemyDie();
        InterruptAnim("DIE", "DIE");
    }
    public void SetEnemyGenerator(EnemyGenerator eg)
    {
        enemyGenerator = eg;
    }
    protected void Init()
    {
        player = GameObject.FindWithTag("Player");
        animator = GetComponentInChildren<Animator>();
    }
    protected  void SelectAnim()
    {
        int idx = 0;
        float r = UnityEngine.Random.value;
        float sumP = 0;
        for(int i = 0; i < animationProb.Length; i++)
        {
            sumP += animationProb[i].prob;
            if(r <= sumP)
            {
                idx = i;
            }
        }
        PlayAnim(animationProb[idx].name, "STAY", 2f, SelectAnim);
    }
    protected Vector3 GetPlayerDir()
    {
        return Vector3.ProjectOnPlane((player.transform.position - transform.position), Vector3.up).normalized;
    }
    protected float GetPlayerDist()
    {
        return Vector3.ProjectOnPlane((player.transform.position - transform.position), Vector3.up).magnitude;
    }
    protected void MoveToPlayer(float minDist, float moveSpeed, System.Action callback)
    {
        StartCoroutine(MoveToPlayerCrtn(minDist, moveSpeed, callback));
    }
    protected void LookPlayer()
    {
        this.transform.forward = GetPlayerDir();
    }
    protected IEnumerator MoveToPlayerCrtn(float minDist, float moveSpeed, System.Action callback)
    {
        LookPlayer();
        PlayAnim("WALK", "WALK");
        while(GetPlayerDist() > minDist)
        {
            transform.position += GetPlayerDir() * moveSpeed * Time.deltaTime;
            yield return null;
        }
        PlayAnim("STAY", "STAY", 0, callback);
    }
    protected void PlayAnim(string animName, string callBackAnim, float callBackDelay = 0, System.Action callback = null)
    {
        if (!isAnimPlay)
        {
            isAnimPlay = true;
            playAnimCrtn = PlayAnimCrtn(animName, callBackAnim, callBackDelay, callback);
            StartCoroutine(playAnimCrtn);
        }
    }
    protected void InterruptAnim(string animName, string callBackAnim, float callbackDelay = 0, System.Action callback = null)
    {
        isAnimPlay = true;
        if (playAnimCrtn != null) StopCoroutine(playAnimCrtn);
        playAnimCrtn = null;
        playAnimCrtn = PlayAnimCrtn(animName, callBackAnim, callbackDelay, callback);
        StartCoroutine(playAnimCrtn);
    }
    protected IEnumerator PlayAnimCrtn(string animName, string callBackAnim, float callBackDelay = 0, System.Action callback = null)
    {
        animator.CrossFadeInFixedTime(animName, 0.1f);
        yield return new WaitForSeconds(0.1f);
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
        animator.CrossFadeInFixedTime(callBackAnim, 0.1f);
        yield return new WaitForSeconds(callBackDelay);
        isAnimPlay = false;
        callback?.Invoke();
    }
}
