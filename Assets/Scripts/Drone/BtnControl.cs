using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ToHome()
    {
        SceneManager.LoadScene("HomeScene");
    }
    public void ToAutoFlyWithBlock(){
        SceneManager.LoadScene("MLSceneCurriculum");
    }
    public void ToAutoFlyWithNoBlock()
    {
        SceneManager.LoadScene("MLSceneCurriculum 1"); 
    }
    public void ToManual()
    {
        SceneManager.LoadScene("MLSceneCurriculum 2"); 
    }
}
