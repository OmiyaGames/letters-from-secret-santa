using UnityEngine;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    public enum Effect
    {
        AllSwitchesOn,
        ToggleSwitches,
        AllItemSlots
    }

    [Header("Required Components")]
    public Effect doorOpenEffect;
    [Header("Animation Fields")]
    public string onKey = "default";
    public string offKey = "off";

    bool isOpen = false;
    readonly List<Switch> allSwitchesToTrigger = new List<Switch>();
    readonly HashSet<Switch> triggeredSwitches = new HashSet<Switch>();
    readonly List<ItemSlot> allItemSlotsToTrigger = new List<ItemSlot>();
    readonly HashSet<ItemSlot> triggeredSlots = new HashSet<ItemSlot>();
    readonly Dictionary<string, List<DoorAnimation>> allAnimations = new Dictionary<string, List<DoorAnimation>>();

    public bool IsOpen
    {
        get
        {
            return isOpen;
        }
        private set
        {
            if(isOpen != value)
            {
                isOpen = value;
            }
        }
    }

    void Start()
    {
        // Grab all door animations
        List<DoorAnimation> doorList = null;
        DoorAnimation[] allDoorAnimations = GetComponentsInChildren<DoorAnimation>();
        foreach(DoorAnimation doorAnimation in allDoorAnimations)
        {
            if (doorAnimation.gameObject.activeSelf == true)
            {
                // Attempt to retrieve the list of doors for this door's key
                if (allAnimations.TryGetValue(doorAnimation.doorAnimationKey, out doorList) == false)
                {
                    // If there isn't any, create a new list
                    doorList = new List<DoorAnimation>();

                    // Add the new list to the dictionary
                    allAnimations.Add(doorAnimation.doorAnimationKey, doorList);
                }

                // Add the door to the door list
                doorList.Add(doorAnimation);
            }
        }
    }

    public void AddSwitch(Switch switchInstance)
    {
        if(switchInstance != null)
        {
            allSwitchesToTrigger.Add(switchInstance);
        }
    }

    public void AddItemSlot(ItemSlot itemSlotInstance)
    {
        if (itemSlotInstance != null)
        {
            allItemSlotsToTrigger.Add(itemSlotInstance);
        }
    }

    public void OnItemSlotChanged(ItemSlot trigger)
    {
        // Check the door effect
        if (doorOpenEffect == Effect.AllItemSlots)
        {
            if (trigger.heldItem != null)
            {
                // Add this switch to the hash set
                triggeredSlots.Add(trigger);
            }
            else
            {
                // Simply remove the switch from the hash set
                triggeredSlots.Remove(trigger);
            }

            // Check if all switches are on
            string searchList = offKey;
            if (triggeredSlots.Count >= allItemSlotsToTrigger.Count)
            {
                searchList = onKey;
            }

            // Attempt to retrieve the list of doors for this switch's key
            List<DoorAnimation> doorList = null;
            if (allAnimations.TryGetValue(searchList, out doorList) == true)
            {
                // If successful, go through all animations
                foreach (DoorAnimation doorAnimation in doorList)
                {
                    // Run the animation
                    doorAnimation.RunAnimation(this, trigger);
                }
            }
        }
    }

    public void OnSwitchTriggerChanged(Switch trigger)
    {
        // Check the door effect
        if (doorOpenEffect == Effect.ToggleSwitches)
        {
            OnSwitchToggleChanged(trigger);
        }
        else if (doorOpenEffect == Effect.AllSwitchesOn)
        {
            OnSwitchOnChanged(trigger);
        }
    }

    public void OnSwitchToggleChanged(Switch trigger)
    {
        if (trigger.IsTriggered == true)
        {
            // Toggle all other switches as off
            foreach (Switch switchInstance in triggeredSwitches)
            {
                switchInstance.IsTriggered = false;
            }

            // Clear out the hash set
            triggeredSwitches.Clear();

            // Attempt to retrieve the list of doors for this switch's key
            List<DoorAnimation> doorList = null;
            if (allAnimations.TryGetValue(trigger.doorAnimationKey, out doorList) == true)
            {
                // If successful, go through all animations
                foreach(DoorAnimation doorAnimation in doorList)
                {
                    // Run the animation
                    doorAnimation.RunAnimation(this, trigger);
                }
            }

            // Add this switch to the hash set
            triggeredSwitches.Add(trigger);
        }
        else
        {
            // Simply remove the switch from the hash set
            triggeredSwitches.Remove(trigger);
        }
    }

    public void OnSwitchOnChanged(Switch trigger)
    {
        if (trigger.IsTriggered == true)
        {
            // Add this switch to the hash set
            triggeredSwitches.Add(trigger);
        }
        else
        {
            // Simply remove the switch from the hash set
            triggeredSwitches.Remove(trigger);
        }

        // Check if all switches are on
        if (triggeredSwitches.Count >= allSwitchesToTrigger.Count)
        {
            // Attempt to retrieve the list of doors for this switch's key
            List<DoorAnimation> doorList = null;
            if (allAnimations.TryGetValue(onKey, out doorList) == true)
            {
                // If successful, go through all animations
                foreach (DoorAnimation doorAnimation in doorList)
                {
                    // Run the animation
                    doorAnimation.RunAnimation(this, trigger);
                }
            }
        }
    }
}
