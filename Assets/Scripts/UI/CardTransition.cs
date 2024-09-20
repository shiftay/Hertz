using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTransition : MonoBehaviour
{
    public Animator animator;
    public List<CardUI> cards;

    public void RandomizeAndShow() {
        // Randomize the Angle of the object
        transform.rotation.eulerAngles.Set(0,0,Random.Range(-25,25));
        // Randomize The cards
        cards.ForEach(n => n.SetImage(GameManager.instance.dealer.spriteHandler.RandomCard()));
        // Play the animation
        animator.SetTrigger("Display");
    }

    public void Remove() {
        animator.SetTrigger("Hide");
    }
}
