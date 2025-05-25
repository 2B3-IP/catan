using UnityEngine;

public class TradeType : MonoBehaviour
{
    public enum type
    {
        wood,
        brick,
        wheat,
        sheep,
        ore,
        any
    }

    public int MyResourceNumber;
    public int TheirResourceNumber;
    public type MyType;
    public type TheirType;
}
