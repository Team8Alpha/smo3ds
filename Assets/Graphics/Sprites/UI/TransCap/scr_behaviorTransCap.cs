using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/*
public class scr_behaviorTransCap : MonoBehaviour {

	bool isFlyingIn = false;
	SpriteRenderer sprRender;
	// Use this for initialization
	void OnEnable () {
		if (!isFlyingIn) {
			sprRender.color = new Color (0,0,0,0);
			scr_main.s.SetFocus(false);
			transform.localScale = new Vector3 (20, 20*0.95f, 0);
		} else {
			transform.localScale = new Vector3 (0, 0, 0);
		}
	}
	void Awake(){
		sprRender = gameObject.GetComponent<SpriteRenderer>();
		gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if(!isFlyingIn){
			transform.localScale -= new Vector3(1f, 1f, 0);
			sprRender.color = new Color (0,0,0,gameObject.GetComponent<SpriteRenderer>().color.a + 0.1f);
			if(transform.localScale.x <0.2f){
				isFlyingIn = true;
				SceneManager.LoadScene (scr_loadScene.s.nextScene);
				scr_main.s.SetFocus(true);
			}
		} else if(scr_main.s.hasLevelLoaded){
			transform.localScale += new Vector3(3f, 3f, 0);
			if(transform.localScale.x > 20){
				isFlyingIn = false;
				gameObject.SetActive (false);
			}
		}
	}
}*/

public class scr_behaviorTransCap : MonoBehaviour
{
    SpriteRenderer mSprite;
    byte iSprNum = 0;
    [SerializeField] Sprite[] img;

    bool isOpening = false;
    float scaleFactor = 0.1f; // Change this scale factor as per requirement
    Vector2 vOrigin;
    float originalAspectRatio;

    void Awake()
    {
        mSprite = gameObject.GetComponent<SpriteRenderer>();

        vOrigin = new Vector2(transform.localScale.x, transform.localScale.y);

        // Calculate the original aspect ratio
        originalAspectRatio = vOrigin.y / vOrigin.x;

        transform.parent.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (!isOpening)
        {
            transform.localScale = new Vector3(vOrigin.x, vOrigin.y, 1);
        }
        else
        {
            transform.localScale = new Vector3(0, 0, 1);
        }
    }

    void Update()
    {
        if (iSprNum >= img.Length) iSprNum = 0;
        mSprite.sprite = img[iSprNum];
        iSprNum++;

        if (!isOpening)
        {
            // Maintain aspect ratio while reducing the scale
            float newScaleX = Mathf.Max(transform.localScale.x - scaleFactor, 0);
            float newScaleY = newScaleX * originalAspectRatio;

            if (newScaleX <= 0)
            {
                isOpening = true;
                transform.localScale = new Vector3(0, 0, 1);
                SceneManager.LoadScene(scr_loadScene.s.nextScene);
            }
            else
            {
                transform.localScale = new Vector3(newScaleX, newScaleY, 1);
            }
        }
        else if(scr_main.s.hasLevelLoaded)
        {
            scr_main.s.SetFocus(true);
            // Maintain aspect ratio while increasing the scale
            float newScaleX = Mathf.Min(transform.localScale.x + scaleFactor * 2, vOrigin.x + 20);
            float newScaleY = newScaleX * originalAspectRatio;

            if (newScaleX >= vOrigin.x)
            {
                transform.parent.gameObject.SetActive(false);
                isOpening = false;
                transform.localScale = new Vector3(vOrigin.x, vOrigin.y, 1);
            }
            else
            {
                transform.localScale = new Vector3(newScaleX, newScaleY, 1);
            }
        }
    }
}
