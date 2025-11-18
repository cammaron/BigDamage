using System;
using UnityEngine;

namespace BigDamage
{
    public class ScaleWithDistance : MonoBehaviour
    {
		public float minDistance = 3f;

		public float? maxScale;
		public float? extraBaseScale;

		public bool debugLog = false;

		Vector3 originalScale;

		protected void Awake()
        {
			originalScale = transform.localScale;
        }

		protected void OnDestroy()
        {
			transform.localScale = originalScale;
        }

		protected void OnDisable()
        {
			transform.localScale = originalScale;
        }
		protected void Update()
        {
			float distanceFromCamera = Vector3.Distance(transform.position, GameData.CamControl.transform.position);
			float distanceScaleMultiplier = Mathf.Max(1, ((distanceFromCamera - minDistance) / Main.distanceToDoubleSize.Value) * 2f);
			float totalMulti = extraBaseScale.GetValueOrDefault(1) * distanceScaleMultiplier;
			if (maxScale.HasValue) totalMulti = Mathf.Min(maxScale.Value, totalMulti);
			transform.localScale = originalScale * totalMulti;
			if (debugLog) Main.Log.LogInfo($"ScaleWithDistance: distFromCamera {distanceFromCamera}, distScaleMulti {distanceScaleMultiplier}, totalMulti {totalMulti}, newScale {transform.localScale.x}");
		}
    }
}
