using UnityEngine;

public class ModelView : MonoBehaviour
{
    [SerializeField]
    private GameObject displayedModel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        displayedModel.transform.Rotate(0,.5f,0);
    }
}
