using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace uSky
{
	public class StarField 
	{
		private float starSizeScale = 1.0f; // if modify this value that will update after click PLAY button in Editor

		private List<CombineInstance> starQuad = new List<CombineInstance>();
		
		private struct Star
		{
			public Vector3 position;
			public Color color;
		}

		public Mesh InitializeStarfield()
		{
			float starDistance = (Camera.main !=null)? Camera.main.farClipPlane - 10f :
									(Camera.current != null)? Camera.current.farClipPlane : 990f ;
			float starSize = starDistance / 100 * starSizeScale;

			// Load star positions and colors from file with 9110 predefined stars.
			TextAsset data =  Resources.Load<TextAsset>("StarsData");
			if (data == null){
				Debug.Log("Can't find or read StarsData.bytes file.");
				return null;
			}

			const int numberOfStars = 9110;
			var stars = new Star[numberOfStars];

			using (BinaryReader reader = new BinaryReader(new MemoryStream(data.bytes)))
			{
				for (int i = 0; i < numberOfStars; i++)
				{
					stars[i].position.x = reader.ReadSingle();
					stars[i].position.z = reader.ReadSingle();
					stars[i].position.y = reader.ReadSingle(); // Z-up to Y-up

					stars[i].position = Vector3.Scale (stars[i].position,new Vector3(-1f,1f,-1f));

					stars[i].color.r = reader.ReadSingle();
					stars[i].color.g = reader.ReadSingle();
					stars[i].color.b = reader.ReadSingle();

					// Using luminance term to sort the brightness for magnitude
					float magnitude = Vector3.Dot(new Vector3(stars[i].color.r,stars[i].color.g,stars[i].color.b), new Vector3(0.22f,0.707f,0.071f));
					
					stars[i].color.a = magnitude ;

					// Note: To improve performance, we could sort stars by brightness and remove less important stars.
					// Generate static stars for upper hemi sky dome only and 1023 predefined stars.
//					if (stars[i].position.y < 0.1f || stars[i].color.a < 1.7037e-2f)
//					{
//						continue;
//					}

					// 2.2 is for avoid an one big yellow star
					if(stars[i].color.a < 1.62e-2f || stars[i].color.a > 2.2f) continue; // 2460 predefined stars.

						CombineInstance ci = new CombineInstance();
						ci.mesh = createQuad(starSize);

						ci.transform = BillboardMatrix(stars[i].position * starDistance);

						Color[] colors = {stars[i].color,stars[i].color,stars[i].color,stars[i].color};
						ci.mesh.colors = colors;

						starQuad.Add(ci);

				}
			}
			// -------------------------------------------
			// Combine Quad Meshes
			Mesh m = new Mesh();
			m.name = "StarFieldMesh";
			m.CombineMeshes(starQuad.ToArray());
			;
			// over size mesh bounds to avoid camera frustum culling for Vertex transformation in shader 
			m.bounds = new Bounds ( Vector3.zero, Vector3.one * 2e9f); // less than 2,147,483,648
//			m.hideFlags = HideFlags.DontSave;
			return m;
		}

		protected Mesh createQuad (float size){

			Vector3[] Vertices = 
			{
				// 4 vertexs for 2 triangles 
				new Vector3( 1, 1, 0) * size,
				new Vector3(-1, 1, 0) * size,
				new Vector3( 1,-1, 0) * size,
				new Vector3(-1,-1, 0) * size
			};

			Vector2[] uv = 
			{ 
				// 2 triangles uv
				new Vector2(0, 1), 
				new Vector2(1, 1),
				new Vector2(0, 0),
				new Vector2(1, 0)
			};

			int[] triangles = new int[6]
			{
				// 2 triangles
				0, 2, 1,
				2, 3, 1
			};

			Mesh m = new Mesh();
			
			m.vertices = Vertices;
			m.uv = uv;
			m.triangles = triangles;
			m.RecalculateNormals();
	//		m.name = "StarSprite"; // debug
			m.hideFlags = HideFlags.DontSave;// prevent leak ?
			return m;
		}

		// Billboard will facing the center origin of the GameObject pivot 
		private Matrix4x4 BillboardMatrix (Vector3 particlePosition)
		{
			Vector3 direction = particlePosition - Vector3.zero;
			direction.Normalize();
			
			Vector3 particleRight = Vector3.Cross(direction, Vector3.up);
			particleRight.Normalize();
			
			Vector3 particleUp = Vector3.Cross(particleRight, direction);
			
			Matrix4x4 matrix = new Matrix4x4();

			matrix.SetColumn(0, particleRight);		// right
			matrix.SetColumn(1, particleUp);		// up
			matrix.SetColumn(2, direction);			// forward
			matrix.SetColumn(3, particlePosition);	// position

			return matrix;
		}
	}
}
