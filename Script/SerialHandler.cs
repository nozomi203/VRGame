using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class SerialHandler : MonoBehaviour
{
    public delegate void SerialDataReceivedEventHandler(string message);
    public event SerialDataReceivedEventHandler OnDataReceived = delegate { };

    public string portName = "COM3";
    public int baudRate = 9600;

    private SerialPort serialPort_;
    private Thread thread_;
    private bool isRunning_ = false;

    private string message_;
    private bool isNewMessageReceived_ = false;

    private void Awake()
    {
        Open();
    }

    // Update is called once per frame
    void Update()
    {

        if (isNewMessageReceived_)
        {
            OnDataReceived(message_);
            Debug.Log(message_);
        }
    }

    private void OnDestroy()
    {
        Close();
    }

    private void Open()
    {
        serialPort_ = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
        serialPort_.ReadTimeout = 5;
        serialPort_.NewLine = "\n";
        serialPort_.Open();

        isRunning_ = true;

        thread_ = new Thread(Read);
        thread_.Start();
    }

    private void Close()
    {
        isRunning_ = false;
        if(thread_ != null && thread_.IsAlive)
        {
            thread_.Join();
        }

        if(serialPort_ != null && serialPort_.IsOpen)
        {
            serialPort_.Close();
            serialPort_.Dispose();
        }
    }
    private void Read()
    {
        while(isRunning_ && serialPort_ != null && serialPort_.IsOpen)
        {
            try
            {
                    message_ = serialPort_.ReadLine();
                    isNewMessageReceived_ = true;
                    //Debug.Log(message_);            
            }
            catch(System.Exception e)
            {
                //Debug.LogWarning(e.Message);
            }
        }
    }

    public void Write(byte[] message)
    {
        try
        {
            serialPort_.Write(message, 0, message.Length);
        }catch(System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
