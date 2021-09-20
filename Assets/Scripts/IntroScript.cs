using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(new Vector3(0, 0.7f, 0), Vector3.up, 100 * Time.deltaTime);
    }

    public void StarGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
