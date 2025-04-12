using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree_of_Life
{
    internal class Modele
    {

        private Dictionary<int, Node> nodes = new Dictionary<int, Node>(); //id -> node
        public class Node
        {
            /****************
            *  ATTRIBUTES  *
            ****************/

            int id;
            string name;
            int nbChildren;
            bool isLeaf;
            bool hasWebsite;
            bool extinct;
            bool isCluster;

            //Confidence in the placement of the node (in the tree)
            enum Confidence { CONFIDENT, NOTCONFIDENT, UNKNOWN };

            //Phylesis of the node
            enum Phylesis { MONOPHYLETIC, UNCERTAIN, NOTMONOPHYLETIC };

            Confidence confidence;
            Phylesis phylesis;

            Node? parentNode;
            ArrayList? children; //list of children NODES (not ids)


            /****************
            *    METHODS    *
            ****************/

            public Node(String[] list)
            {
                id = Int32.Parse(list[0]);
                name = list[1];
                nbChildren = Int32.Parse(list[2]);
                isLeaf = stringToBool(list[3]);
                hasWebsite = stringToBool(list[4]);
                extinct = stringToBool(list[5]);
                confidence = stringToConfidence(list[6]);
                phylesis = stringToPhylesis(list[7]);

                children = new ArrayList();

                isCluster = nbChildren > 5;
            }

            public int getId() { return id; }

            public void setParentNode(Node parent) { parentNode = parent; }
            public void addChild(Node child) { children.Add(child); }

            /** TODO
             */
            private Confidence stringToConfidence(string i)
            {
                switch (i)
                {
                    case "0": return Confidence.UNKNOWN;
                    case "1": return Confidence.CONFIDENT;
                    default: return Confidence.NOTCONFIDENT;
                }
            }

            private Phylesis stringToPhylesis(string i)
            {
                switch (i)
                {
                    case "0": return Phylesis.MONOPHYLETIC;
                    case "1": return Phylesis.UNCERTAIN;
                    default: return Phylesis.UNCERTAIN;
                }
            }

            private Boolean stringToBool(string i)
            {
                switch (i)
                {
                    case "1": return true;
                    default: return false;
                }
            }
        }


        public Modele()
        {
            var links = new StreamReader(File.OpenRead("../../../data/treeoflife_links.csv"));
            var nodes = new StreamReader(File.OpenRead("../../../data/treeoflife_nodes.csv"));

            Debug.Print(nodes.ReadLine()); //on enleve l'entete

            //Création des nodes
            while (!nodes.EndOfStream)
            {
                string line = nodes.ReadLine();
                Debug.Print(line);
                string[] values = line.Split(',');

                if (values.Length == 8)
                {
                    this.nodes.Add(Int32.Parse(values[0]), new Modele.Node(values));
                }
            }

            //création des liens
            links.ReadLine();
            while (!links.EndOfStream)
            {
                string line = links.ReadLine();
                Debug.Print(line);
                string[] values = line.Split(',');

                if (this.nodes.ContainsKey(line[0]) && this.nodes.ContainsKey(line[1]))
                   {
                    this.nodes[line[1]].setParentNode(this.nodes[line[0]]);
                    this.nodes[line[0]].addChild(this.nodes[line[1]]);
                }
            }
        }

    }
}
