using UnityEngine;
using System;

namespace uSky
{	
	[Serializable] 
	public class uSkyAmbient
	{
		[Tooltip("Ambient lighting coming from above.")]
		public Gradient SkyColor = new Gradient ()
		{
			colorKeys = new GradientColorKey[] {
				new GradientColorKey(new Color32(028, 032, 040, 255), 0.225f),
				new GradientColorKey(new Color32(055, 065, 063, 255), 0.25f),
				new GradientColorKey(new Color32(148, 179, 219, 255), 0.28f),
				new GradientColorKey(new Color32(148, 179, 219, 255), 0.72f),
				new GradientColorKey(new Color32(055, 065, 063, 255), 0.75f),
				new GradientColorKey(new Color32(028, 032, 040, 255), 0.775f),
			},
			alphaKeys = new GradientAlphaKey[] {
				new GradientAlphaKey(1.0f, 0.0f),
				new GradientAlphaKey(1.0f, 1.0f)
			}
		};
		[Tooltip("Ambient lighting coming from side.")]
		public Gradient EquatorColor = new Gradient ()
		{
			colorKeys = new GradientColorKey[] {
				new GradientColorKey(new Color32(020, 025, 036, 255), 0.225f),
				new GradientColorKey(new Color32(080, 070, 050, 255), 0.25f),
				new GradientColorKey(new Color32(102, 138, 168, 255), 0.28f),
				new GradientColorKey(new Color32(102, 138, 168, 255), 0.72f),
				new GradientColorKey(new Color32(080, 070, 050, 255), 0.75f),
				new GradientColorKey(new Color32(020, 025, 036, 255), 0.775f),
			},
			alphaKeys = new GradientAlphaKey[] {
				new GradientAlphaKey(1.0f, 0.0f),
				new GradientAlphaKey(1.0f, 1.0f)
			}
		};
		[Tooltip("Ambient lighting coming from below.")]
		public Gradient GroundColor = new Gradient ()
		{
			colorKeys = new GradientColorKey[] {
				new GradientColorKey(new Color32(020, 020, 020, 255), 0.24f),
				new GradientColorKey(new Color32(051, 051, 051, 255), 0.27f),
				new GradientColorKey(new Color32(051, 051, 051, 255), 0.73f),
				new GradientColorKey(new Color32(020, 020, 020, 255), 0.76f),
			},
			alphaKeys = new GradientAlphaKey[] {
				new GradientAlphaKey(1.0f, 0.0f),
				new GradientAlphaKey(1.0f, 1.0f)
			}
		};

	}
}
