using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPointControl : MonoBehaviour
{
    public int checkPointOrder;
    public bool checkPointActive;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake()
    {
        MessageText.Instance.GetComponent<Text>().text = "";

        GameObject[] objs = GameObject.FindGameObjectsWithTag("checkpoint");
        int orderCount = 0;
        foreach (GameObject obj in objs)
        {
            if (obj.GetComponent<CheckPointControl>().checkPointOrder == this.checkPointOrder)
            {
                orderCount += 1;
            }
        }
        if (orderCount>1) Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == SausageScript.Instance.midBone.gameObject)
        {
            checkPointActive = true;
            MessageText.Instance.GetComponent<Text>().text = "CHECKPOINT";
            //Debug.Log("Enter Checkpoint");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == SausageScript.Instance.midBone.gameObject)
        {
            MessageText.Instance.GetComponent<Text>().text = "";
            //Debug.Log("Exit Checkpoint");
        }
    }
}
