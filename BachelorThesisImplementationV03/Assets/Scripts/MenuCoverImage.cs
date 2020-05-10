using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Contains functions for changing the transparency of images and texts.
/// Also contains functionality for loading next scene when image is clicked.
/// This script is used by images present in menu scenes.
/// </summary>
public class MenuCoverImage : MonoBehaviour
{
    public Image coverImage;
    public Text title;
    public string nextScene;

    private Color cover;
    private Color text;

    /// <summary>
    /// 
    /// </summary>
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
