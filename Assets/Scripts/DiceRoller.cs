using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DiceRoller : MonoBehaviour
{
    public Rigidbody cube1;
    public Rigidbody cube2;

    public float minF = 5f;
    public float maxF = 11f;
    public float minT = 4f;
    public float maxT = 13f;

    public Vector3 dir = new Vector3(0, 2, 1);

    public float velStop = 0.05f;
    public float angStop = 0.05f;
    public float restTime = 0.3f;

    bool rolling = false;
    Dictionary<Rigidbody, float> timers = new Dictionary<Rigidbody, float>();

    void Start()
    {
        timers[cube1] = 0;
        timers[cube2] = 0;
    }

    public void OnRoll(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;
        if (rolling) return;

        rolling = true;
        timers[cube1] = 0;
        timers[cube2] = 0;

        ThrowCube(cube1);
        ThrowCube(cube2);

        Debug.Log("все работает");
    }

    void ThrowCube(Rigidbody rb)
    {
        if (rb == null)
        {
            Debug.Log("кубик не обнаружен");
            return;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        float f = Random.Range(minF, maxF);
        float t = Random.Range(minT, maxT);

        Vector3 d = new Vector3(
            Random.Range(-0.5f, 0.5f),
            2f,
            Random.Range(0.8f, 1.2f)
        );

        rb.AddForce(d.normalized * f, ForceMode.Impulse);
        rb.AddTorque(Random.onUnitSphere * t, ForceMode.Impulse);
    }

    void Update()
    {
        if (!rolling) return;

        UpdateTimer(cube1);
        UpdateTimer(cube2);

        bool stop1 = timers[cube1] >= restTime;
        bool stop2 = timers[cube2] >= restTime;

        if (stop1 && stop2)
        {
            rolling = false;
            int a = GetTop(cube1.gameObject);
            int b = GetTop(cube2.gameObject);
            int sum = a + b;
            Debug.Log(" первый куб получил " + a + " второй куб получил " + b + " сумма " + sum);
        }
    }

    void UpdateTimer(Rigidbody rb)
    {
        if (rb.linearVelocity.magnitude <= velStop && rb.angularVelocity.magnitude <= angStop)
        {
            timers[rb] += Time.deltaTime;
        }
        else
        {
            timers[rb] = 0;
        }
    }

    int GetTop(GameObject die)
    {
        int top = 1;
        float best = -999f;
        for (int i = 1; i <= 6; i++)
        {
            Transform f = die.transform.Find("Face" + i);
            if (f == null) continue;
            float d = Vector3.Dot(f.up, Vector3.up);
            if (d > best)
            {
                best = d;
                top = i;
            }
        }
        return top;
    }
}