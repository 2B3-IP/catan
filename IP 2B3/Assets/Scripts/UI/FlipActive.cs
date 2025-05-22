using UnityEngine;
using UnityEngine.Serialization;

public class FlipActive : MonoBehaviour
{
    public GameObject go; 
    public void WhenButtonClicked()
    {
        if (go.activeSelf)
            go.SetActive(false);
        else
            go.SetActive(true);
    }
}