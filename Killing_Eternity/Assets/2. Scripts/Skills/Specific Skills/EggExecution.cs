using System.Collections;
using UnityEngine;

public class EggExecution : MonoBehaviour
{

    public GameObject VFXObject;
    public GameObject eggShards;
    public GameObject spawnedEnemy;
    public float eggGestationTime;
    private float jiggleRange;
    public float jiggleCoefficient;
    public float jiggleMax;
    void Start()
    {
        StartCoroutine(EggHatch());
    }

    void Update()
    {
        transform.localEulerAngles = new Vector3(Random.Range(-jiggleRange,jiggleRange), Random.Range(-jiggleRange,jiggleRange), Random.Range(-jiggleRange,jiggleRange));
        if (jiggleRange < jiggleMax)
        {
            jiggleRange = jiggleRange + (jiggleCoefficient * Time.deltaTime);
        }
        else
        {
            jiggleRange = jiggleMax;
        }
    }

    IEnumerator EggHatch()
    {
        yield return new WaitForSeconds(eggGestationTime);
        Instantiate(VFXObject,transform.position,Quaternion.identity);
        Instantiate(eggShards,transform.position,Quaternion.identity);
        Instantiate(spawnedEnemy,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}
