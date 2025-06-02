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
using System.Collections;

public static class AI
{
     public static int robberX = 0;
    public static int robberY = 0;
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
        // non‐blocking
        if (moveQueue.TryDequeue(out string move))
            return move;
        return "NONE";
    }
    public static bool canmovethief;
    public static void ProcessMove(string msg)
    {
        string[] parts = msg.Split(' ');
        if (parts.Length == 0)
            return;

        switch (parts[0].ToUpper())
        {
            case "BUILD":
                Debug.Log("[Unity] Building: " + msg);
                BuildFunction(parts);
                // Handle building logic here
                break;
            case "BUY":
                Debug.Log("[Unity] Buying: " + msg);
                BuyFunction(parts);
                // Handle building logic here
                break;
           case "MOVEROBBER":
                // Așteptăm formatul: "MOVEROBBER <x> <y>"
                if (parts.Length >= 3
                    && int.TryParse(parts[1], out int rx)
                    && int.TryParse(parts[2], out int ry))
                {
                    robberX = rx;
                    robberY = ry;
                    canmovethief = true;
  
                    Debug.Log($"[Unity] Received MOVEROBBER => x={rx}, y={ry}");
                }
                else
                {
                    Debug.LogWarning("[Unity] Mesaj MOVEROBBER incorect: " + msg);
                }
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
    private static HexVertexDir ParseHexVertexDir(int dir)
    {
        // dir = (dir + 1) % 6; // Normalize to 0-5 range
       switch (dir)
        {
            case 0: return HexVertexDir.TopLeft;
            case 1: return HexVertexDir.TopRight;
            case 2: return HexVertexDir.Right;
            case 3: return HexVertexDir.BottomRight;
            case 4: return HexVertexDir.BottomLeft;
            case 5: return HexVertexDir.Left;
            default: throw new ArgumentOutOfRangeException(nameof(dir), "Invalid direction value");
        }
    }
    private static void BuildFunction(string [] parts)
    {
       switch (parts[1].ToUpper())
        {
            case "SETTLEMENT":
                housePosition = new HexPosition(int.Parse(parts[2]), int.Parse(parts[3]));
                houseDir = ParseHexVertexDir(int.Parse(parts[4]));
                Debug.Log("[Unity] Building a settlement at: " + parts[2] + ", " + parts[3] + " in direction: " + parts[4]);
                houseReady = true; // Set the flag to true to indicate that the house position is ready
                break;
            case "CITY":
                cityPosition = new HexPosition(int.Parse(parts[2]), int.Parse(parts[3]));
                cityDir = ParseHexVertexDir(int.Parse(parts[4]));
                Debug.Log("[Unity] Building a city at: " + parts[2] + ", " + parts[3] + " in direction: " + parts[4]);
                cityReady = true; // Set the flag to true to indicate that the city position is ready
                break;
            case "ROAD":
                roadPosition = new HexPosition(int.Parse(parts[2]), int.Parse(parts[3]));
                roadDir = (HexEdgeDir)Enum.Parse(typeof(HexEdgeDir), parts[4]);
                Debug.Log("[Unity] Building a road at: " + parts[2] + ", " + parts[3] + " in direction: " + parts[4]);
                roadReady = true; // Set the flag to true to indicate that the road position is ready
                break;
            default:
                break;
        }
    }

    private static void BuyFunction(string [] parts)
    {
       switch (parts[1].ToUpper())
        {
            case "SETTLEMENT":
                freeState = "buy house";
                housePosition = new HexPosition(int.Parse(parts[2]), int.Parse(parts[3]));
                houseDir = ParseHexVertexDir(int.Parse(parts[4]));
                Debug.Log("[Unity] Building a settlement at: " + parts[2] + ", " + parts[3] + " in direction: " + parts[4]);
                houseReady = true; // Set the flag to true to indicate that the house position is ready
                freeStateReady = true; // Set the flag to true to indicate that the free state command is ready
                break;
            case "CITY":
                freeState = "buy city";
                cityPosition = new HexPosition(int.Parse(parts[2]), int.Parse(parts[3]));
                cityDir = ParseHexVertexDir(int.Parse(parts[4]));
                Debug.Log("[Unity] Building a city at: " + parts[2] + ", " + parts[3] + " in direction: " + parts[4]);
                cityReady = true; // Set the flag to true to indicate that the city position is ready
                freeStateReady = true; // Set the flag to true to indicate that the free state command is ready
                break;
            case "ROAD":
                freeState = "buy road";
                roadPosition = new HexPosition(int.Parse(parts[2]), int.Parse(parts[3]));
                roadDir = (HexEdgeDir)Enum.Parse(typeof(HexEdgeDir), parts[4]);
                Debug.Log("[Unity] Building a road at: " + parts[2] + ", " + parts[3] + " in direction: " + parts[4]);
                roadReady = true; // Set the flag to true to indicate that the road position is ready
                freeStateReady = true; // Set the flag to true to indicate that the free state command is ready
                break;
            default:
                break;
        }
    }

    

    public static void SendBoard(ResourceType?[] resources, int[] numbers, ResourceType?[] ports)
    {
        // return;
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

    public static HexPosition housePosition = new HexPosition(0, 0);
    public static HexVertexDir houseDir = HexVertexDir.Left;

    public static HexPosition roadPosition = new HexPosition(0, 0);
    public static HexEdgeDir roadDir = HexEdgeDir.Top;

    public static HexPosition cityPosition = new HexPosition(0, 0);
    public static HexVertexDir cityDir = HexVertexDir.Left;

    private static string freeState = "";

    //check OnTradeAndBuildUpdate() din AIPlayer pentru toate comenzile
    public static bool freeStateReady = false;

    public static string GetFreeStateCommand()
    {
      return freeState;
    }

    public static bool houseReady = false;
    public static IEnumerator GetHousePosition(Action<HexPosition, HexVertexDir> callback)
    {
        // Wait until the flag is set by the ProcessMove
        yield return new WaitUntil(() => houseReady);


        callback?.Invoke(housePosition, houseDir);
        houseReady = false; 
    }

    public static bool roadReady = false;

    public static IEnumerator GetRoadPosition(Action<HexPosition, HexEdgeDir> callback)
    {
        // Wait until the flag is set by the ProcessMove
        yield return new WaitUntil(() => roadReady);


        callback?.Invoke(roadPosition, roadDir);
        roadReady = false; 
    }

    

    private static bool cityReady = false;

    public static IEnumerator GetCityPosition(Action<HexPosition, HexVertexDir> callback)
    {
        // Wait until the flag is set by the ProcessMove
        yield return new WaitUntil(() => cityReady);

        callback?.Invoke(cityPosition, cityDir);
        cityReady = false; 
    }




    public static IEnumerator GetThiefPosition()
    {
        Debug.Log("imi intra in get thief");
        while (canmovethief is false)
            yield return null;
                 canmovethief = false;
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



    public static void SendMove(string message)
    {
        Debug.Log("Sending move: " + message);

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
            Debug.Log(" merge aici send dice ");
            client.Close();
            server.Stop();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
}

