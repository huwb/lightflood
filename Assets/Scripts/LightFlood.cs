using UnityEngine;

public class LightFlood : MonoBehaviour
{
    Texture2D _texture;
    MeshRenderer _mr;
    Color[] pixels;
    Color[] pixels2;

    Color _col = Color.white;
    public Color color {  get { return _col; } }

    int _texRes = 64;

    void Start()
    {
        float sz = Mathf.Lerp( transform.lossyScale.x, transform.lossyScale.y, 0.5f );

        _texRes = Mathf.FloorToInt( GIRend.Instance._resPerMeter * sz );
        
        _texture = new Texture2D( _texRes, _texRes, TextureFormat.RGBAHalf, false, true );
        _texture.wrapMode = TextureWrapMode.Clamp;

        // Reset all pixels color to transparent
        pixels = _texture.GetPixels();
        for( int i = 0; i < pixels.Length; i++ )
            pixels[i] = Color.black;
        pixels2 = new Color[pixels.Length];

        _texture.SetPixels( pixels );
        _texture.Apply();

        _mr = GetComponent<MeshRenderer>();
        _mr.material.mainTexture = _texture;
	}

    void Update()
    {
        if( !GIRend.Instance._render )
            return;

        float blurAmt = GIRend.Instance._blurAmt;

        for( int i = 0; i < pixels.Length; i++ )
        {
            float wt = 0f;

            pixels2[i] = pixels[i] * (1f - 4f * blurAmt); wt += 1f - 4f * blurAmt;

            int x = i % _texRes;
            if( x - 1 >= 0 ) { pixels2[i] += blurAmt * pixels[i - 1]; wt += blurAmt; }
            if( x + 1 < _texRes ) { pixels2[i] += blurAmt * pixels[i + 1]; wt += blurAmt; }

            if( i - _texRes >= 0 ) { pixels2[i] += blurAmt * pixels[i - _texRes]; wt += blurAmt; }
            if( i + _texRes < pixels.Length ) { pixels2[i] += blurAmt * pixels[i + _texRes]; wt += blurAmt; }

            pixels2[i] /= wt;
            pixels2[i] *= GIRend.Instance._alpha;
        }

        for( int i = 0; i < pixels.Length; i++ )
            pixels[i] = pixels2[i];

        _texture.SetPixels( pixels );

        //       //_texture.SetPixel( Random.Range( 0, _texture.width - 1 ), Random.Range( 0, _texture.height - 1 ),  );

        _texture.Apply( true, false );

        _col = _mr.material.GetColor( "_WallColour" );
        _mr.material.SetFloat( "_texRes", _texRes );
        _mr.material.SetFloat( "_exposure", GIRend.Instance._exposure );
        _mr.material.SetFloat( "_contrast", GIRend.Instance._contrast );
    }

    public void Hit( ref GIRend.Photon p, ref RaycastHit hit, out Color existingRadiance )
    {
        Vector3 local = transform.InverseTransformPoint( hit.point );

        Vector3 uv = local + 0.5f * Vector3.one;
        uv.z = 0f;

        int x = Mathf.FloorToInt( uv.x * _texture.width );
        int y = Mathf.FloorToInt( uv.y * _texture.height );
        int index = y * _texture.height + x;
        index = Mathf.Clamp( index, 0, pixels.Length - 1 );

        existingRadiance = pixels[index];

        pixels[index].r += p.intensity.x;
        pixels[index].g += p.intensity.y;
        pixels[index].b += p.intensity.z;

        pixels[index].a = 255;
    }
}
