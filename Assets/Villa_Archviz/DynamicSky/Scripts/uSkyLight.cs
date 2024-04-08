using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

namespace uSky
{
	[ExecuteInEditMode]
	[AddComponentMenu("uSky/uSky Light")]
	[RequireComponent (typeof (uSkyManager))]
	public class uSkyLight : MonoBehaviour {	

		[Range(0f,4f)][Tooltip ("Brightness of the Sun (directional light)")]
		public float SunIntensity = 1.0f;

		[Tooltip ("The color of the both Sun and Moon light emitted")]
		public Gradient LightColor = new Gradient()
		{
			colorKeys = new GradientColorKey[] {
				new GradientColorKey(new Color32(055, 066, 077, 255), 0.23f),
				new GradientColorKey(new Color32(245, 173, 084, 255), 0.26f),
				new GradientColorKey(new Color32(249, 208, 144, 255), 0.32f),
				new GradientColorKey(new Color32(252, 222, 186, 255), 0.50f),
				new GradientColorKey(new Color32(249, 208, 144, 255), 0.68f),
				new GradientColorKey(new Color32(245, 173, 084, 255), 0.74f),
				new GradientColorKey(new Color32(055, 066, 077, 255), 0.77f),
			},
			alphaKeys = new GradientAlphaKey[] {
				new GradientAlphaKey(1.0f, 0.0f),
				new GradientAlphaKey(1.0f, 1.0f)
			}
		};
		[Tooltip ("Toggle the Moon lighting during night time")]
		public bool EnableMoonLighting = true;

		[Range(0f,2f)][Tooltip ("Brightness of the Moon (directional light)")]
		public float MoonIntensity = 0.4f;

		[Tooltip ("Ambient light that shines into the scene.")]
		public uSkyAmbient Ambient;
		
		private float currentTime	{ get { return (uSM != null)? uSM.Timeline01	: 1f; }}
		private float dayTime		{ get { return (uSM != null)? uSM.DayTime		: 1f; }} 
		private float nightTime		{ get { return (uSM != null)? uSM.NightTime		: 0f; }}
		private float sunsetTime 	{ get { return (uSM != null)? uSM.SunsetTime	: 1f; }} 


		private uSkyManager _uSM = null;
		private uSkyManager uSM {
			get{
				if (_uSM == null) {
					_uSM = this.gameObject.GetComponent<uSkyManager>();
					#if UNITY_EDITOR
					if (_uSM == null)
						Debug.Log("Can't not find uSkyManager, please apply uSkyLight to uSkyManager gameobject");
					#endif
				}
				return _uSM;
			}
		}
		// check sun light gameobject
		private GameObject sunLightObj{
			get{
				if(uSM != null){
					return (uSM.m_sunLight != null)? uSM.m_sunLight : null;
				}else 
					return null;
			}
		}
		// check moon light gameobject
		private GameObject moonLightObj{
			get{
				if(uSM != null){
					return (uSM.m_moonLight != null)? uSM.m_moonLight : null;
				}else 
					return null;
			}
		}

		private Light _sun_Light;
		private Light sun_Light {
			get {
				if (sunLightObj)
					_sun_Light = sunLightObj.GetComponent<Light> ();
				if (_sun_Light)
					return _sun_Light;
				else return null;
			}
		}

		private Light _moon_Light;
		private Light moon_Light {
			get {
				if (moonLightObj)
					_moon_Light = moonLightObj.GetComponent<Light> ();
				if (_moon_Light)
					return _moon_Light;
				else return null;
			}
		}

		void Start () {
			if (uSM != null)
				InitUpdate ();
		}
		
		void Update (){
			if (uSM != null) {
				if (uSM.SkyUpdate)
					InitUpdate ();
			}
		}
		
		void InitUpdate (){
			SunAndMoonLightUpdate ();

			if (RenderSettings.ambientMode == AmbientMode.Trilight)
				AmbientGradientUpdate ();
			else
//				if (RenderSettings.ambientMode == AmbientMode.Flat)
				RenderSettings.ambientLight = CurrentSkyColor; // update it for cloud color
		}
		
