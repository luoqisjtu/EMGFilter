using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class EmgModule 
{

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

	//Create a binary reader to read the data
	private BinaryReader reader;



    // Use this for initialization
    public void  startEmg () {
        commandSocket = new TcpClient("127.0.0.1", commandPort);
        emgSocket = new TcpClient("127.0.0.1", emgPort);
        commandStream = commandSocket.GetStream();
        commandReader = new StreamReader(commandStream, Encoding.ASCII);
        commandWriter = new StreamWriter(commandStream, Encoding.ASCII);
        emgStream = emgSocket.GetStream();
		emgStream.ReadTimeout = 5;    //set timeout

        string response = SendCommand(COMMAND_START);
		reader = new BinaryReader(emgStream);

        running = true;

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
        //Close all streams and connections
		reader.Close(); //close the reader. This also disconnects

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

    public float getOneSample()
    {



        try
        {
            //Demultiplex the data and save for UI display
            for (int sn = 0; sn < 16; ++sn)
            {
                emgData[sn] = reader.ReadSingle();
                
            }

        }
		catch (Exception e)
        {
			//Debug.Log (e);//ignore timeouts, but force a check of the running flag
        }

       


		return emgData[0];
        
    }

}
