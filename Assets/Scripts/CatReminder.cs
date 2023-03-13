using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatReminder : MonoBehaviour
{

    [SerializeField] private GameObject _cat;

    private void Awake() =>
        StartCoroutine(CoroutineWhatever());

    private IEnumerator CoroutineWhatever()
    {
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => GameInput.GetPlayerActions().enabled);
        yield return new WaitForSeconds(Random.Range(2f, 5f));
        _cat.SetActive(true);
    }
}