		void SunAndMoonLightUpdate (){

			if (sunLightObj != null) {
				if (sun_Light != null) {
					sun_Light.intensity = uSM.Exposure * SunIntensity * dayTime;
					sun_Light.color = CurrentLightColor * dayTime;
					// enable at Day, disable at Night
					sun_Light.enabled = (currentTime < 0.24f || currentTime > 0.76f) ? false : true;
				}
			}
			if (moonLightObj != null) {
				if (moon_Light != null) {
					moon_Light.intensity = uSM.Exposure * MoonIntensity * nightTime;
					moon_Light.color = CurrentLightColor * nightTime;
					// enable at Night, disable at Day
					moon_Light.enabled = (currentTime > 0.26f && currentTime < 0.74f || EnableMoonLighting == false) ?
							 false : (EnableMoonLighting) ? true : false;
					if(!EnableMoonLighting)
						forceSunEnableAtNight ();
				}
			}
			else 
				forceSunEnableAtNight ();
			
		}
		// Enable the sun if no moon light in the scene, uSky need at least one active light,
		// Other wise the moon texture in the night sky will be missing.
		private void forceSunEnableAtNight (){
			if (!sun_Light) return;
			sun_Light.enabled = true;
			sun_Light.intensity = Mathf.Max (1e-3f, sun_Light.intensity);
			sun_Light.color = new Color(sun_Light.color.r, sun_Light.color.g, sun_Light.color.b,
			                            Mathf.Max (1e-2f,sun_Light.color.a ));
		}
		private float exposure {
			get{ return (uSM != null)? uSM.Exposure : 1.0f; }
		}

//		private Color groundColorTint {
//			get{ return (uSM != null)? uSM.m_GroundColor : new Color(0.369f, 0.349f, 0.341f, 1f); }
//		}

		private float rayleighSlider {
			get{ return (uSM != null)? uSM.RayleighScattering : 1.0f; }
		}

		public Color CurrentLightColor {
			get{ return LightColor.Evaluate (currentTime); }
		}

		void AmbientGradientUpdate ()
		{
			RenderSettings.ambientSkyColor = CurrentSkyColor;
			RenderSettings.ambientEquatorColor = CurrentEquatorColor;
			RenderSettings.ambientGroundColor = CurrentGroundColor;
		}

		public Color CurrentSkyColor {
			get{ return colorOffset (Ambient.SkyColor.Evaluate (currentTime), 0.15f, 0.7f, false); }
		}

		public Color CurrentEquatorColor {
			get{ return colorOffset (Ambient.EquatorColor.Evaluate (currentTime), 0.15f, 0.9f, false); }
		}

		public Color CurrentGroundColor {
			get{
				return  colorOffset (Ambient.GroundColor.Evaluate (currentTime), 0.25f, 0.85f, true);
//				return	(Ambient.GroundColor.Evaluate (currentTime)* exposure); 
			}
		}
 
		// get the approximation of color change from uSkymanager and then apply to final output of the gradient colors
		private Color colorOffset ( Color currentColor, float offsetRange, float rayleighOffsetRange, bool IsGround ){
			// default uSky βsR = (5.81, 13.57, 33.13) in kilo unit
			Vector3 _betaR = (uSM != null)? uSM.BetaR * 1e3f : new Vector3 (5.81f, 13.57f, 33.13f);
			// BlendWeight
			Vector3 t = new Vector3 (0.5f, 0.5f, 0.5f); // neutral 

//			if (IsGround)
//				t = new Vector3 (_betaR.x /  5.81f * (groundColorTint.r / 0.369f * 0.5f),
//				                 _betaR.y / 13.57f * (groundColorTint.g / 0.349f * 0.5f),
//				                 _betaR.z / 33.13f * (groundColorTint.b / 0.341f * 0.5f));
//			else
				t = new Vector3 (_betaR.x /  5.81f * 0.5f ,
			                     _betaR.y / 13.57f * 0.5f ,
			                     _betaR.z / 33.13f * 0.5f );

			// switch BlendWeight to oppsite value when sunset time is present
			if(!IsGround)
			t = Vector3.Lerp (new Vector3 ( Mathf.Abs(1 - t.x), Mathf.Abs( 1 - t.y), Mathf.Abs( 1 - t.z)), t, sunsetTime);

			// set BlendWeight to neutral when night time is present
			t = Vector3.Lerp (new Vector3 ( 0.5f, 0.5f, 0.5f), t, dayTime);

			Vector3 c0 = new Vector3();
			// add betaR offset based on "t" to interpolates
//			if(!IsGround)
			c0 = new Vector3 (Mathf.Lerp (currentColor.r - offsetRange, currentColor.r + offsetRange, t.x),
			                          Mathf.Lerp (currentColor.g - offsetRange, currentColor.g + offsetRange, t.y),
			                          Mathf.Lerp (currentColor.b - offsetRange, currentColor.b + offsetRange, t.z));

			// add rayleigh offset from "rayleighSlider"
			Vector3 c2 = new Vector3 (c0.x / _betaR.x, c0.y / _betaR.y, c0.z / _betaR.z) * 4f ;

			c0 = (rayleighSlider < 1.0f) ? Vector3.Lerp (Vector3.zero, c0, rayleighSlider) :
				Vector3.Lerp (c0, c2 , Mathf.Max(0f,(rayleighSlider - 1f)) / 4f * rayleighOffsetRange);

			return new Color(c0.x,c0.y,c0.z,1) * exposure;
		}

	}
}
