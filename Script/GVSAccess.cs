using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GVSAccess : MonoBehaviour
{
    [System.Serializable]
    public enum WaveForm
    {
        Constant = 0,
        Sin = 1,
        Square=2,
        Custom=3
    };

    public void SendGVSParam(SerialHandler serialHandler, int channel, int current, WaveForm mode, int freq, bool useHigeFreq = false)
    {
        byte dat1 = 0;
        byte dat2 = 0;
        byte dat3 = 0;
        byte dat4 = 0;
        //dat1
        if (useHigeFreq) dat1 += 128;
        if ((channel & 0x02) != 0) dat1 += 64;
        if ((channel & 0x01) != 0) dat1 += 32;
        if ((current & 0x800) != 0) dat1 += 8;
        if ((current & 0x400) != 0) dat1 += 4;
        if ((current & 0x200) != 0) dat1 += 2;
        if ((current & 0x100) != 0) dat1 += 1;
        //dat2
        if ((current & 0x80) != 0) dat2 += 128;
        if ((current & 0x40) != 0) dat2 += 64;
        if ((current & 0x20) != 0) dat2 += 32;
        if ((current & 0x10) != 0) dat2 += 16;
        if ((current & 0x08) != 0) dat2 += 8;
        if ((current & 0x04) != 0) dat2 += 4;
        if ((current & 0x02) != 0) dat2 += 2;
        if ((current & 0x01) != 0) dat2 += 1;
        //dat3
        if (((int)mode & 0x02) != 0) dat3 += 128;
        if (((int)mode & 0x01) != 0) dat3 += 64;
        if (current != 0) dat3 += 32;
        if ((freq & 0x1000) != 0) dat3 += 16;
        if ((freq & 0x800) != 0) dat3 += 8;
        if ((freq & 0x400) != 0) dat3 += 4;
        if ((freq & 0x200) != 0) dat3 += 2;
        if ((freq & 0x100) != 0) dat3 += 1;
        //dat4
        if ((freq & 0x80) != 0) dat4 += 128;
        if ((freq & 0x40) != 0) dat4 += 64;
        if ((freq & 0x20) != 0) dat4 += 32;
        if ((freq & 0x10) != 0) dat4 += 16;
        if ((freq & 0x08) != 0) dat4 += 8;
        if ((freq & 0x04) != 0) dat4 += 4;
        if ((freq & 0x02) != 0) dat4 += 2;
        if ((freq & 0x01) != 0) dat4 += 1;

        byte[] dat = new byte[5] { 0, dat1, dat2, dat3, dat4 };
        serialHandler.Write(dat);

    }

    public void SetCustomWave(SerialHandler serialHandler, System.Func<float, float> func)
    {
        byte[] dat = new byte[1000];
        byte[] h = new byte[1] { 1 };
        serialHandler.Write(h);
        for (int i = 0; i < 1000; i++)
        {
            dat[i] = (byte)(func(0.001f * i) * 255);
        }
        for(int i = 0; i < 200; i++)
        {
            byte[] dat_div = new byte[5];
            System.Array.Copy(dat, 5 * i, dat_div, 0, 5);
            serialHandler.Write(dat_div);
        }
    }
}