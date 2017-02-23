using UnityEngine;
using System.Collections;

public class ShootLights : MonoBehaviour
{
    public float _speedZ = 3f;
    public float _speedY = 3f;

    public GameObject _projectilePrefab;

	void Start ()
    {
	}
	
	void Update ()
    {
        if( Input.GetKeyDown( KeyCode.Space ) )
        {
            GameObject proj = Instantiate( _projectilePrefab, transform.position + Vector3.down, Quaternion.identity ) as GameObject;
            proj.GetComponent<Rigidbody>().velocity = _speedZ * transform.forward + _speedY * transform.up;
        }
	}
}
