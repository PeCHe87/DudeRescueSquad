using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DudeResqueSquad
{
    public class FPSCounter : MonoBehaviour
    {
        private Text _txt;
        private float _count;

        private float _deltaTime = 0f;

        #region Unity mehtods

        void Awake()
        {
            _txt = GetComponent<Text>();
#if NO_CHEATS
		//gameObject.SetActive(false);
#endif
        }

        private void OnEnable()
        {
            StartCoroutine(Run());
        }

        private void Update()
        {
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        }

        IEnumerator Run()
        {
            while (true)
            {
                if (Time.timeScale == 1)
                {
                    yield return new WaitForSeconds(0.1f);

                    _count = (1 / _deltaTime);
                    _txt.text = "FPS :" + (Mathf.Round(_count));
                }
                else
                {
                    _txt.text = "Pause";
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
        #endregion
    }
}