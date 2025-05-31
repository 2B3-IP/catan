using B3.BoardSystem;
using UnityEngine;
using System.Linq;
using System;
using B3.ResourcesSystem;
using System.IO;
using System.Net;
using System.Net.Sockets;

public static class AI
{

    public static void SendBoard(ResourceType?[] resources, int[] numbers, ResourceType?[] ports)
    {
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

    // hex ul selectat de ai, coltu hex ului
    public static (HexPosition, HexVertexDir) GetHousePosition()
    {
        return (new HexPosition(0, 0), HexVertexDir.Left);
    }

    public static (HexPosition, HexVertexDir) GetCityPosition()
    {
        return (new HexPosition(0, 0), HexVertexDir.Left);
    }

    public static (HexPosition, HexEdgeDir) GetRoadPosition()
    {
        return (new HexPosition(0, 0), HexEdgeDir.TopLeft);
    }

    public static HexPosition GetThiefPostion()
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
    public static (ResourceType, ResourceType) GetBankTradeInfo()
    {
        return (ResourceType.Brick, ResourceType.Brick);
    }

    //check OnTradeAndBuildUpdate() din AIPlayer pentru toate comenzile
    public static String GetFreeStateCommand()
    {
        return "end turn";//string freestate
    }

    public static void SendMove(string message)
    {
        Debug.Log("Sending move: " + message);
        // return;
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
            Debug.Log(ex.ToString());
        }
    }
    public static string GetMove()
    {
        try
        {
            using (TcpClient client = new TcpClient())
            {
                client.ReceiveTimeout = 50; // Optional: short timeout to avoid hangs
                client.Connect("127.0.0.1", 6969);

                NetworkStream stream = client.GetStream();
                if (stream.DataAvailable)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string move = reader.ReadLine();
                        Debug.Log("[Unity] Received move from Java: " + move);
                        return move ?? "NONE";
                    }
                }
                else
                {
                    return "NONE"; // No data available
                }
            }
        }
        catch (SocketException e)
        {
            Debug.LogWarning("[Unity] Socket error: " + e.Message);
            return "NONE";
        }
        catch (IOException e)
        {
            Debug.LogWarning("[Unity] IO error: " + e.Message);
            return "NONE";
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[Unity] Unexpected error: " + ex.Message);
            return "NONE";
        }
    }


    private static void ProcessMove(string msg)
    {
        // Exemplu de parsare: "SETTLEMET -1 1 2"
        Debug.Log("Processing move: " + msg);
        // Poți adăuga aici cod pentru a desena piese etc.
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

            client.Close();
            server.Stop();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
}

