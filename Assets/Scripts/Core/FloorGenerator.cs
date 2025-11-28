using UnityEngine;
using UnityEngine.Tilemaps;

namespace SlopJam.Core
{
    public class FloorGenerator : MonoBehaviour
    {
        [SerializeField] private Sprite[] floorSprites;
        [SerializeField] private int width = 40;
        [SerializeField] private int height = 40;
        [SerializeField] private Vector2 centerPosition = new Vector2(-10f, 0f);
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private float noiseScale = 0.1f;

        private void Start()
        {
            GenerateFloor();
        }

        private void GenerateFloor()
        {
            if (floorSprites == null || floorSprites.Length == 0) return;

            var gridGo = new GameObject("Grid");
            gridGo.transform.position = new Vector3(centerPosition.x, centerPosition.y, 0f);
            // If we want to scale the tiles down, we can scale the Grid
            gridGo.transform.localScale = Vector3.one * tileSize;
            
            var grid = gridGo.AddComponent<Grid>();

            var tilemapGo = new GameObject("Floor");
            tilemapGo.transform.SetParent(gridGo.transform, false);
            var tilemap = tilemapGo.AddComponent<Tilemap>();
            var renderer = tilemapGo.AddComponent<TilemapRenderer>();
            renderer.sortingOrder = -10;

            var tiles = new Tile[floorSprites.Length];
            for (int i = 0; i < floorSprites.Length; i++)
            {
                tiles[i] = ScriptableObject.CreateInstance<Tile>();
                tiles[i].sprite = floorSprites[i];
            }

            int halfWidth = width / 2;
            int halfHeight = height / 2;

            for (int x = -halfWidth; x <= halfWidth; x++)
            {
                for (int y = -halfHeight; y <= halfHeight; y++)
                {
                    float noise = Mathf.PerlinNoise((x + centerPosition.x) * noiseScale, (y + centerPosition.y) * noiseScale);
                    int index = Mathf.Clamp(Mathf.FloorToInt(noise * floorSprites.Length), 0, floorSprites.Length - 1);
                    tilemap.SetTile(new Vector3Int(x, y, 0), tiles[index]);
                }
            }
        }
    }
}

