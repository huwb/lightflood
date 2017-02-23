using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float _translateSpeed = 2f;
    public float _rotateSpeed = 2f;

	void Start()
    {
	}
	
	void Update()
    {
        transform.position += Input.GetAxis( "Vertical" ) * transform.forward * _translateSpeed * Time.deltaTime;

        float strafe = 0f;
        if( Input.GetKey( KeyCode.A ) ) strafe -= 1f;
        if( Input.GetKey( KeyCode.D ) ) strafe += 1f;
        transform.position += strafe * transform.right * _translateSpeed * Time.deltaTime;

        float rotate = 0f;
        if( Input.GetKey( KeyCode.LeftArrow ) ) rotate -= 1f;
        if( Input.GetKey( KeyCode.RightArrow ) ) rotate += 1f;
        transform.eulerAngles += rotate * Vector3.up * _rotateSpeed * Time.deltaTime;

        float updown = 0f;
        if( Input.GetKey( KeyCode.Q ) ) updown -= 1f;
        if( Input.GetKey( KeyCode.E ) ) updown += 1f;
        transform.position += updown * transform.up * _translateSpeed * Time.deltaTime;
    }
}
