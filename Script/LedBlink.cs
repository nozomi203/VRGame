using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedBlink : MonoBehaviour {

    [SerializeField]
    private int[] GVSID;
    [SerializeField]
    private SerialHandler serialHandler;
    [SerializeField, Range(0, 8191)]
    private int[] frequency;
    [SerializeField, Range(0, 4095)]
    private int[] current;
    [SerializeField]
    private GVSAccess.WaveForm[] mode;
    GVSAccess gvsAccess;

    //データ送信中
    private bool isSending = false;

    public float SquareDuty90(float phase)
    {
        if(phase < 0.9f)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    private void Start()
    {
        gvsAccess = GetComponent<GVSAccess>();
    }

    // Update is called once per frame
    void Update () {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isSending)
            {
                isSending = true;
                for (int i = 0; i < GVSID.Length; i++)
                {
                    gvsAccess.SendGVSParam(serialHandler, GVSID[i], current[i], mode[i], frequency[i]);
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isSending = false;
            for (int i = 0; i < GVSID.Length; i++)
            {
                gvsAccess.SendGVSParam(serialHandler, GVSID[i], 0, 0, 0);
            }
        }
        /*
        if (Input.GetKeyUp(KeyCode.Space))
        {
            gvsAccess.SetCustomWave(serialHandler, SquareDuty90);
        }
        */
    }

    private void OnDestroy()
    {
        Debug.Log("Finalize.");
        for (int i = 0; i < GVSID.Length; i++)
        {
            gvsAccess.SendGVSParam(serialHandler, GVSID[i], 0, 0, 0);
        }
    }
}
