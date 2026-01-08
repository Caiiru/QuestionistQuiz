using System;
using System.Collections;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public Transform LoadingScreenTransform;
    public float AwaitTime;
    private void OnEnable()
    {
        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        yield return new WaitForSeconds(AwaitTime);
        LoadingScreenTransform.gameObject.SetActive(false);
    }
}
