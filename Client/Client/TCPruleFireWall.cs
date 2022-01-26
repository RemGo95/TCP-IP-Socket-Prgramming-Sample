using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using StackAlgorithm;


class TCPNewClientTest: MonoBehaviour{



    public void RESET()
    {
		ConnectToTcpServer();
    }

	/// <summary> 	
	/// Setup socket connection. 	
	/// </summary> 	
	private void ConnectToTcpServer()
	{
		try
		{
			clientReceiveThread = new Thread(new ThreadStart(ListenForData));
			clientReceiveThread.IsBackground = true;
			clientReceiveThread.Start();

			//Status = "Connected";
			//Debug.Log("Connected");
		}
		catch (Exception e)
		{
			Debug.Log("On client connect exception " + e);
			Status = "error during connection";
		}
	}
	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	private void ListenForData()
	{
		try
		{
			socketConnection = new TcpClient(ip, Int32.Parse(port));
			Byte[] bytes = new Byte[1024];
			Status = "Connected";
			Debug.Log("Connected");
			while (true)
			{
				
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream())
				{
					int length;
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						var incommingData = new byte[length];
						string DataForParser;
						Array.Copy(bytes, 0, incommingData, 0, length);
						// Convert byte array to string message. 						
						string serverMessage = Encoding.ASCII.GetString(incommingData);
						Debug.Log("server message received as: " + serverMessage);
						Data = serverMessage;
						DataForParser = Data;

						//Parser - proceed data from server
						ParseDataReciveString(Data);
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
			Status = "error during connection";
		}
	}
	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	private void SendMessage()
	{
		if (socketConnection == null)
		{
			return;
		}
		try
		{
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream();
			if (stream.CanWrite)
			{
				string clientMessage = "wiadomosc z unity";
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
				Debug.Log("Client sent his message - should be received by server");
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
			Status = "error during write";
		}
	}

	public void DisplayData()
    {
		if(Data != null)
        {
			DataFromServer.text = Data.ToString();
		}
		
		ServerInfo.text = ("Server: " + ip + "\n" + port + "\n" + Status).ToString();
		InputData.text = ("Id: " + _id + " X: " + _width + " Y: " + _height + " Z: " + _depth).ToString();
	}

	public void ParseDataReciveString(string inputData)
	{
		string[] splitedData = SplitData(inputData);

		//splitedData[0][0] = DataType

		if (splitedData[0][0] == '2')
		{
			DataType = 2;
			GetConfiguration(String.Concat(splitedData));
		}
		else if (splitedData[0][0] == '1')
		{
			DataType = 1;
			GetPackages(splitedData);
		}

	}

	public void GetDataFromServer()
    {

    }


	private void GetPackages(string[] inputData)
	{

		pakagesMetadata = Convert.ToByte(inputData[0][2]);
		string[] tmp;

		for (int i = 1; i < inputData.Length; i++)
		{
			tmp = inputData[i].Split(',');


			var y = packages.Find(x => x.id == Int16.Parse(tmp[0]));
			if (y != null) continue;
			_id = Int16.Parse(tmp[0]);
			_width = float.Parse(tmp[1]);
			_height = float.Parse(tmp[2]);
			_depth = float.Parse(tmp[3]);

			packages.Add(new Item(Int16.Parse(tmp[0]), float.Parse(tmp[1]), float.Parse(tmp[2]), float.Parse(tmp[3])));

			

			
		}
	}

	private void GetConfiguration(string inputData)
	{
		string[] configString = inputData.Split(',');

		if (configuration.id == Int16.Parse(configString[1]))
		{
			return;
		}

		actualType = Int16.Parse(configString[0]);
		configuration.id = Int16.Parse(configString[1]);
		configuration.palletSize_width = float.Parse(configString[2]);
		configuration.palletSize_height = float.Parse(configString[3]);
		configuration.palletSize_depth = float.Parse(configString[4]);
		configuration.palletSize_maxWeight = float.Parse(configString[5]);

	}
	private static string[] SplitData(string inputData)
	{
		string[] mainSplit;
		string dataString;
		mainSplit = inputData.Split(';');
		dataString = mainSplit[0];
		mainSplit = dataString.Split(':');

		return mainSplit;

	}

    public static void RemoveFirewallRules(string RuleName = "BreakermindCom")
    {
    try
    {
        Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
        INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
        var currentProfiles = fwPolicy2.CurrentProfileTypes;               

        // Lista rules
        // List<INetFwRule> RuleList = new List<INetFwRule>();

        foreach (INetFwRule rule in fwPolicy2.Rules)
        {
            // Add rule to list
            // RuleList.Add(rule);
            // Console.WriteLine(rule.Name);
            if (rule.Name.IndexOf(RuleName) != -1)
            {
                // Now add the rule
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));                     
                firewallPolicy.Rules.Remove(rule.Name);
                Console.WriteLine(rule.Name + " has been deleted from Firewall Policy");
            }
        }
    }
    catch (Exception r)
    {
        Console.WriteLine("Error delete rule from firewall");
    }
    }   




}
