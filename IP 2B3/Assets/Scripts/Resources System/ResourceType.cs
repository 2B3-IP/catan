using System;

namespace B3.ResourcesSystem
{
    public enum ResourceType
    {
        Wood  = 0,
        Brick = 1,
        Wheat = 2,
        Sheep = 3,
        Ore   = 4
    }

    static class ResourceTypeExtensions
    {
        public static string GetString(this ResourceType resourceType) =>
            resourceType switch
            {
                ResourceType.Wood  => "<color=#632506>Wood</color>",
                ResourceType.Brick => "<color=#8F1313>Brick</color>",
                ResourceType.Wheat => "<color=#FED44B>Wheat</color>",
                ResourceType.Sheep => "<color=#B2B2B2>Sheep</color>",
                ResourceType.Ore   => "<color=#52B5D7>Ore</color>",
                _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
            };
    }
}