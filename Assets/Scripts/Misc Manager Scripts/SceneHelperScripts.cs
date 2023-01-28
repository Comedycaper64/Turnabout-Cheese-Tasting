using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneHelperScripts : MonoBehaviour
{
    public static SceneHelperScripts Instance {get; private set;}
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image sceneBackground;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one SceneHelper! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ChangeSceneBackground(Sprite background)
    {
        sceneBackground.sprite = background;
    }

    public IEnumerator FadeToBlack()
    {
        while(canvasGroup.alpha > .001f)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    public IEnumerator FadeFromBlack()
    {
        if (canvasGroup.alpha > .99f)
        {
            yield return null;
        }
        else
        {
            while(canvasGroup.alpha < .99f)
            {
                canvasGroup.alpha += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }
    }
}
