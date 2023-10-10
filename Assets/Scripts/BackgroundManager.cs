using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundManager : MonoBehaviour {

    const int cloudTileSize = 16;

    [SerializeField] CinemachineVirtualCamera followCamera;
    [SerializeField] RuleTile cloudsRuleTile;

    Vector3[] backgroundStartingPositions = new Vector3[4];
    Transform[] backgroundTransforms = new Transform[4];
    float[] backgroundMovementMultiplier = new float[4] { 0.3f, 0.45f, 0.6f, 0.75f };
    Transform camTransform;
    Vector3 camStartingPosition;
    float cloudDrift;
    Camera mainCam;

    Vector3Int[] lastBottomLeft = new Vector3Int[4];


    private void Awake() {
        cloudDrift = (Random.value + 0.5f) *
                         (Random.value < 0.5f ? -1 : 1);

        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);

        for (int i = 0; i < backgroundTransforms.Length; i++) {
            backgroundTransforms[i] = transform.GetChild(i).transform;
            backgroundStartingPositions[i] = backgroundTransforms[i].position;
        }

        camTransform = followCamera.transform;
        camStartingPosition = camTransform.localPosition;

        InitializeBackgrounds();
        UpdateBackgrounds();
    }

    void OnCameraUpdated(CinemachineBrain brain) {
        UpdateBackgroundPosition();
        UpdateBackgrounds();
    }

    void UpdateBackgroundPosition() {
        Vector3 driftDelta = Vector3.right * cloudDrift * Time.time;
        for (int i = 0; i < backgroundTransforms.Length; i++) {
            backgroundTransforms[i].position = backgroundStartingPositions[i] +
                                               (camTransform.localPosition - camStartingPosition - driftDelta) *
                                               backgroundMovementMultiplier[i] + driftDelta;
        }
    }


    void InitializeBackgrounds() {


        for (int i = 0; i < backgroundTransforms.Length; i++) {
            Tilemap tilemap = backgroundTransforms[i].GetComponent<Tilemap>();
            Vector3Int 
                bottomLeft = CalculateCloudGenerationCoordinates(tilemap),
                topRight = CalculateUpperRight(bottomLeft);
            
                AddClouds(tilemap, bottomLeft.x, bottomLeft.y, topRight.x, topRight.y);
            
            // more distant backgrounds are grey-blue shifted (blue value not decreased)
            float a = 1 - i * 0.07f;
            Color color = backgroundTransforms[0].GetComponent<Tilemap>().color;
            color = new Color(color.r * a, color.g * a, color.b, color.a * a);
            tilemap.color = color;
        }
    }

    void UpdateBackgrounds() {
        for (int i = 0; i < backgroundTransforms.Length; i++) {
            Tilemap tilemap = backgroundTransforms[i].GetComponent<Tilemap>();
            Vector3Int bottomLeft = CalculateCloudGenerationCoordinates(tilemap);
            if (bottomLeft == lastBottomLeft[i]) {
                continue;
            }

            AddNewCloudsAndRemoveOld(tilemap, lastBottomLeft[i], bottomLeft);
            lastBottomLeft[i] = bottomLeft;
        }

    }


    // adds new clouds within a buffer area around the screen where needed,
    // and removes old clouds that have moved outside this buffer area
    void AddNewCloudsAndRemoveOld(Tilemap tilemap, Vector3Int oldMin, Vector3Int newMin) {
        Vector3Int
            oldMax = CalculateUpperRight(oldMin),
            newMax = CalculateUpperRight(newMin);


        int sideMinY = Mathf.Max(oldMin.y, newMin.y),
            sideMaxY = Mathf.Min(newMax.y, oldMax.y);

        // top
        if (newMax.y > oldMax.y)
            AddClouds(tilemap, newMin.x, oldMax.y, newMax.x, newMax.y);
        else RemoveClouds(tilemap, oldMin.x, newMax.y, oldMax.x, oldMax.y);

        // left
        if (newMin.x < oldMin.x)
            AddClouds(tilemap, newMin.x, sideMinY, oldMin.x, sideMaxY);
        else RemoveClouds(tilemap, oldMin.x, sideMinY, newMin.x, sideMaxY);

        // right
        if (newMax.x > oldMax.x)
            AddClouds(tilemap, oldMax.x, sideMinY, newMax.x, sideMaxY);
        else RemoveClouds(tilemap, newMax.x, sideMinY, oldMax.x, sideMaxY);

        // bottom
        if (newMin.y < oldMin.y)
            AddClouds(tilemap, newMin.x, newMin.y, newMax.x, oldMin.y);
        else RemoveClouds(tilemap, oldMin.x, oldMin.y, oldMax.x, newMin.y);

    }

    void AddClouds(Tilemap tilemap, int i0, int j0, int i1, int j1) {
        if (i1 <= i0 || j1 <= j0) return;

        float cloudProbability = 0.004f;

        for (int i = i0; i < i1; i++) {
            for (int j = j0; j < j1; j++) {
                bool makeCloud = Random.value < cloudProbability && !tilemap.HasTile(new Vector3Int(i, j, 0));// &&  !IsCloudWithinDistance(tilemap, i, j, 16, false, false);
                if (makeCloud) {
                    tilemap.SetTile(new Vector3Int(i, j, 0), cloudsRuleTile);
                }
            }
        }
    }

    void RemoveClouds(Tilemap tilemap, int i0, int j0, int i1, int j1) {
        if (i1 <= i0 || j1 <= j0) return;
        for (int i = i0; i < i1; i++) {
            for (int j = j0; j < j1; j++) {
                tilemap.SetTile(new Vector3Int(i, j, 0), null);
            }
        }

    }


    // returns bottom left tile map coordinate of the area in which clouds should exist to cover the current camera
    Vector3Int CalculateCloudGenerationCoordinates(Tilemap tilemap) {
        Vector3Int result;
        Vector3 tilemapPosition = tilemap.transform.position;
        result = Vector3Int.RoundToInt(camTransform.position - tilemapPosition);
        result -= Vector3Int.one * (int)(
                                            (followCamera.m_Lens.OrthographicSize + 1) +
                                            cloudTileSize
                                        );
        return result;
    }


    Vector3Int CalculateUpperRight(Vector3Int bottomLeft) {
        int difference = 2 * ((int)followCamera.m_Lens.OrthographicSize + cloudTileSize);
        return bottomLeft + new Vector3Int(difference, difference, 0);
    }

}
