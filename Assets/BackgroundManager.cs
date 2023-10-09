using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundManager : MonoBehaviour {

    [SerializeField] CinemachineVirtualCamera followCamera;
    [SerializeField] RuleTile cloudsRuleTile;

    Vector3[] backgroundStartingPositions = new Vector3[4];
    Transform[] backgroundTransforms = new Transform[4];
    float[] backgroundMovementMultiplier = new float[4] { 0.5f, 0.6f, 0.7f, 0.8f };
    Transform camTransform;
    Vector3 camStartingPosition;

    private void Awake() {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);

        for (int i = 0; i < backgroundTransforms.Length; i++) {
            backgroundTransforms[i] = transform.GetChild(i).transform;
            backgroundStartingPositions[i] = backgroundTransforms[i].position;
        }

        camTransform = followCamera.transform;
        camStartingPosition = camTransform.localPosition;
        GenerateBackgrounds();
    }

    void OnCameraUpdated(CinemachineBrain brain) {
        UpdateBackgrounds();
    }

    void UpdateBackgrounds() {
        for (int i = 0; i < backgroundTransforms.Length; i++) {
            backgroundTransforms[i].position = backgroundStartingPositions[i] +
                                               (camTransform.localPosition - camStartingPosition) *
                                               backgroundMovementMultiplier[i];
        }
    }



    void GenerateBackgrounds() {
        for (int i = 0; i < backgroundTransforms.Length; i++) {
            Tilemap tilemap = backgroundTransforms[i].GetComponent<Tilemap>();
            AddClouds(tilemap, -100, -100, 100, 100);

            float a = 0.9f - i * 0.07f;
            tilemap.color = new Color(a, a, a, a);
        }
    }


    void AddClouds(Tilemap tilemap, int i0, int j0, int i1, int j1) {
        float cloudProbability = 0.01f;

        for (int i = i0; i < i1; i++) {
            for (int j = j0; j < j1; j++) {
                tilemap.SetTile(new Vector3Int(i, j, 0), Random.value < cloudProbability ? cloudsRuleTile : null);
            }
        }
    }








}
