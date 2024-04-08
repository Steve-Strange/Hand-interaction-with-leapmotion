using UnityEngine;
//using System;
using uSky;

[ExecuteInEditMode]
[AddComponentMenu("uSky/uSky Manager")]
public class uSkyManager : MonoBehaviour 
{
	[Tooltip ("Update of the sky calculations in each frame.")]
	public bool SkyUpdate = true; // TODO: Update mode : Off, All_Settings, Timeline_Only
//	public bool useSlider = true;
	[Range(0.0f, 24.0f)][Tooltip ("This value controls the light vertically. It represents sunrise/day and sunset/night time( Rotation X )")]
	public float Timeline = 17.0f;

	[Range(-180.0f, 180.0f)][Tooltip ("This value controls the light horizionally.( Rotation Y )")]
	public float Longitude = 0.0f;

	[Space(10)][Tooltip ("This value sets the brightness of the sky.(for day time only)")]
	[Range(0.0f, 5.0f)]
	public float Exposure = 1.0f;

	[Range(0.0f, 5.0f)][Tooltip ("Rayleigh scattering is caused by particles in the atmosphere (up to 8 km). It produces typical earth-like sky colors (reddish/yellowish colors at sun set, and the like).")]
	public float RayleighScattering = 1.0f;

	[Range(0.0f, 5.0f)][Tooltip ("Mie scattering is caused by aerosols in the lower atmosphere (up to 1.2 km). It is for haze and halos around the sun on foggy days.")]
	public float MieScattering = 1.0f;

	[Range (0.0f,0.9995f)][Tooltip ("The anisotropy factor controls the sun's appearance in the sky.The closer this value gets to 1.0, the sharper and smaller the sun spot will be. Higher values cause more fuzzy and bigger sun spots.")]
	public float SunAnisotropyFactor = 0.76f;

	[Range (1e-3f,10.0f)][Tooltip ("Size of the sun spot in the sky")]
	public float SunSize = 1.0f;

	[Tooltip ("It is visible spectrum light waves. Tweaking these values will shift the colors of the resulting gradients and produce different kinds of atmospheres.")]
	// Wavelengths for visible light ray from 380 to 780 
	public Vector3 Wavelengths = new Vector3(680f, 550f, 440f); // sea level mie

	[Tooltip ("It is wavelength dependent. Tweaking these values will shift the colors of sky color.")]
	public Color SkyTint = new Color(0.5f, 0.5f, 0.5f, 1f);

	[Tooltip ("It is the bottom half color of the skybox")]
	public Color m_GroundColor = new Color(0.369f, 0.349f, 0.341f, 1f);

//	[NullDetectAttribute("something...","Please apply a Directioal Light here!")]
//	[NullDetectAttribute(m_sunLight,"Please apply a Directioal Light here!")]
	[Tooltip ("It is a Directional Light from the scene, it represents Sun Ligthing")]
	public GameObject m_sunLight = null;
	public GameObject m_sunUI = null;



	public enum NightModes
	{
		Off = 1,
		Static = 2,
		Rotation = 3
	}
	[Space (10)]
	public NightModes NightSky = NightModes.Static;

//	[Tooltip ("Toggle the Night Sky On and Off")]
	private bool EnableNightSky {
		get {
			return (NightSky == NightModes.Off)? false : true ;
		}
	}

