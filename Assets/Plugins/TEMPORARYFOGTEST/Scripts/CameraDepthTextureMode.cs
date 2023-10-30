using UnityEngine;

public class CameraDepthTextureMode : MonoBehaviour 
{
    [SerializeField]
    DepthTextureMode depthTextureMode;

	private void OnEnable() 
	{
        GetComponent<Camera>().depthTextureMode = depthTextureMode;
    }
}
