using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FloatingText : MonoBehaviour, IPooledObject {
    public Animator animator;
    private Text damageText;

    void OnEnable() {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        //Destroy(gameObject, clipInfo[0].clip.length);
        
        damageText = animator.GetComponent<Text>();
        StartCoroutine(ActivationRoutine(clipInfo[0].clip.length));
    }

    public void setText(string text) {
        damageText.text = text;
    }
    private IEnumerator ActivationRoutine(float length) {
        yield return new WaitForSeconds(length);
        gameObject.SetActive(false);
    }

    public void onObjectSpawn(Transform location, string text, GameObject canvas) {
        gameObject.SetActive(true);
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location.position);
        transform.SetParent(canvas.transform, false);
        transform.position = screenPosition;
        setText(text);
    }
}
