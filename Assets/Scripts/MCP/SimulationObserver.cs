using UnityEngine;
using System.Collections.Generic;

namespace MCP
{
    [System.Serializable]
    public class SimulationState
    {
        public float score;
        public RodParameters rod_parameters;
        public string view;
    }

    [System.Serializable]
    public class RodParameters
    {
        public float length;
        public float thickness;
        public float[] deformation;
    }

    public class SimulationObserver
    {
        public static string Execute(string jsonArgs)
        {
            SimulationState state = new SimulationState();
            
            // TODO: 正確なスコア計算は未実装のため、ひとまずランダムな値を返す
            state.score = Random.Range(0.0f, 1.0f);
            
            state.rod_parameters = new RodParameters();
            
            ShaftController shaft = Object.FindObjectOfType<ShaftController>();
            if (shaft != null)
            {
                state.rod_parameters.length = shaft.transform.localScale.z; 
                state.rod_parameters.thickness = shaft.transform.localScale.x;

                List<float> defs = new List<float>();
                foreach(var bp in shaft.bonePoints)
                {
                    defs.Add(bp.plasticBendX + bp.elasticBendX);
                }
                state.rod_parameters.deformation = defs.ToArray();
            }
            else
            {
                state.rod_parameters.length = 1.0f;
                state.rod_parameters.thickness = 0.05f;
                state.rod_parameters.deformation = new float[] { 0.0f, 0.0f, 0.0f };
            }
            
            state.view = ViewController.GetCurrentViewName(); 

            return JsonUtility.ToJson(state);
        }
    }
}
