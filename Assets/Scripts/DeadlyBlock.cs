using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyBlock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == SausageScript.Instance.midBone || collision.gameObject == SausageScript.Instance.upperBone || collision.gameObject == SausageScript.Instance.lowerBone)
        {
            SausageScript.Instance.gameOver = true;            
        }
    }
}
