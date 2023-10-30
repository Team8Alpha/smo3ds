using UnityEngine;

public class FogAffector : MonoBehaviour 
{
    [SerializeField]
    float radius = 0.5f;
    
    private void OnTriggerStay(Collider other)
    {
        Fog fog = other.GetComponentInParent<Fog>();

        if (fog != null)
        {
            fog.Impact(transform.position, radius);
        }
    }
}
