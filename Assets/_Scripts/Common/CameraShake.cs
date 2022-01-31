using UnityEngine;
using System.Collections;

namespace Gismo.Effects
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake instance;
        bool isShaking;

        private void Awake()
        {
            if (!instance)
                instance = this;
        }

        public void DoShake(float duration, float magnitude)
        {
            if(!isShaking)
                StartCoroutine(Shake(duration, magnitude));
        }

        public IEnumerator Shake(float duration, float magnitude)
        {
            isShaking = true;
            Vector3 orignalPosition = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.position = new Vector3(x, y, -10f);
                elapsed += Time.deltaTime;
                yield return 0;
            }
            transform.position = orignalPosition;

            isShaking = false;
        }
    }
}
