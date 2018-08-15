using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HardDriveGameController : MonoBehaviour
{
    public Game game { get; set; }
    public GameController controller { get; set; }

    Image gameImage;

    public bool downloading { get; set; }
    public bool deleting { get; set; }

	// Use this for initialization
	void Start () 
    {
        RectTransform[] rectTransforms = GetComponentsInParent<RectTransform>();
        gameImage = GetComponent<Image>();
        gameImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ((float)game.space / (float)controller.hardDriveSize) * rectTransforms[1].rect.width);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (downloading)
        {
            gameImage.fillAmount += Time.deltaTime;
            if (gameImage.fillAmount == 1.0f)
            {
                downloading = false;
            }
        }

        if (deleting)
        {
            gameImage.fillAmount -= Time.deltaTime;
            if (gameImage.fillAmount == 0.0f)
            {
                controller.removeGame(this);
                Destroy(gameObject);
            }
        }
	}

    public void download()
    {
        GetComponent<Image>().fillAmount = 0.0f;
        downloading = true;
    }

    public void delete()
    {
        deleting = true;
    }
}
