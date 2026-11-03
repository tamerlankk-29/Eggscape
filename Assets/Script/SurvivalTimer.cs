using UnityEngine;

public class SurvivalTimer : MonoBehaviour
{
    public float goal = 60f;
    public Animator gateAnimator;
    public Spawner spawner;

    float t;
    bool won;

    void Update()
    {
        t += Time.deltaTime;
        if (!won && t >= goal)
        {
            won = true;
            Debug.Log("YOU WIN! Выжил " + goal + " сек");
            gateAnimator.SetBool("Open", true);
            spawner.CancelInvoke();
        }
    }
}
