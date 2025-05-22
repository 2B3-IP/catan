using System;

namespace B3.DevelopmentCardSystem
{
    public enum DevelopmentCardType
    {
        Knight,
        RoadBuilding,
        Monopoly,
        YearOfPlenty,
        VictoryPoint
    }

    public static class DevelopmentCardTypeExt
    {
        public static string Name(this DevelopmentCardType cardType) =>
            cardType switch
            {
                DevelopmentCardType.Knight => "Knight",
                DevelopmentCardType.RoadBuilding => "Road Building",
                DevelopmentCardType.Monopoly => "Monopoly",
                DevelopmentCardType.YearOfPlenty => "Year of Plenty",
                DevelopmentCardType.VictoryPoint => "Victory Point",
                _ => throw new ArgumentOutOfRangeException(nameof(cardType), cardType, null)
            };
    }
}