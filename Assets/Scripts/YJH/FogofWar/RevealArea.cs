// RevealArea.cs
using UnityEngine;

public class RevealArea : MonoBehaviour
{
    public GameObject revealPrefab;
    GameObject revealInstance;

    void Start()
    {
        revealInstance = Instantiate(revealPrefab, transform.position, Quaternion.Euler(90, 0, 0));
        revealInstance.transform.SetParent(transform);
    }
}
