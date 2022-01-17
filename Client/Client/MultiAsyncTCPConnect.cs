using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.ComponentModel;


public class MultiAsyncFunctions : MonoBehaviour
{
    //public byte[] buffer;
    
    private TcpClient client;
    public StreamReader STR;
    public StreamWriter STW;
    public string recieve;
    public String TextToSend = ("Wiadomosc od aplikacji");
    string ip = "192.168.1.29";
    string port = "12000";
    System.Diagnostics.Stopwatch timer14 = new System.Diagnostics.Stopwatch();
    System.Diagnostics.Stopwatch timer15 = new System.Diagnostics.Stopwatch();
    //byte[] buffer;
    // Start is called before the first frame update

    class ThreadCreationProgram
    {
        public Thread childThread;
        public ThreadStart childRef;
        //public byte SendData = (byte);

        public ThreadCreationProgram()
        {
            ThreadStart childRef = new ThreadStart(CallToChildThread);
            Debug.Log("!!!!!!!!!!!!!Started new Task...");
        }

        public void StartThread()
        {
            childThread.Start();
        }

        public static void CallToChildThread()
        {
            string ip = "192.168.1.29";
            string port = "12000";
            TcpClient tCPClient = new TcpClient(ip,Int16.Parse(port));
            tCPClient.Connect(ip, Int16.Parse(port));
            tCPClient.GetStream();
            //tCPClient.SendRecive();
            //while (true)
            //{
                byte[] buffer = new byte[124];
                //tCPClient.Client.Receive(buffer);
                Receive(tCPClient.Client, buffer, 0, buffer.Length, 100000);
                string recieve = buffer.ToString();
                
                //tCPClient.Client.Receive(recieve);

                //tCPClient.Client.Send(Byte.Parse("YYYYYYYYYY"));

                Debug.Log("Data from server: " + recieve);
                if (recieve == "c" || !tCPClient.Connected)
                {
                    tCPClient.Close();
                    //break;
                }
                recieve = "";
                Thread.Sleep(500);
           // }
            Debug.Log("End");
        }

        public void RestroreThread()
        {
            Thread.Sleep(50000);
            childRef = new ThreadStart(CallToChildThread);
            Debug.Log("Restore Thread");
            childThread = new Thread(childRef);
        }
    }

    public void Start()
    {
        //StartCoroutine(RunTask());
        ThreadCreationProgram myThread = new ThreadCreationProgram();
        myThread.StartThread();

        while (true)
        {
            if (!myThread.childThread.IsAlive)
            {
                //myThread.RestroreThread();
                myThread.StartThread();
            }
            Debug.Log("Main Thread Running");
            Thread.Sleep(2000);
        }
    }

    //IEnumerator RunTask()
    //{
    //    yield return RunTaskAsync().AsIEnumerator();
    //}

    //async Task RunTaskAsync()
    //{
    //    // run async code
    //}

    // Update is called once per frame
    void Update()
    {
        
        

    }

    public static void Send(Socket socket, byte[] buffer, int offset, int size, int timeout)
    {
        int startTickCount = Environment.TickCount;
        int sent = 0;  // how many bytes is already sent
        do
        {
            if (Environment.TickCount > startTickCount + timeout)
                throw new Exception("Timeout.");
            try
            {
                sent += socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.WouldBlock ||
                    ex.SocketErrorCode == SocketError.IOPending ||
                    ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                {
                    // socket buffer is probably full, wait and try again
                    Thread.Sleep(30);
                }
                else
                    throw ex;  // any serious error occurr
            }
        } while (sent < size);
    }

    public static void Receive(Socket socket, byte[] buffer, int offset, int size, int timeout)
    {
        int startTickCount = Environment.TickCount;
        int received = 0;  // how many bytes is already received
        do
        {
            if (Environment.TickCount > startTickCount + timeout)
                throw new Exception("Timeout.");
            try
            {
                received += socket.Receive(buffer, offset + received, size - received, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.WouldBlock ||
                    ex.SocketErrorCode == SocketError.IOPending ||
                    ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                {
                    // socket buffer is probably empty, wait and try again
                    Thread.Sleep(30);
                }
                else
                    throw ex;  // any serious error occurr
            }
        } while (received < size);
    }







}
