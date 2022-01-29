using UnityEngine;

public class FontHolder : MonoBehaviour
{
    [SerializeField] private Font[] Fonts;
    private static Font[] _fonts;
    void Awake() => _fonts = Fonts;
    public static Font GetFont(int i) => _fonts[i];
}
