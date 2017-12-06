using UnityEngine;

public struct MineralStats {
    public int Value;
    public int MinDepth;
    public int MaxDepth;
    public int Density;

    public MineralStats(int value, int minDepth, int maxDepth, int density) {
        Value = value;
        MinDepth = minDepth;
        MaxDepth = maxDepth;
        Density = density;
    }
}

public static class Tuning {

    public const float PLAYER_MOVE_SPEED = 4.0f;
    public const float PLAYER_FLY_SPEED = 5.8f;
    public const float PLAYER_DIG_SPEED = 2.0f;
    public const float PLAYER_ACCEL = 2.0f;
    public const float PLAYER_DECEL = 2.2f;
    public const float CAMERA_FOLLOW_SPEED = 3.0f;

    public const float PLAYER_EXHAUST_EMITRATE = 48f;

    public const int MAP_WIDTH = 50;
    public const int MAP_HEIGHT = 200;

    public const float FUEL_IDLE_RATE = 0.02f;
    public const float FUEL_DRIVING_RATE = 0.12f;
    public const float FUEL_DIGGING_RATE = 0.45f;
    public const float FUEL_FLYING_RATE = 0.22f;
    public const float FUEL_FLASHLIGHT_RATE = 0.85f;

    public const float COST_FUEL = 10f;

    public static MineralStats GetMineralStats(CargoType cargo) {
        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (cargo) {
            case CargoType.Iron:
                return new MineralStats(10, 0, 120, 270);
            case CargoType.Copper:
                return new MineralStats(15, 0, 60, 150);
            case CargoType.Tin:
                return new MineralStats(15, 8, 80, 150);
            case CargoType.Silver:
                return new MineralStats(40, 20, 140, 140);
            case CargoType.Gold:
                return new MineralStats(80, 40, 160, 140);
            case CargoType.Platinum:
                return new MineralStats(150, 70, 180, 130);
            case CargoType.Emerald:
                return new MineralStats(300, 85, 190, 100);
            case CargoType.Ruby:
                return new MineralStats(300, 95, 190, 100);
            case CargoType.Diamond:
                return new MineralStats(800, 135, MAP_HEIGHT, 50);
            case CargoType.Adamantium:
                return new MineralStats(2000, 170, MAP_HEIGHT, 45);
            case CargoType.Unobtainium:
                return new MineralStats(1000000, MAP_HEIGHT - 5, MAP_HEIGHT, 1);
            case CargoType.Bronze:
                return new MineralStats(50, 0, 0, 0);
            case CargoType.Oragnium:
                return new MineralStats(250, 0, 0, 0);
            case CargoType.Katanium:
                return new MineralStats(3000, 0, 0, 0);
            default:
                Debug.LogError("lol rekt, Invalid value in GetMineralStats: " + cargo);
                return new MineralStats(0, 0, 0, 0);
        }
    }

    public static int GetUpgradeCost(int level) {
        switch (level) {
            case 1:
                return 150;
            case 2:
                return 500;
            case 3:
                return 2000;
            case 4:
                return 5000;
            case 5:
                return 15000;
            case 6:
                return 40000;
            case 7:
                return 75000;
            case 8:
                return 0; // dummy
            default:
                Debug.LogError("bad level value to GetUpgradeCost");
                return 0;
        }
    }

    public static float GetFuelCapacity(int level) {
        switch (level) {
            case 0:
                return 15f;
            case 1:
                return 20f;
            case 2:
                return 25f;
            case 3:
                return 30f;
            case 4:
                return 35f;
            case 5:
                return 40f;
            case 6:
                return 45f;
            case 7:
                return 50f;
            default:
                Debug.LogError("bad level value to GetFuelCapacity");
                return 0f;
        }
    }

    public static int GetCargoCapacity(int level) {
        switch (level) {
            case 0:
                return 6;
            case 1:
                return 10;
            case 2:
                return 14;
            case 3:
                return 18;
            case 4:
                return 22;
            case 5:
                return 26;
            case 6:
                return 30;
            case 7:
                return 35;
            default:
                Debug.LogError("bad level value to GetCargoCapacity");
                return 0;
        }
    }

}
