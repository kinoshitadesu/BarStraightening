using UnityEngine;

namespace MCP
{
    [System.Serializable]
    public class ViewArgs
    {
        public string view;
    }

    public class ViewController
    {
        private static string currentView = "side";

        public static string Execute(string jsonArgs)
        {
            ViewArgs args = JsonUtility.FromJson<ViewArgs>(jsonArgs);
            if (args == null || string.IsNullOrEmpty(args.view))
            {
                return JsonRpcErrorResponse.Create(-32602, "Invalid arguments");
            }

            Camera cam = Camera.main;
            if (cam == null) return JsonRpcErrorResponse.Create(-32000, "Main Camera not found");

            switch (args.view.ToLower())
            {
                case "front":
                    cam.transform.position = new Vector3(0, 5, -10);
                    cam.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case "side":
                    cam.transform.position = new Vector3(10, 5, 0);
                    cam.transform.rotation = Quaternion.Euler(0, -90, 0);
                    break;
                case "top":
                    cam.transform.position = new Vector3(0, 10, 0);
                    cam.transform.rotation = Quaternion.Euler(90, 0, 0);
                    break;
                case "iso":
                    cam.transform.position = new Vector3(7, 7, -7);
                    cam.transform.rotation = Quaternion.Euler(30, -45, 0);
                    break;
                default:
                    return JsonRpcErrorResponse.Create(-32602, "Unknown view type");
            }
            
            currentView = args.view.ToLower();
            return "{\"status\": \"success\"}";
        }

        public static string GetCurrentViewName()
        {
            return currentView;
        }
    }
}
