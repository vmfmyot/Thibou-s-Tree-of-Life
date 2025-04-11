using System.Diagnostics;
using System.Collections;


namespace Tree_of_Life
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]

        public static Node? getNode(ArrayList nodeList, int nodeId)
        {
            foreach (Node node in nodeList)
            {
                if (node.getId() == nodeId) 
                {
                    return node; 
                } 
            }
            return null;
        }

        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            var links = new StreamReader(File.OpenRead("../../../data/treeoflife_links.csv"));
            var nodes = new StreamReader(File.OpenRead("../../../data/treeoflife_nodes.csv"));

            Debug.Print(nodes.ReadLine()); //on enleve l'entete

            //Création des nodes
            ArrayList nodesList = new ArrayList();
            while (!nodes.EndOfStream){
                string line = nodes.ReadLine();
                Debug.Print(line);
                string[] values = line.Split(',');

                if (values.Length == 8)
                {
                    nodesList.Add(new Node(values));
                }
            }

            //création des liens
            links.ReadLine();
            while (!links.EndOfStream)
            {
                string line = links.ReadLine();
                Debug.Print(line);
                string[] values = line.Split(',');

                Node source=getNode(nodesList, line[0]);
                Node target= getNode(nodesList, line[1]);
                if (source != null && target != null)
                {
                    source.setParentNode(target);
                    target.addChild(source);
                }
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}