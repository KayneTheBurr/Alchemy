using System.Collections;
using UnityEngine;

public class SelfDestructScript : MonoBehaviour
{
    public float destructTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(destructTime);
        Destroy(gameObject);
    }
}
