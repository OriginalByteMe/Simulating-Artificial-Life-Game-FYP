using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Level_transport : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 1f;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            StartCoroutine(LoadMain());
        }
        
    }

    IEnumerator LoadMain()
    {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        //int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        //int nextSceneIndex = currentSceneIndex + 1;
        //if(nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        //{
        //    nextSceneIndex = 0;
        //}
        SceneManager.LoadScene(0);
    }



}
