using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GifPlayer : MonoBehaviour
{
    public Sprite[] Frames; 
    public float FramesPerSecond = 10;

    private SpriteRenderer SpriteRenderer;

    void Awake() 
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        int index = (int)((Time.time * FramesPerSecond) % Frames.Length); 
        SpriteRenderer.sprite = Frames[index];
    }
}