	[Tooltip ("The zenith color of the night sky gradient. (Top of the night sky)")]
	public Gradient NightZenithColor = new Gradient()
	{
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(050, 071, 099, 255), 0.225f),
			new GradientColorKey(new Color32(074, 107, 148, 255), 0.25f),
			new GradientColorKey(new Color32(074, 107, 148, 255), 0.75f),
			new GradientColorKey(new Color32(050, 071, 099, 255), 0.775f),
		},
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(1.0f, 1.0f)
		}
	};
	[Tooltip ("The horizon color of the night sky gradient.")]
	public Color NightHorizonColor = new Color(0.43f,0.47f,0.5f,1f);

	[Range(0.0f, 5.0f)][Tooltip ("This controls the intensity of the Stars field in night sky.")]
	public float StarIntensity = 1.0f;

	[Range(0.0f, 2.0f)][Tooltip ("This controls the intensity of the Outer Space Cubemap in night sky.")]
	public float OuterSpaceIntensity = 0.25f;

	[Tooltip ("The color of the moon's inner corona. This Alpha value controls the size and blurriness corona.")]
	public Color MoonInnerCorona = new Color(1f, 1f, 1f, 0.5f);

	[Tooltip ("The color of the moon's outer corona. This Alpha value controls the size and blurriness corona.")]
	public Color MoonOuterCorona = new Color(0.25f,0.39f,0.5f,0.5f);

	[Range(0.0f, 1.0f)][Tooltip ("This controls the moon texture size in the night sky.")]
	public float MoonSize = 0.15f;

	[Tooltip ("It is additional Directional Light from the scene, it represents Moon Ligthing.")]
	public GameObject m_moonLight;

	[Tooltip ("It is the uSkybox Material of the uSky.")]
	public Material SkyboxMaterial;

	[SerializeField][Tooltip ("It will automatically assign the current skybox material to Render Settings.")]
	private bool _AutoApplySkybox = true;
	public bool AutoApplySkybox {
		get{ return _AutoApplySkybox; }
		set{
			if (value && SkyboxMaterial){
				if (RenderSettings.skybox != SkyboxMaterial) 
					RenderSettings.skybox = SkyboxMaterial;
			} 
			_AutoApplySkybox = value;
		}
	}

	[HideInInspector]
	public bool LinearSpace; //  Auto Detection

	[Tooltip ("Toggle it if the Main Camera is using HDR mode and Tonemapping image effect.")]
	public bool Tonemapping = false; // TODO : Auto Detect Main Camera?

	private Vector3 euler;
	private Vector3 angle;
	private Matrix4x4 moon_wtl;

	// NOTE: "Stars.Shader" need to be placed in Resources folder for mobile build!
	private Material m_starMaterial;
	protected Material starMaterial {
		get {
			if (m_starMaterial == null) {
				m_starMaterial = new Material(Shader.Find("Hidden/uSky/Stars"));
				m_starMaterial.hideFlags = HideFlags.DontSave;
			}
			return m_starMaterial;
		} 
	}

	private Mesh _starsMesh = null;
	public Mesh starsMesh {
		get{
			StarField Stars = new StarField();
			if (_starsMesh == null )
				_starsMesh = Stars.InitializeStarfield();
			return _starsMesh;
		}
	}
	
	void OnEnable() {
		if (m_sunLight == null)
			m_sunLight = GameObject.Find ("Directional Light");
		#if UNITY_EDITOR
		if (m_sunLight == null)
			Debug.Log("Please apply the <b>Directional Light</b> to uSkyManager");
//		if (SkyboxMaterial == null)
//			Debug.Log("Please apply the <b>Skybox Material</b> to uSkyManager");
		#endif			
//		if (EnableNightSky && starsMesh == null) {
//			InitStarsMesh ();
//		}
		#if UNITY_EDITOR
			detectColorSpace ();
		#else
			InitMaterial (SkyboxMaterial);
		#endif
	}
	
	void OnDisable() {
		if (starsMesh)		DestroyImmediate(starsMesh);
		if (m_starMaterial)	DestroyImmediate(m_starMaterial);
	}

	private void detectColorSpace (){
//			LinearSpace = QualitySettings.activeColorSpace == ColorSpace.Linear;
		#if UNITY_EDITOR
			LinearSpace = UnityEditor.PlayerSettings.colorSpace == ColorSpace.Linear;		// Editor
		#endif
		#if UNITY_IPHONE || UNITY_ANDROID
			LinearSpace = false; // Gamma only on mobile
		#endif
		if( SkyboxMaterial != null )
			InitMaterial (SkyboxMaterial);
	}

	private void Start() 
	{
//		if(useSlider)
			InitSun();
		InitMoon ();
		if( SkyboxMaterial != null )
			InitMaterial (SkyboxMaterial);

		AutoApplySkybox = _AutoApplySkybox;
//		if (EnableNightSky && starsMesh == null) {
//			InitStarsMesh ();
//		}
	}

	public float Timeline01 { get{ return Timeline / 24; }}

	void Update()
	{
		if (SkyUpdate) {
			// reset Timeline slider
			if (Timeline >= 24.0f)
				Timeline = 0.0f;

			if (Timeline < 0.0f)
				Timeline = 24.0f;

			// Update every frame for all shader Paramaters
			if (SkyboxMaterial != null) {
//				if(useSlider)
				InitSun ();
				InitMoon();
				InitMaterial (SkyboxMaterial);
			}
		}

		#if UNITY_EDITOR
			AutoApplySkybox = _AutoApplySkybox;
			detectColorSpace ();  
		#endif

		// Draw Star field
		if (EnableNightSky && starsMesh != null && starMaterial != null && SunDir.y < 0.2f)
			Graphics.DrawMesh (starsMesh, Vector3.zero, Quaternion.identity, starMaterial, 0 );
	}

	// rotate and align the sun direction with Timeline slider
	void InitSun()
	{
		euler.x = Timeline * 360.0f / 24.0f - 90.0f;
		euler.y = Longitude;
		euler.z = 0;
		if (m_sunLight != null)
			m_sunLight.transform.localEulerAngles = euler;
		
	
		angle.x = 0;
		angle.y = 0;
		angle.z = Timeline * 360.0f / 24.0f - 90.0f;
		if (m_sunUI != null)
			m_sunUI.transform.localEulerAngles = angle;

	}

	public void InitMaterial(Material mat)
	{
		mat.SetVector ("_SunDir", SunDir); 
		mat.SetMatrix ("_Moon_wtl", getMoonMatrix);
		
		mat.SetVector ("_betaR", betaR_RayleighOffset);
		mat.SetVector ("_betaM", BetaM);

		// x = Sunset, y = Day, z = Night 
		mat.SetVector ("_SkyMultiplier", skyMultiplier);

		mat.SetFloat ("_SunSize", 32.0f / SunSize);
		mat.SetVector ("_mieConst", mieConst);
		mat.SetVector ("_miePhase_g", miePhase_g);
		mat.SetVector ("_GroundColor", bottomTint);

		mat.SetVector ("_NightHorizonColor", getNightHorizonColor);
		mat.SetVector ("_NightZenithColor", getNightZenithColor);
		mat.SetVector ("_MoonInnerCorona", getMoonInnerCorona);
		mat.SetVector ("_MoonOuterCorona", getMoonOuterCorona); 
		mat.SetFloat ("_MoonSize", MoonSize);
		mat.SetVector ("_colorCorrection", ColorCorrection);


		if (Tonemapping) {
			mat.DisableKeyword ("USKY_HDR_OFF");
			mat.EnableKeyword ("USKY_HDR_ON");
		} else {
			mat.EnableKeyword ("USKY_HDR_OFF");
			mat.DisableKeyword ("USKY_HDR_ON");
		}
		if (EnableNightSky)
			mat.DisableKeyword("NIGHTSKY_OFF");
		else
			mat.EnableKeyword("NIGHTSKY_OFF");

		if ( NightSky == NightModes.Rotation)
			mat.SetMatrix ("rotationMatrix", m_sunLight.transform.worldToLocalMatrix);
		else
			mat.SetMatrix ("rotationMatrix", Matrix4x4.identity);

		mat.SetFloat ("_OuterSpaceIntensity", OuterSpaceIntensity);
		if (starMaterial != null) {
			starMaterial.SetFloat ("StarIntensity", starBrightness);
			if ( NightSky == NightModes.Rotation)
				starMaterial.SetMatrix ("rotationMatrix", m_sunLight.transform.localToWorldMatrix );
			else
				starMaterial.SetMatrix ("rotationMatrix", Matrix4x4.identity );
		}

	}

	public Vector3 SunDir {
		get { return (m_sunLight != null)? m_sunLight.transform.forward * -1: new Vector3(0.321f,0.766f,-0.557f);}
	}

	private Matrix4x4 getMoonMatrix {
		get {
			if (m_moonLight == null) {
					// predefined Moon Direction
					moon_wtl = Matrix4x4.TRS (Vector3.zero, new Quaternion (-0.9238795f, 8.817204e-08f, 8.817204e-08f, 0.3826835f), Vector3.one);
			} else if (m_moonLight != null) {
					moon_wtl = m_moonLight.transform.worldToLocalMatrix;
					moon_wtl.SetColumn (2, Vector4.Scale (new Vector4 (1, 1, 1, -1), moon_wtl.GetColumn (2)));
			}

//			if ( NightSky == NightModes.Rotation ){
//				Quaternion rotation = m_moonLight.transform.rotation + m_sunLight.transform.rotation;
//				moon_wtl = Matrix4x4.TRS (m_moonLight.transform.position,rotation,m_moonLight.transform.localScale);
//			}
			return moon_wtl;
		}
	}
	void InitMoon()
	{
		if (NightSky == NightModes.Rotation)
//			m_moonLight.transform.eulerAngles +=  m_sunLight.transform.localEulerAngles;
//			m_moonLight.transform.SetParent (m_sunLight.transform, false);
			m_moonLight.transform.forward = m_sunLight.transform.forward * -1;
//		else
//			m_moonLight.transform.forward = m_moonLight.transform.forward;

	}
