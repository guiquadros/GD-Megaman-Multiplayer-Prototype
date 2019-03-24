using UnityEngine;
using Megaman.Util;
using System.Collections.Generic;

namespace Megaman
{
    [RequireComponent(typeof(Camera))]
    public class CameraBehavior : MonoBehaviour
    {

        private new Camera camera;

        public List<Transform> Tranformers { get; set; }

        [Header("Camera")]
        [SerializeField]
        [Range(1, 30)]
        private float minOrthographicZoom = 7;

        [Range(1, 30)]
        [SerializeField]
        private float maxOrthographicZoom = 14;

        private float defaultZ, defaultY;

        private Transform lefTransform, rightTransform;

        [SerializeField]
        private float cameraOrthographicSizeMultiplier;

        [SerializeField]
        private float minCameraPos;

        [SerializeField]
        private float maxCameraPos;

        void Awake()
        {
            camera = GetComponent<Camera>();
            Tranformers = new List<Transform>();

            this.lefTransform = transform.GetChild(0);
            this.rightTransform = transform.GetChild(1);
        }

        void Start()
        {
            defaultZ = camera.transform.position.z;
            defaultY = camera.transform.position.y;
        }

        private void Update()
        {
            this.UpdateCameraOrtographicSize();

            Vector3 cameraPos = camera.transform.position;

            float xPos = camera.orthographicSize*cameraOrthographicSizeMultiplier;

            cameraPos.x += xPos;
            rightTransform.position = cameraPos;
            cameraPos.x -= xPos * 2f;
            lefTransform.position = cameraPos;
        }

        private void UpdateCameraOrtographicSize()
        {
            //Define a posição atual da câmera como sendo o centro entre os players
            //posicao = (pos1 + pos2) / 2;
            Vector3 newCameraPos = Vector3.zero;
            for (int i = 0; i < Tranformers.Count; i++)
            {
                newCameraPos += Tranformers[i].position;
            }

            if (Tranformers.Count > 1)
                newCameraPos = newCameraPos*.5f;

            //Corrige a posição Z da câmera
            newCameraPos.z = defaultZ;
            newCameraPos.y = defaultY;

            newCameraPos.x = Mathf.Clamp(newCameraPos.x, minCameraPos, maxCameraPos);
            camera.transform.position = newCameraPos;
        }
    }
}