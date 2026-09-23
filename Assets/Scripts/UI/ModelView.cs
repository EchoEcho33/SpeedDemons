using UnityEngine;

public class ModelView : MonoBehaviour
{
    [SerializeField]
    private GameObject displayedModel;
     void Update()
     {
         displayedModel.transform.Rotate(0,.2f,0);
     }
 }