//	public Vector3 MoonDir {
//		get {
//			return getMoonMatrix.GetColumn(2);
//		}
//	}
	
	private Vector3 variableRangeWavelengths {
		get { 
			return new Vector3 (Mathf.Lerp ( Wavelengths.x + 150, Wavelengths.x - 150, SkyTint.r ),
			                    Mathf.Lerp ( Wavelengths.y + 150, Wavelengths.y - 150, SkyTint.g ),
			                    Mathf.Lerp ( Wavelengths.z + 150, Wavelengths.z - 150, SkyTint.b ));
		}
	}

	public Vector3 BetaR {
		get {
			// Evaluate Beta Rayleigh function is based on A.J.Preetham

			Vector3 WL = variableRangeWavelengths * 1e-9f;

			const float Km = 1000.0f;
			const float n = 1.0003f;		// the index of refraction of air
			const float N = 2.545e25f;		// molecular density at sea level
			const float pn = 0.035f;		// depolatization factor for standard air

			Vector3 waveLength4 = new Vector3 (Mathf.Pow (WL.x, 4), Mathf.Pow (WL.y, 4), Mathf.Pow (WL.z, 4));
			Vector3 theta = 3.0f * N * waveLength4 * (6.0f - 7.0f * pn);
			float ray = (8 * Mathf.Pow (Mathf.PI, 3) * Mathf.Pow (n * n - 1.0f, 2) * (6.0f + 3.0f * pn));
			return Km * new Vector3 (ray / theta.x, ray / theta.y, ray / theta.z) ;
		}
	}
	private Vector3 betaR_RayleighOffset{
		get{ return  BetaR * Mathf.Max (1e-3f, RayleighScattering); }
	}

	// Mie extinction : Constant value is based on Eric Bruneton
	private readonly Vector3 BetaM = new Vector3 (4e-3f,4e-3f,4e-3f) * 0.9f; // cheaper

