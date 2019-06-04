using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class EmgModule 
{

    private Thread emgThread;
    private TcpClient commandSocket;
    private TcpClient emgSocket;
    private const int commandPort = 50040;  //server command port
    private const int emgPort = 50041;
    private NetworkStream commandStream;
    private NetworkStream emgStream;
    private const string COMMAND_START = "START";
    public float[] emgData = new float[16];
    private StreamReader commandReader;
    private StreamWriter commandWriter;
    private bool connected = true; //true if connected to server
    private bool running = false;   //true when acquiring data



    // Use this for initialization
    public void  startEmg () {

        emgThread = new Thread(emgWorker);
        emgThread.IsBackground = true;
        commandSocket = new TcpClient("127.0.0.1", commandPort);
        emgSocket = new TcpClient("127.0.0.1", emgPort);
        commandStream = commandSocket.GetStream();
        commandReader = new StreamReader(commandStream, Encoding.ASCII);
        commandWriter = new StreamWriter(commandStream, Encoding.ASCII);
        emgStream = emgSocket.GetStream();
        string response = SendCommand(COMMAND_START);

        running = true;


        emgThread.Start();
    }
    private string SendCommand(string command)
    {
        string response = "";

        //Check if connected
        if (connected)
        {
            //Send the command

            commandWriter.WriteLine(command);
            commandWriter.WriteLine();  //terminate command
            commandWriter.Flush();  //make sure command is sent immediately

            //Read the response line and display    
            response = commandReader.ReadLine();
            commandReader.ReadLine();   //get extra line terminator

        }
        else
            Debug.Log("Not connected.");
        return response;    //return the response we got
    }
    public void stopEmg()
    {
        running = false;    //no longer running
                            //Wait for threads to terminate
        emgThread.Join();

        //Close all streams and connections
        commandStream.Close();
        commandSocket.Close();
        emgStream.Close();
        emgSocket.Close();
        commandReader.Close();
        commandWriter.Close();


    }

    // Update is called once per frame
    void Update () {
		
	}
    private void emgWorker()
    {
        emgStream.ReadTimeout = 5;    //set timeout

        //Create a binary reader to read the data
        BinaryReader reader = new BinaryReader(emgStream);



        while (running)
        {
            try
            {
                //Demultiplex the data and save for UI display
                for (int sn = 0; sn < 16; ++sn)
                {
                    emgData[sn] = reader.ReadSingle();
                    //Debug.Log(emgData[0]);
                }


            }
            catch
            {
                //ignore timeouts, but force a check of the running flag
            }

        }

        
        reader.Close(); //close the reader. This also disconnects
        
    }

}
