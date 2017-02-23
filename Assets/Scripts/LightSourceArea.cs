using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightSourceArea : LightSource
{
    [Range( 0f, 90f )]
    public float _coneAngleDeg = 90f;

    public Texture2D _texture;

    public override void MakePhoton( ref GIRend.Photon p )
    {
        p.position = transform.TransformPoint( new Vector3( Random.value - 0.5f, Random.value - 0.5f, 0f ) );

        // construct a random direction within cone angle
        float azimuth = Random.value * Mathf.PI * 2f;
        float altitude = Random.value * _coneAngleDeg * Mathf.Deg2Rad;

        Vector3 local;
        local.x = Mathf.Cos( azimuth ) * Mathf.Sin( altitude );
        local.y = Mathf.Sin( azimuth ) * Mathf.Sin( altitude );
        local.z = Mathf.Sqrt( 1f - local.x * local.x - local.y * local.y );

        p.next_dir = local.x * transform.right + local.y * transform.up - local.z * transform.forward;

        Color col;
        if( _texture != null )
        {
            Vector3 localPos = transform.InverseTransformPoint( p.position );

            Vector3 uv = localPos + 0.5f * Vector3.one;
            uv.z = 0f;

            col = intensity * _texture.GetPixelBilinear( uv.x, uv.y );
        }
        else
        {
            col = intensity * _light.color;
        }

        p.intensity = new Vector3( col.r, col.g, col.b );
    }

    void Update()
    {
        _surfaceArea = transform.lossyScale.x * transform.lossyScale.y;

        if( _texture != null )
        {
            GetComponent<MeshRenderer>().material.mainTexture = _texture;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = _light.color * intensity * 10f;
        }
    }
}
