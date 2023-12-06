using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A singleton in charge of all game events. Game objects use this as a middleman
/// to communicate to other classes that they would otherwise be agnostic of.
/// </summary>
public class EventManager : MonoBehaviour {

    public static UnityEvent<int> OnPlayerHealthChanged = new UnityEvent<int>();

    public static void ModifyPlayerHealth(int amount) {
        OnPlayerHealthChanged.Invoke(amount);
    }
}
