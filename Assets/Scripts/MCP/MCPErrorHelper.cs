using UnityEngine;

namespace MCP
{
    [System.Serializable]
    public class JsonRpcErrorResponse
    {
        public string jsonrpc = "2.0";
        public RpcError error;
        
        [System.Serializable]
        public class RpcError
        {
            public int code;
            public string message;
        }

        public static string Create(int code, string message)
        {
            var res = new JsonRpcErrorResponse
            {
                error = new RpcError { code = code, message = message }
            };
            return JsonUtility.ToJson(res);
        }
    }
}
