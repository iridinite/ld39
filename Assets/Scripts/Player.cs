using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public GameObject digParticles;
    public GameObject explosionParticles;
    public ParticleSystem exhaust;

    public GameObject refuelText;
    public Transform propellor;
    public Transform drillForward;
    public Transform drillDown;

    private float accelHor, accelVer;
    private Vector3 lastMoveDir;
    private GameController gctl;
    private Rigidbody cachedRigid;
    private Transform cachedTf;

    private bool didDeathAnimation;
    private bool flashlightTutorialShown;
    private bool facing;
    private float currentAngle;

    private bool digging;
    private bool digHorizontal;
    private float digProgress;
    private Vector3 digStart;
    private Vector3 digTarget;
    private GameObject digTile;

    public int LevelFuel { get; set; }
    public int LevelCargo { get; set; }

    public float Fuel { get; set; }
    public float FuelMax { get; set; }
    public int Money { get; set; }
    public int Score { get; set; }
    public bool Flashlight { get; private set; }

    public List<CargoType> Cargo { get; private set; }
    public int CargoCapacity { get; set; }

    private void Start() {
        accelHor = 0.0f;
        accelVer = 0.0f;
        lastMoveDir = Vector3.zero;
        didDeathAnimation = false;
        flashlightTutorialShown = false;
        digging = false;
        facing = false;
        currentAngle = 0f;
        LevelFuel = 0;
        LevelCargo = 0;
        FuelMax = Tuning.GetFuelCapacity(LevelFuel);
        Fuel = FuelMax;
        CargoCapacity = Tuning.GetCargoCapacity(LevelCargo);
        Cargo = new List<CargoType>();
        Score = 0;
        Money = 0;

        TutorialManager.QueueTutorial(0);
        StartCoroutine(TutorialCoroutine());
    }

    private IEnumerator TutorialCoroutine() {
        yield return new WaitForSeconds(10f);
        TutorialManager.QueueTutorial(5);
        TutorialManager.QueueTutorial(1);

        yield return new WaitForSeconds(15f);
        TutorialManager.QueueTutorial(4);

        yield return new WaitForSeconds(15f);
        TutorialManager.QueueTutorial(6);
    }

    private void Awake() {
        cachedRigid = GetComponent<Rigidbody>();
        cachedTf = GetComponent<Transform>();
        gctl = FindObjectOfType<GameController>();
    }

    private void FixedUpdate() {
        // no world input in shop
        if (gctl.ShopOpen || gctl.Victory || gctl.Paused) return;

        // explosion effect and stuff
        if (gctl.GameOver) {
            exhaust.Stop();

            if (!didDeathAnimation) {
                didDeathAnimation = true;
                ScreenShaker.SetShake(0.8f, 0.1f);
                Instantiate(explosionParticles, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
            }

            return;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown("f6")) {
            Money += 100000;
        }
        if (Input.GetKeyDown("f7")) {
            Fuel = 1f;
        }
        if (Input.GetKeyDown("f8")) {
            Fuel = FuelMax;
        }
#endif

        // animate model rotation
        //Debug.Log(facing);
        if (facing) {
            currentAngle += 500f * Time.fixedDeltaTime;
            if (currentAngle > 90f) currentAngle = 90f;
        } else {
            currentAngle -= 500f * Time.fixedDeltaTime;
            if (currentAngle < -90f) currentAngle = -90f;
        }
        cachedTf.localRotation = Quaternion.Euler(0f, currentAngle, 0f);

        if (digging) {
            // ignore input while digging, and animate position
            accelHor = 0f;
            accelVer = 0f;
            cachedRigid.isKinematic = true;
            cachedRigid.velocity = Vector3.zero;
            digProgress += Time.fixedDeltaTime * Tuning.PLAYER_DIG_SPEED;
            Fuel -= Time.fixedDeltaTime * Tuning.FUEL_DIGGING_RATE;

            SetExhaustStrength(1.0f);
            AnimateScale(drillDown, !digHorizontal);
            AnimateScale(drillForward, digHorizontal);

            ScreenShaker.SetShake(0.1f, 0.01f);

            if (digProgress > 0.5f && digTile != null) {
                digTile.GetComponent<Tile>().PickUp();
                Destroy(digTile);
                digTile = null;
            }

            if (digProgress >= 1.0f) {
                cachedTf.position = digTarget;
                cachedRigid.isKinematic = false;
                digging = false;
            } else {
                cachedTf.position = Vector3.Lerp(digStart, digTarget, digProgress) + (digHorizontal
                    ? Vector3.up * Random.Range(-0.025f, 0.025f)
                    : Vector3.right * Random.Range(-0.045f, 0.045f));
            }

            return;
        }

        // standby fuel consumption
        SetExhaustStrength(0.25f);
        Fuel -= Time.fixedDeltaTime * Tuning.FUEL_IDLE_RATE;

        {
            // update flag whether user is using controller
            float ctlKey = Mathf.Abs(Input.GetAxis("KeyX")) + Mathf.Abs(Input.GetAxis("KeyY"));
            float ctlJoy = Mathf.Abs(Input.GetAxis("JoyX")) + Mathf.Abs(Input.GetAxis("JoyY"));
            if (ctlJoy > 0.1f || ctlKey > 0.1f)
                gctl.ControllerMode = ctlJoy > ctlKey;
            //Debug.Log(gctl.ControllerMode);
        }

        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Flashlight"))
            Flashlight = !Flashlight;
        if (Flashlight) {
            Fuel -= Tuning.FUEL_FLASHLIGHT_RATE * Time.fixedDeltaTime;
            SetExhaustStrength(1f);
        }

        // player position
        int playerMapX, playerMapY;
        GameController.WorldToMap(cachedTf.position, out playerMapX, out playerMapY);
        bool grounded = gctl.GetTileAt(playerMapX, playerMapY + 1) != null;

        // flashlight tutorial when turning it on or sufficiently deep
        if (!flashlightTutorialShown && (Flashlight || playerMapY >= 24)) {
            flashlightTutorialShown = true;
            TutorialManager.QueueTutorial(2);
        }

        // accelerate / decelerate
        accelHor = Mathf.Abs(hor) > 0.001f
            ? Mathf.Min(accelHor + Time.fixedDeltaTime * Tuning.PLAYER_ACCEL, 1.0f)
            : Mathf.Max(accelHor - Time.fixedDeltaTime * Tuning.PLAYER_DECEL, 0.0f);
        accelVer = Mathf.Abs(ver) > 0.001f
            ? Mathf.Min(accelVer + Time.fixedDeltaTime * Tuning.PLAYER_ACCEL, 1.0f)
            : Mathf.Max(accelVer - Time.fixedDeltaTime * Tuning.PLAYER_DECEL, 0.0f);

        // interact with fuel depot
        if (playerMapY == -1 && playerMapX >= 9 && playerMapX <= 14) {
            refuelText.SetActive(true);
            refuelText.GetComponent<TextMesh>().text = gctl.ControllerMode
                ? "Press A to open shop"
                : "Press Enter to open shop";

            if (Input.GetButtonDown("Confirm")) {
                //Debug.Log("Do fancy thing");
                //Fuel = FuelMax;
                Input.ResetInputAxes();
                refuelText.SetActive(false);
                gctl.SetShopOpen(true);
            }
        } else {
            refuelText.SetActive(false);
        }

        // horizontal movement
        Vector3 movedir = lastMoveDir;
        if (Mathf.Abs(hor) > 0.001f) {
            // set horizontal speed
            movedir.x = hor * Tuning.PLAYER_MOVE_SPEED;
            Fuel -= Time.fixedDeltaTime * Tuning.FUEL_DRIVING_RATE;
            SetExhaustStrength(0.75f);

            // set model rotation
            facing = hor < 0f; // left/right

            // try digging maybe
            if (Mathf.Abs(hor) >= 0.85f && // deadzone
                grounded && Mathf.Abs(cachedRigid.velocity.y) < 0.1f && cachedRigid.useGravity && // can't dig sideways while flying
                gctl.GetTileAt(playerMapX + Mathf.RoundToInt(hor), playerMapY) != null) // must be a solid tile in the way

                StartDigging(playerMapX + Mathf.RoundToInt(hor), playerMapY, true);
        }

        AnimateScale(drillDown, false);
        AnimateScale(drillForward, grounded && Mathf.Abs(hor) > 0.1f);

        // vertical movement
        //Debug.Log(ver);
        if (ver > 0.5f) {
            movedir.y = ver * Tuning.PLAYER_FLY_SPEED;
            Fuel -= Time.fixedDeltaTime * Tuning.FUEL_FLYING_RATE;
            SetExhaustStrength(0.75f);
            AnimateScale(propellor, true, 2.5f);
            AnimateScale(drillDown, false);
            AnimateScale(drillForward, false);

            // cancel out gravity
            cachedRigid.useGravity = false;
            Vector3 vel = cachedRigid.velocity;
            vel.y = Mathf.Min(0.0f, vel.y + 4.0f * Time.fixedDeltaTime);
            cachedRigid.velocity = vel;
        } else {
            // was flying, need to accelerate again
            //if (!cachedRigid.useGravity) {
            //    cachedRigid.velocity = accelVer * Tuning.
            //}
            movedir.y = 0;
            cachedRigid.useGravity = true;
            AnimateScale(propellor, !grounded, 2.5f);

            if (grounded && ver < -0.5f && // dead zone
                Mathf.Abs(cachedRigid.velocity.y) < 0.1f && cachedRigid.useGravity && // can't dig while flying
                gctl.GetTileAt(playerMapX, playerMapY + 1) != null) // must be a solid tile
            {
                // downwards
                StartDigging(playerMapX, playerMapY + 1, false);
            }
        }

        // if moving, save the last speed
        if (Mathf.Abs(hor) > 0.001f || Mathf.Abs(ver) > 0.001f)
            lastMoveDir = movedir;

        // apply velocity
        Vector3 finalMoveDir = movedir;
        finalMoveDir.x *= accelHor;
        finalMoveDir.y *= accelVer;
        Vector3 newpos = cachedRigid.position + finalMoveDir * Time.fixedDeltaTime;
        newpos.x = Mathf.Clamp(newpos.x, -(Tuning.MAP_WIDTH / 2), Tuning.MAP_WIDTH / 2.0f - 1.0f);
        cachedRigid.position = newpos;
    }

    private void StartDigging(int targetx, int targety, bool horizontal) {
        accelHor = 0f;
        accelVer = 0f;

        // check type, cannot dig through barriers
        digTile = gctl.GetTileAt(targetx, targety);
        if (digTile.GetComponent<Tile>().type == TileType.Barrier ||
            digTile.GetComponent<Tile>().type == TileType.Boulder) return;

        digging = true;
        digProgress = 0f;
        digStart = cachedTf.position;
        digTarget = GameController.MapToWorld(targetx, targety);
        digHorizontal = horizontal;

        // beautiful programmer art
        GameObject particles = Instantiate(digParticles);
        particles.transform.localPosition = GameController.MapToWorld(targetx, targety);
    }

    private void SetExhaustStrength(float val) {
        var emi = exhaust.emission;
        emi.rateOverTimeMultiplier = Tuning.PLAYER_EXHAUST_EMITRATE * val;
        //exhaust.emissionRate = emi;
    }

    private void AnimateScale(Transform thing, bool open, float deploySpeed = 6f) {
        float scale = thing.localScale.x;
        scale = open
            ? Mathf.Clamp01(scale + Time.fixedDeltaTime * deploySpeed)
            : Mathf.Clamp01(scale - Time.fixedDeltaTime * deploySpeed);
        thing.localScale = new Vector3(scale, scale, scale);
    }

}
