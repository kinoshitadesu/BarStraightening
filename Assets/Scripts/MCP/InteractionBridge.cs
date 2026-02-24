using UnityEngine;
using System.Collections;

namespace MCP
{
    [System.Serializable]
    public class InteractionArgs
    {
        public string target;
        public float[] force;
        public float duration;
    }

    public class InteractionBridge
    {
        public static string Execute(string jsonArgs)
        {
            InteractionArgs args = JsonUtility.FromJson<InteractionArgs>(jsonArgs);
            if (args == null)
            {
                return JsonRpcErrorResponse.Create(-32602, "Invalid arguments");
            }

            PressController pressController = Object.FindObjectOfType<PressController>();
            if (pressController != null && pressController.ramObj != null)
            {
                // Create a temporary GameObject to run the Coroutine
                GameObject coroutineRunner = new GameObject("MCP_PressCoroutineRunner");
                MCPPressSimulator simulator = coroutineRunner.AddComponent<MCPPressSimulator>();
                
                // Fallback duration if not specified
                float dur = args.duration > 0 ? args.duration : 1.0f;
                simulator.SimulatePress(pressController, dur);
                
                return "{\"status\":\"success\"}";
            }

            return JsonRpcErrorResponse.Create(-32000, "PressController or ramObj not found");
        }
    }

    public class MCPPressSimulator : MonoBehaviour
    {
        public void SimulatePress(PressController pc, float duration)
        {
            StartCoroutine(PressRoutine(pc, duration));
        }

        private IEnumerator PressRoutine(PressController pc, float duration)
        {
            // Temporarily disable the original controller to prevent interference with ramObj.position
            bool originalState = pc.enabled;
            pc.enabled = false;

            Transform ram = pc.ramObj;
            float startY = pc.topY;
            float endY = pc.bottomY;
            
            float halfDuration = duration / 2f;
            
            // Move down
            float elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                Vector3 pos = ram.position;
                pos.y = Mathf.Lerp(startY, endY, t);
                ram.position = pos;
                yield return null;
            }

            // Move up
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                Vector3 pos = ram.position;
                pos.y = Mathf.Lerp(endY, startY, t);
                ram.position = pos;
                yield return null;
            }

            // Restore controller
            pc.enabled = originalState;
            Destroy(gameObject); // Cleanup
        }
    }
}
