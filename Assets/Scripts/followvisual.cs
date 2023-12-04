using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class followvisual : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform visualTarget;
    private bool freeze = false;
    private Vector3 initialLocalPos;

    public float followAngleThreshold;
    public float resetSpeed = 5;
    public Vector3 localAxis;
    private Vector3 offset;
    private Transform pokeAttachTransform;
    private XRBaseInteractable interactable;
    private bool isFollowing = false;
    
    void Start()
    {
        initialLocalPos = visualTarget.localPosition;
        interactable = GetComponent<XRBaseInteractable>();
        interactable.hoverEntered.AddListener(Follow);
        interactable.hoverExited.AddListener(Reset);
        interactable.selectEntered.AddListener(Freeze);
    }

    public void Follow(BaseInteractionEventArgs hover)
    {
        if(hover.interactorObject is XRPokeInteractor) {
            XRPokeInteractor interactor = (XRPokeInteractor)hover.interactorObject;

            pokeAttachTransform = interactor.attachTransform;
            offset = visualTarget.position - pokeAttachTransform.position;

            float pokeAngle = Vector3.Angle(offset, visualTarget.TransformDirection(localAxis));

            if (pokeAngle < followAngleThreshold) {
                isFollowing = true;
                freeze = false;
            }
        }
     }

     public void Freeze(BaseInteractionEventArgs hover) {
        if (hover.interactorObject is XRPokeInteractor) {
            freeze = true;
        }
     }

     public void Reset(BaseInteractionEventArgs hover)
     {
        if(hover.interactorObject is XRPokeInteractor) {
            isFollowing = false;
            freeze = false;
            SceneManager.LoadScene("TestMovement");
        }
     }

    // Update is called once per frame
    void Update()
    {
        if (freeze) {
            return;
        }
        if(isFollowing) {
            Vector3 localTargetPosition = visualTarget.InverseTransformPoint(pokeAttachTransform.position + offset);
            Vector3 constrainedLocalTargetPosition = Vector3.Project(localTargetPosition, localAxis);

            visualTarget.position = visualTarget.TransformPoint(constrainedLocalTargetPosition);
        }
        else {
            visualTarget.localPosition = Vector3.Lerp(visualTarget.localPosition, initialLocalPos, Time.deltaTime * resetSpeed);
        }
    }
}
