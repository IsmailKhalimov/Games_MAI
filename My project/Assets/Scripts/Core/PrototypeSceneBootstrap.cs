using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrototypeSceneBootstrap : MonoBehaviour
{
    private const string RootName = "PrototypeRuntimeRoot";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != "Prototype" && scene.name != "SampleScene")
        {
            return;
        }

        if (GameObject.Find(RootName) != null)
        {
            return;
        }

        BuildPrototypeScene();
    }

    private static void BuildPrototypeScene()
    {
        var root = new GameObject(RootName);
        CleanupDefaultSceneObjects();

        SetupLighting(root.transform);
        SetupGround(root.transform);

        SetupManager(root.transform);
        PlayerInteractor player = SetupPlayer(root.transform);

        SetupDoorArea(root.transform);
        SetupPlatform(root.transform);
        SetupEnemy(root.transform);
        SetupCrate(root.transform);
        SetupHud(root.transform, player);
    }

    private static void SetupLighting(Transform parent)
    {
        Light directional = Object.FindFirstObjectByType<Light>();
        if (directional == null)
        {
            GameObject lightGo = new("Directional Light");
            lightGo.transform.SetParent(parent);
            directional = lightGo.AddComponent<Light>();
            directional.type = LightType.Directional;
            directional.intensity = 0.8f;
            directional.shadows = LightShadows.None;
            lightGo.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
        }
        else
        {
            directional.type = LightType.Directional;
            directional.intensity = 0.8f;
            directional.shadows = LightShadows.None;
            directional.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
        }
    }

    private static void SetupGround(Transform parent)
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.SetParent(parent);
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(4f, 1f, 4f);

        Renderer renderer = ground.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(0.2f, 0.35f, 0.2f);
        }
    }

    private static DeferredActionManager SetupManager(Transform parent)
    {
        GameObject managerGo = new("DeferredActionManager");
        managerGo.transform.SetParent(parent);
        var manager = managerGo.AddComponent<DeferredActionManager>();
        manager.SetActivationDelay(1f);
        return manager;
    }

    private static PlayerInteractor SetupPlayer(Transform parent)
    {
        GameObject player = new("Player");
        player.transform.SetParent(parent);
        player.transform.position = new Vector3(0f, 1.1f, -10f);

        var controller = player.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.35f;
        controller.center = new Vector3(0f, 0.9f, 0f);

        GameObject camGo = new("PlayerCamera");
        camGo.transform.SetParent(player.transform);
        camGo.transform.localPosition = new Vector3(0f, 0.7f, 0f);
        camGo.transform.localRotation = Quaternion.identity;

        Camera camera = camGo.AddComponent<Camera>();
        camera.tag = "MainCamera";
        camGo.AddComponent<AudioListener>();

        var interactor = player.AddComponent<PlayerInteractor>();
        interactor.Configure(camera, 5f, 140f, 4f);
        return interactor;
    }

    private static void SetupDoorArea(Transform parent)
    {
        GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frame.name = "DoorFrame";
        frame.transform.SetParent(parent);
        frame.transform.position = new Vector3(-4f, 1.5f, 6f);
        frame.transform.localScale = new Vector3(2.4f, 3f, 0.4f);
        frame.GetComponent<Renderer>().material.color = new Color(0.3f, 0.3f, 0.3f);

        GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leaf.name = "DoorLeaf";
        leaf.transform.SetParent(parent);
        leaf.transform.position = frame.transform.position + new Vector3(0f, 0f, 0.3f);
        leaf.transform.localScale = new Vector3(1.8f, 2.6f, 0.2f);
        leaf.GetComponent<Renderer>().material.color = new Color(0.6f, 0.2f, 0.2f);

        GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        button.name = "DoorButton";
        button.transform.SetParent(parent);
        button.transform.position = new Vector3(-6f, 1f, 5f);
        button.transform.localScale = new Vector3(0.4f, 0.2f, 0.4f);

        Renderer buttonRenderer = button.GetComponent<Renderer>();
        buttonRenderer.material.color = new Color(0.7f, 0.7f, 0.2f);

        var interactable = button.AddComponent<DoorInteractable>();
        interactable.SetDisplayName("Кнопка двери");
        interactable.SetMarkerRenderer(buttonRenderer);
        interactable.SetColors(new Color(0.7f, 0.7f, 0.2f), Color.yellow);
        interactable.Configure(leaf.transform, new Vector3(0f, 3f, 0f), 2.5f);
    }

    private static void SetupPlatform(Transform parent)
    {
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.name = "MovingPlatform";
        platform.transform.SetParent(parent);
        platform.transform.position = new Vector3(0f, 0.5f, 10f);
        platform.transform.localScale = new Vector3(3f, 0.4f, 3f);

        Renderer renderer = platform.GetComponent<Renderer>();
        renderer.material.color = new Color(0.2f, 0.4f, 0.8f);

        var interactable = platform.AddComponent<MovingPlatformInteractable>();
        interactable.SetDisplayName("Платформа");
        interactable.SetMarkerRenderer(renderer);
        interactable.SetColors(new Color(0.2f, 0.4f, 0.8f), Color.yellow);
        interactable.Configure(new Vector3(0f, 0f, 6f), 1.8f);
    }

    private static void SetupEnemy(Transform parent)
    {
        GameObject enemy = new("EnemyDummy");
        enemy.transform.SetParent(parent);
        enemy.transform.position = new Vector3(6f, 1f, 8f);
        Color bodyColor = new(0.82f, 0.67f, 0.52f);

        GameObject torso = GameObject.CreatePrimitive(PrimitiveType.Cube);
        torso.name = "Torso";
        torso.transform.SetParent(enemy.transform);
        torso.transform.localPosition = new Vector3(0f, 1.25f, 0f);
        torso.transform.localScale = new Vector3(0.8f, 1.2f, 0.38f);
        Renderer torsoRenderer = torso.GetComponent<Renderer>();
        torsoRenderer.material.color = bodyColor;

        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(enemy.transform);
        head.transform.localPosition = new Vector3(0f, 2.08f, 0f);
        head.transform.localScale = new Vector3(0.48f, 0.48f, 0.48f);
        head.GetComponent<Renderer>().material.color = bodyColor;

        GameObject leftEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leftEye.name = "LeftEye";
        leftEye.transform.SetParent(head.transform);
        leftEye.transform.localPosition = new Vector3(-0.12f, 0.03f, 0.45f);
        leftEye.transform.localScale = Vector3.one * 0.12f;
        leftEye.GetComponent<Renderer>().material.color = Color.black;

        GameObject rightEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightEye.name = "RightEye";
        rightEye.transform.SetParent(head.transform);
        rightEye.transform.localPosition = new Vector3(0.12f, 0.03f, 0.45f);
        rightEye.transform.localScale = Vector3.one * 0.12f;
        rightEye.GetComponent<Renderer>().material.color = Color.black;

        GameObject leftArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        leftArm.name = "LeftArm";
        leftArm.transform.SetParent(enemy.transform);
        leftArm.transform.localPosition = new Vector3(-0.58f, 1.22f, 0f);
        leftArm.transform.localScale = new Vector3(0.22f, 0.55f, 0.22f);
        leftArm.GetComponent<Renderer>().material.color = bodyColor;

        GameObject rightArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        rightArm.name = "RightArm";
        rightArm.transform.SetParent(enemy.transform);
        rightArm.transform.localPosition = new Vector3(0.58f, 1.22f, 0f);
        rightArm.transform.localScale = new Vector3(0.22f, 0.55f, 0.22f);
        rightArm.GetComponent<Renderer>().material.color = bodyColor;

        GameObject leftLeg = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        leftLeg.name = "LeftLeg";
        leftLeg.transform.SetParent(enemy.transform);
        leftLeg.transform.localPosition = new Vector3(-0.2f, 0.5f, 0f);
        leftLeg.transform.localScale = new Vector3(0.24f, 0.72f, 0.24f);
        leftLeg.GetComponent<Renderer>().material.color = bodyColor;

        GameObject rightLeg = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        rightLeg.name = "RightLeg";
        rightLeg.transform.SetParent(enemy.transform);
        rightLeg.transform.localPosition = new Vector3(0.2f, 0.5f, 0f);
        rightLeg.transform.localScale = new Vector3(0.24f, 0.72f, 0.24f);
        rightLeg.GetComponent<Renderer>().material.color = bodyColor;

        var interactable = enemy.AddComponent<EnemyDummyInteractable>();
        interactable.SetDisplayName("Враг");
        interactable.SetMarkerRenderer(torsoRenderer);
        interactable.SetColors(bodyColor, Color.yellow);
        interactable.ConfigureDamage(16);
    }

    private static void SetupCrate(Transform parent)
    {
        GameObject crate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crate.name = "Crate";
        crate.transform.SetParent(parent);
        crate.transform.position = new Vector3(3f, 0.75f, 5f);
        crate.transform.localScale = Vector3.one * 1.5f;

        Renderer renderer = crate.GetComponent<Renderer>();
        renderer.material.color = new Color(0.6f, 0.45f, 0.2f);

        Rigidbody rigidbody = crate.AddComponent<Rigidbody>();
        rigidbody.mass = 2f;
        rigidbody.angularDamping = 0.2f;
        rigidbody.linearDamping = 0.1f;

        var interactable = crate.AddComponent<CrateInteractable>();
        interactable.SetDisplayName("Ящик");
        interactable.SetMarkerRenderer(renderer);
        interactable.SetColors(new Color(0.6f, 0.45f, 0.2f), Color.yellow);
        interactable.Configure(rigidbody, new Vector3(1f, 0.4f, 1f), 6f);
    }

    private static void SetupHud(Transform parent, PlayerInteractor playerInteractor)
    {
        GameObject canvasGo = new("Canvas");
        canvasGo.transform.SetParent(parent);
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Text target = CreateText("TargetText", canvasGo.transform, font, new Vector2(12f, -12f), "Цель: нет");
        Text marks = CreateText("MarksText", canvasGo.transform, font, new Vector2(12f, -44f), "Метки: 0");
        Text countdown = CreateText("CountdownText", canvasGo.transform, font, new Vector2(12f, -76f), "Задержка: -");
        Text status = CreateText("StatusText", canvasGo.transform, font, new Vector2(12f, -108f), "Статус: ожидание");
        Text hint = CreateText("HintText", canvasGo.transform, font, new Vector2(12f, -140f), "E - пометить, F - активировать все");
        hint.fontSize = 18;
        hint.color = new Color(0.95f, 0.95f, 0.75f);

        var hud = canvasGo.AddComponent<GameplayHud>();
        hud.Configure(target, marks, countdown, hint, status, playerInteractor);
    }

    private static Text CreateText(string objectName, Transform parent, Font font, Vector2 anchoredPos, string initialText)
    {
        GameObject go = new(objectName);
        go.transform.SetParent(parent);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = new Vector2(700f, 30f);

        Text text = go.AddComponent<Text>();
        text.font = font;
        text.fontSize = 20;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleLeft;
        text.text = initialText;
        return text;
    }

    private static void CleanupDefaultSceneObjects()
    {
        Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera cam in cameras)
        {
            Object.Destroy(cam.gameObject);
        }
    }
}
