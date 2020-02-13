using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuCoverImage : MonoBehaviour
{
    public Image coverImage;
    public Text title;
    public string nextScene;

    private Color cover;
    private Color text;

    // Start is called before the first frame update
    void Start()
    {
        cover = coverImage.color;
        text = title.color;

        cover.a = 0;
        text.a = 0;

        coverImage.color = cover;
        title.color = text;
    }

    public void MouseHoversOver()
    {
        cover.a = 0.8f;
        text.a = 1;

        coverImage.color = cover;
        title.color = text;
    }

    public void MouseIsOutside()
    {
        cover.a = 0;
        text.a = 0;

        coverImage.color = cover;
        title.color = text;
    }

    public void LoadNewScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
