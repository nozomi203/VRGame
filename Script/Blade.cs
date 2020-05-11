using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour {

    [SerializeField]
    private float bladeLength = 2;
    [SerializeField]
    private float bladeRadius = 0.02f;
    [SerializeField]
    private float speedThreshold = 10;
    [SerializeField]
    private int damage = 4;
    [SerializeField]
    private Vector3 bladeOffset;

    private bool isHit;
    private bool isHitContinue = false;
    private int layerMask;
    private Ray ray;
    private RaycastHit hit;


    //マウス位置
    private Vector3 _mousePosition;
    private Vector3 direction;
    private Vector3 prePos;
    private Vector3 proj;
    private float angle;
    private GameObject parentObj;


    public float GetBladeLength()
    {
        return bladeLength;
    }
    public float GetBladeRadius()
    {
        return bladeRadius;
    }
    public bool GetIsHit()
    {
        return isHit;
    }
    public RaycastHit GetHitInfo()
    {
        return hit;
    }

	// Use this for initialization
	void Start () {
        layerMask = LayerMask.NameToLayer("DrawLine");
        layerMask = 1 << layerMask;

        parentObj = transform.parent.gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        MoveBlade();
        //DetectBladeHit();
    }

    private void MoveBlade()
    {
        //マウス情報取得
        _mousePosition = Input.mousePosition;
        _mousePosition.z = 1;
        _mousePosition = Camera.main.ScreenToWorldPoint(_mousePosition);
        this.transform.position = _mousePosition;
        //刀の角度を変える
        direction = (this.transform.position - prePos) / Time.deltaTime;

        if (direction.magnitude > speedThreshold)
        {
            this.transform.forward = (this.transform.position - parentObj.transform.position - bladeOffset).normalized;
            direction = direction.normalized;

            proj = Vector3.ProjectOnPlane(direction, this.transform.forward).normalized;

            float dot = Vector3.Dot(this.transform.forward, Vector3.Cross(-this.transform.up, proj));

            angle = Vector3.Angle(-this.transform.up, proj) * ((dot > 0) ? 1 : -1);

            Quaternion rot = Quaternion.AngleAxis(angle, this.transform.forward);
            this.transform.rotation = rot * this.transform.rotation;

        }

        prePos = this.transform.position;


        //Vector3 pos = Input.mousePosition;
        //pos.z = 2;
        //pos = mainCamera.ScreenToWorldPoint(pos);
        //transform.position = pos;
        //transform.forward = (pos - mainCamera.transform.position).normalized;

    }

    private void DetectBladeHit()
    {
        ray = new Ray(transform.position, transform.forward);
        isHit = Physics.Raycast(ray, out hit, bladeLength, layerMask);
        if (isHit)
        {
            if (!isHitContinue)
            {
                isHitContinue = true;
                if (hit.collider.GetComponent<LifeManager>() != null)
                {
                    hit.collider.GetComponent<LifeManager>().Damage(damage);
                }
                else
                {
                    Transform trans = hit.collider.transform;
                    while (trans.name != "Armature")
                    {
                        trans = trans.parent;
                    }
                    trans.parent.GetComponentInChildren<LifeManager>().Damage(damage);
                }

            }
        }
        else
        {
            isHitContinue = false;
        }

    }
}
