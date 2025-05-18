using B3.BoardSystem;
using B3.ResourcesSystem;

public static class AI 
{
    public static void PassPiecesResources(ResourceType?[] resources)
    {

    }

    public static void PassPiecesNumber(int[] numbers)
    {

    }

    public static void PassPortsResources(ResourceType?[] ports)
    {

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

    public static void SendMove(string message)
    {

    }
}
