using System.Collections.Generic;
using System.Linq;
using Assets.FlaExporter.Data.RawData.FrameElements;
using Assets.FlaExporter.Data.RawData.Geom;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.FlaExporter.Editor
{
    public static class FlaShapeProcessor
    {
        public static GameObject ProcessFlaShape(FlaShapeRaw shape)
        {
            foreach (var edgeRaw in shape.Edges)
            {
                ProcessFlaEdge(edgeRaw);
            }
            return  new GameObject();
        }

        private static Mesh ProcessFlaEdge(FlaEdgeRaw edge)
        {
            var mesh = new Mesh();
            var vertices = ProcessFlaEdgeString(edge.Edges);
            Debug.Log("edgeBegin!!===========");
            foreach (var v in vertices)
            {
                Debug.Log(v);
            }
            Debug.Log("edgeEnd!!===========");
            return mesh;
        }

        private static List<Vector2> ProcessFlaEdgeString(string edgeString)
        {
            var list = new List<Vector2>();
            if (edgeString == null || edgeString == "")
                return list;
            edgeString =
                edgeString.Replace("!", " ! ")
                    .Replace("|", " | ")
                    .Replace("/", " | ")
                    .Replace("\\", " | ")
                    .Replace("[", " [ ")
                    .Replace("]", " [ ");
            Debug.Log(edgeString);
            var commands = edgeString.Split(' ');

            for (int i = 0; i < commands.Length; i++)
            {
               
                switch (commands[i])
                {
                    case "!" :
                        list.Add(TryParseFlaVector2(commands[i+1],commands[i+2]));
                        break;

                    case "|":
                        list.Add(TryParseFlaVector2(commands[i + 1], commands[i + 2]));
                        break;

                    case "[":
                        Debug.Log(string.Format("curve not parsed (p:{0},c:{1}", TryParseFlaVector2(commands[i + 1], commands[i + 2]), TryParseFlaVector2(commands[i + 3], commands[i + 4])));
                        break;
                    default:
                        break;
                }
            }
            return list;
        }

        private static Vector2 TryParseFlaVector2(string commandX, string commandY)
        {
            var x = 0;
            int.TryParse(RemoveSymbolsFromCommands(commandX), out x);
            var y = 0;
            int.TryParse(RemoveSymbolsFromCommands(commandY), out y);
            return new Vector2((float)x/20.0f,(float)y/20.0f);
        }

        private static string RemoveSymbolsFromCommands(string command)
        {
            var indexOfS = command.ToLower().IndexOf("s");
            if (indexOfS>0)
                return command.Substring(0,indexOfS);
            return command;
        }



        



    }
}
