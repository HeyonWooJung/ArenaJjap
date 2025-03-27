using UnityEngine;

public class RevealArea : MonoBehaviour
{
    public GameObject revealPrefab;
    GameObject revealInstance;

    void Start()
    {
        revealInstance = Instantiate(revealPrefab, gameObject.transform);
        revealInstance.transform.SetParent(transform);
    }
}