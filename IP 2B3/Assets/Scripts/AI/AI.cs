using B3.BoardSystem;
using UnityEngine;
using System.Linq;
using System;
using B3.ResourcesSystem;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading;

public static class AI
{

    private static Thread listenerThread;
    private static volatile bool isListening = false;
    private static readonly ConcurrentQueue<string> moveQueue = new ConcurrentQueue<string>();
    private static int GET_MOVES_PORT = 7070; 

    public static void StartMoveListener()
    {
        if (isListening)
            return;

        isListening = true;
        listenerThread = new Thread(ListenForMoves);
        listenerThread.IsBackground = true;
        listenerThread.Start();
        Debug.Log("[Unity] Move listener thread started.");
    }

    public static void StopMoveListener()
    {
        isListening = false;
        listenerThread?.Join();
        Debug.Log("[Unity] Move listener thread stopped.");
    }

    private static void ListenForMoves()
    {
        try
        {
            TcpListener server = new TcpListener(IPAddress.Any, GET_MOVES_PORT);
            server.Start();

            while (isListening)
            {
                if (server.Pending())
                {
                    using (TcpClient client = server.AcceptTcpClient())
                    using (StreamReader reader = new StreamReader(client.GetStream()))
                    {
                        string move = reader.ReadLine();
                        if (!string.IsNullOrEmpty(move))
                        {
                            moveQueue.Enqueue(move);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(10); // Avoid tight loop
                }
            }

            server.Stop();
        }
        catch (Exception ex)
        {
            Debug.LogError("[Unity] Listener error: " + ex.Message);
        }
    }

    public static string PollMove()
    {
        if (moveQueue.TryDequeue(out string move))
        {
            ProcessMove(move);
            return move;
        }
        return "NONE";
    }

    private static void ProcessMove(string msg)
    {
        string[] parts = msg.Split(' ');
        if (parts.Length == 0)
            return;

        switch (parts[0].ToUpper())
        {
            case "BUILD":
                Debug.Log("[Unity] Building: " + msg);
                // Handle building logic here
                break;
            case "MOVE":
                Debug.Log("[Unity] Moving: " + msg);
                // Handle moving logic here
                break;
            case "TRADE":
                Debug.Log("[Unity] Trading: " + msg);
                // Handle trading logic here
                break;
            case "END":
                Debug.Log("[Unity] Ending turn: " + msg);
                // Handle end turn logic here
                break;
            default:
                Debug.LogWarning("[Unity] Unknown command: " + msg);
                break;
        }
    }
    private static void BuildFunction(string [] parts)
    {
       switch (parts[1].ToUpper())
        {
            case "SETTLEMENT":
                housePosition = new HexPosition(int.Parse(parts[2]), int.Parse(parts[3]));
                houseDir = (HexVertexDir)Enum.Parse(typeof(HexVertexDir), parts[4]);
                Debug.Log("[Unity] Building a settlement at: " + parts[2] + ", " + parts[3] + " in direction: " + parts[4]);
                break;
            case "CITY":
                // Logic to build a city
                break;
            case "ROAD":
                roudPosition = new HexPosition(int.Parse(parts[2]), int.Parse(parts[3]));
                roudDir = (HexEdgeDir)Enum.Parse(typeof(HexEdgeDir), parts[4]);
                Debug.Log("[Unity] Building a road at: " + parts[2] + ", " + parts[3] + " in direction: " + parts[4]);
                break;
            default:
                break;
        }
    }

    

    public static void SendBoard(ResourceType?[] resources, int[] numbers, ResourceType?[] ports)
    {
        return;
        int[] indexSwap = { 6, 5, 4, 3, 2, 1, 0, 8, 7 };
        // swap the port with the new index
        var swappedPorts = ports.ToArray();
        for (int i = 0; i < ports.Length; i++)
            swappedPorts[indexSwap[i]] = ports[i];

        var res = resources.Select(r => r.HasValue ? (r.Value.ToString().ToLower() == "brick" ? "clay" : r.Value.ToString().ToLower()) : "null");
        var nums = numbers.Select(r => r.ToString());
        var port = swappedPorts.Select(r => r.HasValue ? (r.Value.ToString().ToLower() == "brick" ? "clay" : r.Value.ToString().ToLower()) : "null");

        try
        {
            TcpListener server = new TcpListener(IPAddress.Any, 6969);
            server.Start();
            Debug.Log("Server is running on port 6969...");

            TcpClient client = server.AcceptTcpClient();
            Debug.Log("Client connected!");

            using (StreamWriter writer = new StreamWriter(client.GetStream()))
            {
                // Send hex types
                writer.Write("HEX_TYPES");
                writer.Write(" " + string.Join(" ", res));
                writer.WriteLine();

                // Send dice numbers
                writer.Write("DICE_NUMBERS");
                writer.Write(" " + string.Join(" ", nums));
                writer.WriteLine();

                // Send ports
                writer.Write("PORTS");
                writer.Write(" " + string.Join(" ", port));
                writer.WriteLine();

                Debug.Log("Data sent to client.");
            }
            client.Close();
            server.Stop();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    private static HexPosition housePosition = new HexPosition(0, 0);
    private static HexVertexDir houseDir = HexVertexDir.Left;

    private static HexPosition roudPosition = new HexPosition(0, 0);
    private static HexEdgeDir roudDir = HexEdgeDir.Top;


    // hex ul selectat de ai, coltu hex ului
    private static int i = 0;
    public static (HexPosition, HexVertexDir) GetHousePosition()
    {

        return (housePosition, houseDir);

    }

    public static (HexPosition, HexVertexDir) GetCityPosition()
    {
        return (new HexPosition(0, 0), HexVertexDir.Left);
    }

    public static (HexPosition, HexEdgeDir) GetRoadPosition()
    {

        return (roudPosition, roudDir);
    }

    public static HexPosition GetThiefPosition()
    {
        return new HexPosition(0, 0);
    }

    public static int[] GetDiscardedResources()
    {
        return new int[5];
    }

    //playerId si array-ul de resurse pentru trade
    public static (int, int[]) GetPlayerTradeInfo()
    {
        return (0, new int[5]);
    }

    //resursele pe care le dai, resursa pe care o primesti
    public static (int[], int[]) GetBankTradeInfo()
    {
        return (null, null);
    }

    //check OnTradeAndBuildUpdate() din AIPlayer pentru toate comenzile
    public static String GetFreeStateCommand()
    {
        return "end turn";//string freestate
    }

    public static void SendMove(string message)
    {
        Debug.Log("Sending move: " + message);
         return;
        try
        {
            TcpListener server = new TcpListener(IPAddress.Any, 6868);
            server.Start();
            Debug.Log("Server is running on port 6868...");

            TcpClient client = server.AcceptTcpClient();
            Debug.Log("Client connected!");
            //o sa am public static void getmove cu while message != endturn
            using (StreamWriter writer = new StreamWriter(client.GetStream()))
            {
                // Send move
                writer.Write(message);
                writer.WriteLine();

                Debug.Log("Data sent to client.");
            }
            client.Close();
            server.Stop();
        }
        catch (Exception ex)
        {
            // Debug.Log(ex.ToString());
        }
    }


    
    public static void SendDice(int dice)
    {
        return;
        try
        {
            TcpListener server = new TcpListener(IPAddress.Any, 6969);
            server.Start();
            Debug.Log("Server is running on port 6969...");

            TcpClient client = server.AcceptTcpClient();
            Debug.Log("Client connected!");

            using (StreamWriter writer = new StreamWriter(client.GetStream()))
            {
                writer.WriteLine("DICE_NUMBER " + dice);
                Debug.Log("Sent single dice number to client: " + dice);
            }

            client.Close();
            server.Stop();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
}

