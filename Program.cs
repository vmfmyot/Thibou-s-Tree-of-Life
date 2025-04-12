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

        public static Modele.Node? getNode(ArrayList nodeList, int nodeId)
        {
            foreach (Modele.Node node in nodeList)
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

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}