// ScenifyXR_ProceduralGenerator.cs
using UnityEngine;

using TMPro;

[CreateAssetMenu(menuName = "ScenifyXR/Prefab Recipe")]
public class PrefabRecipe : ScriptableObject
{
    public string objectName;
    public Vector3 size = Vector3.one;
    public Color baseColor = Color.gray;
    public string[] labels;
    public string[] ports;
    public bool hasFaultLight = false;
    public string faultType = "red_blink";
}

public class ProceduralGenerator : MonoBehaviour
{
    public static GameObject Create(PrefabRecipe recipe)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = recipe.objectName;
        go.transform.localScale = recipe.size;

        var mat = new Material(Shader.Find("Standard"));
        mat.color = recipe.baseColor;
        go.GetComponent<Renderer>().material = mat;

        foreach (string label in recipe.labels)
            AddLabel(go, label, new Vector3(0, recipe.size.y/2 + 0.05f, 0));

        foreach (string port in recipe.ports)
            AddPort(go, port);

        if (recipe.hasFaultLight)
            AddFaultLight(go);

        var grab = go.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable.MovementType.Instantaneous;

        return go;
    }

    static void AddLabel(GameObject parent, string text, Vector3 offset)
    {
        var labelObj = new GameObject("Label_" + text);
        labelObj.transform.SetParent(parent.transform);
        labelObj.transform.localPosition = offset;
        var tmp = labelObj.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = 0.15f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
    }

    static void AddPort(GameObject parent, string name)
    {
        var port = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        port.name = "Port_" + name;
        port.transform.SetParent(parent.transform);
        port.transform.localScale = new Vector3(0.03f, 0.01f, 0.03f);
        port.transform.localPosition = new Vector3(
            Random.Range(-0.4f, 0.4f) * parent.transform.localScale.x,
            -parent.transform.localScale.y/2 - 0.02f,
            Random.Range(-0.15f, 0.15f) * parent.transform.localScale.z
        );
        port.GetComponent<Renderer>().material.color = Color.black;
        port.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
    }

    static void AddFaultLight(GameObject parent)
    {
        var lightObj = new GameObject("FaultLight");
        lightObj.transform.SetParent(parent.transform);
        lightObj.transform.localPosition = new Vector3(0, parent.transform.localScale.y/2 + 0.1f, 0);
        var light = lightObj.AddComponent<Light>();
        light.color = Color.red;
        light.intensity = 2f;
        light.range = 1f;
        var blinker = lightObj.AddComponent<Blinker>();
        blinker.frequency = 2f;
    }
}

public class Blinker : MonoBehaviour
{
    public float frequency = 1f;
    Light light;
    void Start() { light = GetComponent<Light>(); }
    void Update()
    {
        light.enabled = (Mathf.Sin(Time.time * frequency * Mathf.PI) > 0);
    }
}