//	public Vector3 BetaM{
//		get {
			// Beta Mie (simplified) function is based on Cryengine
//			return new Vector3 (Mathf.Pow (Wavelengths.x, -0.84f), Mathf.Pow (Wavelengths.y, -0.84f), Mathf.Pow (Wavelengths.z, -0.84f));
//		}
//	}

	// 0 ~ 2.0 // Sun fall ratio function is based on Eric Bruneton 
	public float uMuS { get { return Mathf.Atan (Mathf.Max (SunDir.y, -0.1975f) * 5.35f) / 1.1f + 0.739f;}}

	// 0 ~ 1.0
	public float DayTime { get { return Mathf.Clamp01 (uMuS); }}

	public float SunsetTime {
		get { return Mathf.Clamp01 ((uMuS - 1.0f) * (1.5f / Mathf.Pow (RayleighScattering, 4f))); }
	}

	public float NightTime { get { return 1 - DayTime; }}
	
	public Vector3 miePhase_g {
		get{
			// partial mie phase : approximated with the Cornette Shanks phase function
			float g2 = SunAnisotropyFactor * SunAnisotropyFactor;
			float cs = LinearSpace && Tonemapping? 2f : 1f;
			return new Vector3 ( cs * ((1.0f - g2) / (2.0f + g2)), 1.0f + g2, 2.0f * SunAnisotropyFactor);
		}
	}
	public Vector3 mieConst {
		get { return new Vector3 (1f, BetaR.x/ BetaR.y, BetaR.x/ BetaR.z) * BetaM.x * MieScattering;}
	}

	// x = Sunset, y = Day, z = Night
	public Vector3 skyMultiplier {
		get{ return new Vector3 ( SunsetTime, Exposure * 4 * DayTime * Mathf.Sqrt(RayleighScattering), NightTime) ;}
	}

	private Vector3 bottomTint{
		get {
			float cs = LinearSpace ? 1e-2f : 2e-2f;
			return new Vector3 (betaR_RayleighOffset.x / (m_GroundColor.r * cs ),
			                    betaR_RayleighOffset.y / (m_GroundColor.g * cs ),
			                    betaR_RayleighOffset.z / (m_GroundColor.b * cs ));
//			float cs = LinearSpace ? 1f : 2f;
//			return new Vector3 ((m_GroundColor.r * cs ),
//			                    (m_GroundColor.g * cs ),
//			                    (m_GroundColor.b * cs ));

		}
	}

	public Vector2 ColorCorrection {
		get{
			return (LinearSpace && Tonemapping) ? new Vector2 (0.38317f, 1.413f): // (0.5f, 1.5f) :
				// using 2.0 instead of 2.2
				LinearSpace ? new Vector2 (1f, 2.0f) : Vector2.one; 
		}
	}

	public Color getNightHorizonColor{ get{ return NightHorizonColor * NightTime; }}

	public Color getNightZenithColor{ get{ return NightZenithColor.Evaluate(Timeline01) * 1e-2f; }}

	private float moonCoronaFactor {
		get{
			float m = 0.0f;
			float dir = m_sunLight.transform.forward.y;
			if (NightSky == NightModes.Rotation)
				m = NightTime * dir;
			else
				m = NightTime;
			return m;
		}
	}

	private Vector4 getMoonInnerCorona {
		get {
			return new Vector4 (MoonInnerCorona.r * moonCoronaFactor,
			                    MoonInnerCorona.g * moonCoronaFactor,
			                    MoonInnerCorona.b * moonCoronaFactor,
			                    4e2f / MoonInnerCorona.a);
		}
	}

	private Vector4 getMoonOuterCorona {
		get {
			float cs = LinearSpace?  Tonemapping ? 16f : 12f: 8f;
			return new Vector4 (MoonOuterCorona.r * 0.25f * moonCoronaFactor,
			                    MoonOuterCorona.g * 0.25f * moonCoronaFactor,
			                    MoonOuterCorona.b * 0.25f * moonCoronaFactor,
			                    cs / MoonOuterCorona.a); 
		}
	}

	// Stars shader setting
	private float starBrightness {
		get {
			float cs = LinearSpace ? 1f : 1.5f;
			return StarIntensity * NightTime * cs;
		}
	}
	
}

