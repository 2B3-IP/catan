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
        
        public static string Description(this DevelopmentCardType cardType) =>
            cardType switch
            {
                DevelopmentCardType.Knight => "<color=red>Move the robber.</color>\n Steal <color=blue>one</color> resource from the owner of a settlement or city adjacent to the robber's new hex",
                DevelopmentCardType.RoadBuilding => "<color=red>Place </color><color=blue>two</color> <color=red>new roads </color>as if you had just built them",
                DevelopmentCardType.Monopoly => "<color=red>When you play this card, announce</color> <color=blue>one</color><color=red> type of resource.</color>\n All other players must give you all og their resources of that type.",
                DevelopmentCardType.YearOfPlenty => "<color=red>Take any </color><color=blue> two </color><color=red>resources from the bank</color>. Add them to your hand. They can be two of the same resource or two different resources.",
                DevelopmentCardType.VictoryPoint => " <color=blue>One</color> <color=red>additional Victory Point</color> is added to your total and doesn't need to be played to win",
                _ => throw new ArgumentOutOfRangeException(nameof(cardType), cardType, null)
            };
    }
}