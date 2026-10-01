using UnityEngine;
using UnityEngine.UI;

public class FlashBang : Item
{
    RacerController victim;

    GameObject imageObject;

    private float duration = 3.0f;

    private float timeRemaining ;

    public override void Use()
    {
        if (imageObject != null) {
            Destroy(imageObject);
            imageObject = null;
        }

        timeRemaining = duration;
        //placeholder
        victim = GameManager.Instance.Racers[Random.Range(0, 4)];

        //flash the camera
        if (victim is PlayerController)
        {
            imageObject = new GameObject("DaFlash");
            imageObject.transform.SetParent(GameManager.Instance.UIManager.MainRaceCanvas.transform, false);

            RectTransform rectTransform = imageObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(1920 * 1.5f, 1080 * 1.5f);

            Image image = imageObject.AddComponent<Image>();
            image.color = new Color(1, 1, 1, 1);
         
        }
        //blind the AI someway somehow
        else
        {

        }
        
    }

    public void Update()
    {
        if (timeRemaining > 0) {
            timeRemaining -= Time.deltaTime;
            //flash the camera
            if (victim is PlayerController)
            {
                if (imageObject.GetComponent<Image>() != null)
                {
                    imageObject.GetComponent<Image>().color = new Color(1, 1, 1, (Mathf.Max(timeRemaining, 0) / duration));
                }
            }
            //blind the AI someway somehow
            else
            {

            }

        } else if (imageObject != null) { 
            Destroy(imageObject);
        }

    }

}
