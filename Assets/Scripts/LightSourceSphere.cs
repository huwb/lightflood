using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightSourceSphere : LightSource
{
    public float _deathPlane = -15f;

    float _radius = 1f;

    public override void MakePhoton( ref GIRend.Photon p )
    {
        Vector3 off = Random.onUnitSphere;

        p.position = transform.position + _radius * off;

        p.next_dir = Random.onUnitSphere;
        if( Vector3.Dot( p.next_dir, off ) < 0f )
            p.next_dir = -p.next_dir;

        Color col = intensity * _light.color;

        p.intensity = new Vector3( col.r, col.g, col.b );
    }

    void Update()
    {
        _radius = 0.5f * (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3f;
        _surfaceArea = Mathf.PI * _radius * _radius;
        GetComponent<MeshRenderer>().material.color = _light.color * intensity * 10f;

        if( transform.position.y < _deathPlane )
            Destroy( gameObject );
    }
}
