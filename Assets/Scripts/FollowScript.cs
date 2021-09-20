using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float y = SausageScript.Instance.midBone.transform.position.y;
        if (y < 0) y = 0;
        transform.position = new Vector3(SausageScript.Instance.midBone.transform.position.x, y, SausageScript.Instance.midBone.transform.position.z);
    }
}
