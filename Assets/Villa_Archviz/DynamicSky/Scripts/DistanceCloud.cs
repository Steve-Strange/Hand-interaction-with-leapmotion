// TODO: In Unity 5, the cloud may need to render inside the skybox shader for better skybox ambient light generation that will sync with cloud color.
// However it will cost more to generate skybox ambient, may not a good idea if the "Continuous Baking" is active.

// Note: Must check the "Read/Write Enable" from the Hemisphere_Mesh import setting.

using UnityEngine;
using System.Collections;

namespace uSky
{
	[ExecuteInEditMode]
	[AddComponentMenu("uSky/Distance Cloud (beta)")]
	[RequireComponent (typeof (uSkyManager),typeof (uSkyLight))]
	public class DistanceCloud : MonoBehaviour {

		uSkyManager m_uSM;
		uSkyLight m_uSL;

		public int cloudLayer = 18;

		const float NightBrightness = 0.25f; // override the brightness at night time

		public Material CloudMaterial;

		private Mesh skyDome;

		protected uSkyManager uSM {
			get{
				if (m_uSM == null) {
					m_uSM = this.gameObject.GetComponent<uSkyManager>();
					if (m_uSM == null)
						Debug.Log(" Can't not find uSkyManager Component, Please apply DistanceCloud in uSkyManager gameobject");
				}
				return m_uSM;
			}
		}
		protected uSkyLight uSL {
			get{
				if (m_uSL == null) {
					m_uSL = this.gameObject.GetComponent<uSkyLight>();
					if (m_uSL == null)
						Debug.Log("Can't not find uSkyLight Component, Please apply DistanceCloud in uSkyManager gameobject");
				}
				return m_uSL;
			}
		}

		protected Mesh InitSkyDomeMesh (){
			Mesh Hemisphere = Resources.Load<Mesh> ("Hemisphere_Mesh") as Mesh;
	//		Hemisphere.hideFlags = HideFlags.HideAndDontSave;
	//		skyDome = (Mesh) Instantiate ( Resources.Load<Mesh> ("Hemisphere_Mesh")); // error after build!
			if (Hemisphere == null) {
				Debug.Log ("Can't find Hemisphere_Mesh.fbx file.");
	//			skyDome = null;
				return null;
			} else {

				Mesh m = new Mesh ();
				Vector3[] verts = Hemisphere.vertices;
//				float scale = (Camera.main !=null)? Camera.main.farClipPlane - 10f :
//								(Camera.current != null)? Camera.current.farClipPlane : 990f ;
				int i = 0;
				while (i < verts.Length) {
//					verts[i] *=  scale;
					verts[i].y *=  0.85f; // scale in shader instead?
					i++;
				}
	//			m.vertices = Hemisphere.vertices;
				m.vertices = verts;
				m.triangles = Hemisphere.triangles;
				m.normals = Hemisphere.normals;
	//			m.tangents = Hemisphere.tangents; // no needed at the moment
				m.uv = Hemisphere.uv;
				m.uv2 = Hemisphere.uv2;
				// over size mesh bounds to avoid camera frustum culling for Vertex transformation in shader 
				m.bounds = new Bounds (Vector3.zero, Vector3.one * 2e9f); // less than 2,147,483,648
				m.hideFlags = HideFlags.DontSave; // prevent leak ?
				m.name = "skydomeMesh";

				return m;
			}
		}

		void OnEnable (){
			if (skyDome == null)
				skyDome = InitSkyDomeMesh ();
		}

		void OnDisable() {
			if (skyDome) 
				DestroyImmediate(skyDome);
		}

		// Use this for initialization
		void Start () {
	//		if (skyDome == null)
	//			InitSkyDomeMesh ();
			if (uSM != null && uSL != null)
					UpdateCloudMaterial (); 
		}
		
		// Update is called once per frame
		void Update (){
			if (uSM != null)
				if (uSM.SkyUpdate && uSL != null)
					UpdateCloudMaterial ();

			if (skyDome && CloudMaterial)
				Graphics.DrawMesh (skyDome, Vector3.zero, Quaternion.identity, CloudMaterial, cloudLayer );
		}

		void UpdateCloudMaterial () {
			 
			float	Brightness = Mathf.Max ( Mathf.Pow ( NightBrightness, uSM.LinearSpace ? 1.5f : 1f) , uSM.DayTime); 
					Brightness *= Mathf.Sqrt( uSM.Exposure); // sync with sky Exposure?

			if (CloudMaterial != null) {
	//			CloudMaterial.SetVector("ShadeColorFromSun", new Vector3(
	//			                        Mathf.Pow ( uSL.CurrentLightColor.r , ColorCorrection)* Brightness,
	//			                        Mathf.Pow ( uSL.CurrentLightColor.g , ColorCorrection)* Brightness,
	//			                        Mathf.Pow ( uSL.CurrentLightColor.b , ColorCorrection)* Brightness));
	//			CloudMaterial.SetVector("ShadeColorFromSky", new Vector3(
	//			                        Mathf.Pow ( uSL.CurrentSkyColor.r , ColorCorrection)* Brightness,
	//			                        Mathf.Pow ( uSL.CurrentSkyColor.g , ColorCorrection)* Brightness,
	//			                        Mathf.Pow ( uSL.CurrentSkyColor.b , ColorCorrection)* Brightness));

				// too much color saturation in linear?
				CloudMaterial.SetVector("ShadeColorFromSun",uSM.LinearSpace ? uSL.CurrentLightColor.linear * Brightness : uSL.CurrentLightColor * Brightness);
				CloudMaterial.SetVector("ShadeColorFromSky",uSM.LinearSpace ? uSL.CurrentSkyColor.linear * Brightness : uSL.CurrentSkyColor * Brightness);

	//			CloudMaterial.SetVector ("_SunDir", Vector3.Lerp(Vector3.up, uSM.SunDir, uSM.uMuS));
	//			CloudMaterial.SetVector("_colorCorrection", ColorCorrection);
			}
		}

	//	private float ColorCorrection {
	//		get {
	//			return 
	//				(!uSM.LinearSpace && uSM.Tonemapping) ? 1.5f :
	//				(uSM.LinearSpace && uSM.Tonemapping) ? 2.6f :
	//				uSM.LinearSpace ? 1.5f : 1f; 
	//			}
	//		}
		}
}
