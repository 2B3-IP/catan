using System.Collections;
using UnityEngine;

namespace B3.ThiefSystem
{
    internal sealed class ThiefController : ThiefControllerBase
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float stoppingDistance = 0.01f;
        public override IEnumerator MoveThief(Vector3 endPosition)
        {
            Debug.Log("moving thief");
            transform.position = endPosition;
            yield break;
            while (Vector3.Distance(transform.position, endPosition) > stoppingDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, endPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = endPosition;
        }
    }
}