using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Keeps your standard legacy text link perfect!

public class MeatCollectable : MonoBehaviour
{
    public GameObject Portal;
    public int meat;
    public Text MeatText;

    // Automated Interface Bridge Reference
    private PlayerMovement playerMovementRef;

    void Start()
    {
        // Instantly locates your active werewolf object right on startup
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerMovementRef = playerObj.GetComponent<PlayerMovement>();
        }
    }

    void Update()
    {
        // Forces the UI to pull its numbers directly from the wolf's script!
        if (playerMovementRef != null)
        {
            meat = playerMovementRef.Meat != null ? playerMovementRef.Meat.meat : meat;
        }

        // Prints updated counter string cleanly onto monitor screen layout
        if (MeatText != null)
        {
            MeatText.text = "Meat Count: " + meat.ToString();
        }

        // Back-Up Portal Trigger Sequence
        if (meat >= 6)
        {
            if (Portal != null && !Portal.activeSelf)
            {
                Portal.SetActive(true);
            }
        }
    }
}



