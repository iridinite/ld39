using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject TilePrefab;
    public GameObject TileBackPrefab;
    public GameObject ShopPanel;

    public bool Paused { get; set; }
    public bool Victory { get; set; }
    public bool GameOver { get; private set; }
    public bool ShopOpen { get; private set; }
    public bool ControllerMode { get; set; }

    private GameObject[,] map;
    private Player plr;

    private void Start() {
#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
#endif

        GameOver = false;

        Transform cachedTransform = transform;

        for (int x = -15; x < Tuning.MAP_WIDTH + 15; x++) {
            for (int z = 1; z < 10; z++) {
                Vector3 pos = MapToWorld(x, 0);
                pos.z = z;
                GameObject tile = Instantiate(TilePrefab);
                tile.transform.position = pos;
                tile.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0, 4) * 90f); // randomly rotate for variation
                tile.transform.SetParent(cachedTransform);
                tile.GetComponent<Tile>().type = TileType.Barrier;
            }
        }


        map = new GameObject[Tuning.MAP_WIDTH, Tuning.MAP_HEIGHT];
        for (int y = 0; y < Tuning.MAP_HEIGHT + 8; y++) {
            for (int x = -10; x < Tuning.MAP_WIDTH + 10; x++) {
                GameObject tileBack = Instantiate(TileBackPrefab);
                tileBack.transform.localPosition = MapToWorld(x, y) + new Vector3(0, 0, 0.5f);
                tileBack.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0, 4) * 90f); // randomly rotate for variation
                tileBack.transform.SetParent(cachedTransform);

                GameObject tile = Instantiate(TilePrefab);
                tile.transform.position = MapToWorld(x, y);
                tile.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0, 4) * 90f); // randomly rotate for variation
                tile.transform.SetParent(cachedTransform);
                Tile tilescript = tile.GetComponent<Tile>();

                TileType type;
                if (x >= 0 && x < Tuning.MAP_WIDTH && y < Tuning.MAP_HEIGHT) {
                    map[x, y] = tile;

                    if (y == 0 && x >= 9 && x <= 14)
                        type = TileType.Barrier;
                    else if (y >= Tuning.MAP_HEIGHT - 1)
                        type = TileType.Barrier;
                    else
                        type = TileType.Stone;
                } else {
                    type = TileType.Barrier;
                }

                tilescript.type = type;
            }
        }


        // now generate all minerals
        for (TileType block = TileType.Iron; block <= TileType.Unobtainium; block++) {
            CargoType cargo = Tile.GetCargoType(block);
            MineralStats stats = Tuning.GetMineralStats(cargo);
            while (stats.Density > 0) {
                int mx = Random.Range(0, Tuning.MAP_WIDTH);
                int my = Random.Range(stats.MinDepth, stats.MaxDepth - 1);

                if (map[mx, my] == null) continue;

                var script = map[mx, my].GetComponent<Tile>();
                if (script.type != TileType.Boulder)
                    script.type = block;

                stats.Density--;
            }
        }

        // and place special thingers
        for (TileType block = TileType.Artifact; block <= TileType.Boulder; block++) {
            int count;
            switch (block) {
                case TileType.Artifact:
                    count = Random.Range(4, 7);
                    break;
                case TileType.Boulder:
                    count = Random.Range(100, 130);
                    break;
                default:
                    count = Random.Range(10, 15);
                    break;
            }
            while (count > 0) {
                count--;
                int mx = Random.Range(0, Tuning.MAP_WIDTH);
                int my = Random.Range(30, Tuning.MAP_HEIGHT);

                if (map[mx, my] == null) continue;

                var script = map[mx, my].GetComponent<Tile>();
                if (script.type == TileType.Stone)
                    script.type = block;
            }
        }

        // place a few gaps
        int gaps = Random.Range(50, 75);
        while (gaps > 0) {
            gaps--;
            int mx = Random.Range(0, Tuning.MAP_WIDTH);
            int my = Random.Range(0, Tuning.MAP_HEIGHT);
            int gapsize = Random.Range(0, 5) - 2;

            if (gapsize <= 0) {
                // single dot
                if (mx < 0 || my < 1 || mx >= Tuning.MAP_WIDTH || my >= Tuning.MAP_HEIGHT - 1)
                    continue;
                if (map[mx, my] == null)
                    continue; // cannot remove blocks that are already gone
                if (map[mx, my].GetComponent<Tile>().type != TileType.Stone)
                    continue; // cannot overwrite minerals
                Destroy(map[mx, my]);
                map[mx, my] = null;
            } else {
                // larger gap
                for (int x = mx - gapsize; x <= mx + gapsize; x++) {
                    for (int y = my - gapsize; y <= my + gapsize; y++) {
                        if (x < 0 || y < 1 || x >= Tuning.MAP_WIDTH || y >= Tuning.MAP_HEIGHT - 1)
                            continue; // in map bounds
                        if (Vector2.Distance(new Vector2(x, y), new Vector2(mx, my)) > gapsize)
                            continue; // make circles
                        if (map[x, y] == null)
                            continue; // cannot remove blocks that are already gone
                        if (map[x, y].GetComponent<Tile>().type != TileType.Stone)
                            continue; // cannot overwrite minerals
                        Destroy(map[x, y]);
                        map[x, y] = null;

                        if (Random.value > 0.8f)
                            gapsize = Mathf.Max(1, gapsize - 1);
                    }
                }
            }
        }

        //map[0, 0].GetComponent<Tile>().type = TileType.Unobtainium;
    }

    private void Awake() {
        plr = FindObjectOfType<Player>();
    }

    private void LateUpdate() {
        if (plr.Fuel <= 0f) {
            GameOver = true;
        }
    }

    public GameObject GetTileAt(int x, int y) {
        if (x < 0 || y < 0 || x >= Tuning.MAP_WIDTH || y >= Tuning.MAP_HEIGHT)
            return null;
        return map[x, y];
    }

    public static Vector3 MapToWorld(int x, int y) {
        return new Vector3(x - (Tuning.MAP_WIDTH / 2), -y, 0);
    }

    public static void WorldToMap(Vector3 world, out int mapx, out int mapy) {
        mapx = Mathf.RoundToInt(world.x) + Tuning.MAP_WIDTH / 2;
        mapy = -Mathf.RoundToInt(world.y);
    }

    public void SetShopOpen(bool mode) {
        ShopOpen = mode;
        ShopPanel.SetActive(mode);
        Input.ResetInputAxes();
    }

}
