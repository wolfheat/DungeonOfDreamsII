using System.Collections.Generic;
using UnityEngine;
using Wolfheat.StartMenu;

public class FireStormSpawner : MonoBehaviour
{
    private float stepTimer = 0;
    private const float StepTime = 0.3f;

    private int maxSteps = 3;
    private int stepDistance = -1;

    List<List<Vector2Int>> positions;


    public void InitiateAt(Vector2Int pos)
    {
        // Start position for the firestorm
        positions = GetPositionList(pos);
    }

    private List<List<Vector2Int>> GetPositionList(Vector2Int pos)
    {
        List<List<Vector2Int>> positionList = new List<List<Vector2Int>>();
        positionList.Add(new List<Vector2Int>());
        positionList.Add(new List<Vector2Int>());
        positionList.Add(new List<Vector2Int>());

        for (int i = -3; i<4; i++) {
            for (int j = -3; j < 4; j++) {
                if (i == 0 && j == 0) continue;
                Vector2Int newPos = pos + new Vector2Int(i, j);
                int maxDist = Mathf.Max(Mathf.Abs(i), Mathf.Abs(j));
                //Debug.Log("Added ["+newPos.x+","+newPos.y+"] to list index "+(maxDist-1));
                positionList[maxDist-1].Add(newPos);
                //Debug.Log("YES");
            }
        }
        return positionList;


    }

    void Update()
    {
        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0)
        {
            stepTimer += StepTime;
            Spawn();
        }
    }

    private void Spawn()
    {
        // Step away one step
        stepDistance++;

        // If to far away destroy the spawner
        if(stepDistance >= maxSteps) { 
            Destroy(gameObject);
            return;
        }

        SpawnAll(stepDistance);

    }

    private void SpawnAll(int stepDistance)
    {
        foreach(var pos in positions[stepDistance]) {
            Vector3 aligned = Convert.V2IntToV3(pos);
            
            Debug.Log("Spawning Firestorm at "+aligned);

            if (LevelCreator.Instance.TargetHasWall(aligned)) continue; // Skips if creating in wall
            //ParticleEffects.Instance.PlayTypeAt(ParticleType.WildFire, aligned);
            ParticleEffects.Instance.PlayTypeAt(ParticleType.WildFire, aligned+Vector3.down*0.48f);
            Explosion.Instance.FireDamage(aligned);
        }
        
    }
}
