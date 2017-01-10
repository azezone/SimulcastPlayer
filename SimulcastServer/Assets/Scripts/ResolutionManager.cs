using UnityEngine;
public class ResolutionManager : MonoBehaviour {
    private int arrow;
    private int[] widths = new int[] { 480, 720, 1024, 1280, 1360, 1366, 1600, 1920, 2560 };
    private int[] heights = new int[] { 270, 405, 576, 720, 768, 768, 900, 1080, 1440 };
    void Start() {
        for (int i = 0; i < widths.Length; i++) {
            if (Screen.width == widths[i]) {
                arrow = i;
            }
        }
    }
    void Awake()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);//不要删除管理者对象  
    }
    void Update() {
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.UpArrow)) {
            if (arrow < widths.Length - 1)
                arrow++;
            Screen.SetResolution(widths[arrow], heights[arrow], false);
        }
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.DownArrow)) {
            if (arrow > 0)
                arrow--;
            Screen.SetResolution(widths[arrow], heights[arrow], false);
        }
    }
    void OnGUI() {
        int index = QualitySettings.GetQualityLevel();
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.UpArrow))
            index++;
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.DownArrow))
            index--;
        QualitySettings.SetQualityLevel(index);
    }
}