using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageText : MonoBehaviour
{

    //Singleton instance
    private static MessageText instance;
    public static MessageText Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<MessageText>();
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
