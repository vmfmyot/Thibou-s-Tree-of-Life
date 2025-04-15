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

        /******************
         *  CONSTRUCTEUR  *
         ******************/
        public Modele()
        {
            var links = new StreamReader(File.OpenRead("../../../data/treeoflife_links.csv"));
            var nodes = new StreamReader(File.OpenRead("../../../data/treeoflife_nodes.csv"));

            Debug.Print(nodes.ReadLine()); //on enleve l'entete

            //Création des nodes
            while (!nodes.EndOfStream)
            {
                string line = nodes.ReadLine();
                //Debug.Print(line);
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
                //Debug.Print(line);
                string[] values = line.Split(',');

                int parentId = Int32.Parse(values[0]);
                int childId = Int32.Parse(values[1]);

                if (this.nodes.ContainsKey(parentId) && this.nodes.ContainsKey(childId))
                {
                    this.nodes[childId].setParentNode(this.nodes[parentId]);
                    this.nodes[parentId].addChild(this.nodes[childId]);
                }
            }
        }

        /****************
         *  GETTERS     *
         ****************/
        public Node getRootNode()
        {  return this.nodes[1]; } //donne la racine de l'arbre
        
        public Dictionary<int, Node> getNodes()
        { return this.nodes; } //donne le dictionnaire de tous les noeuds




        /****************************************************************
         * Classe Node : représente un noeud de l'arbre aka une espèce
         ****************************************************************/
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
            public enum Confidence { CONFIDENT, NOTCONFIDENT, UNKNOWN };

            //Phylesis of the node
            public enum Phylesis { MONOPHYLETIC, UNCERTAIN, NOTMONOPHYLETIC };

            Confidence confidence;
            Phylesis phylesis;

            Node? parentNode;
            ArrayList? children; //list of children NODES (not ids)


            /*********************
            *    CONSTRUCTEUR    *
            **********************/
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

            
            /*******************
            *    ACCESSEURS    *
            ********************/

            public int getId() { return id; }
            public string getName() { return name; }

            public void setParentNode(Node parent) { parentNode = parent; }
            public Node? getParentNode() { return parentNode; }

            public ArrayList getChildren() { return children; }
            public void addChild(Node child) { children.Add(child); }
            public int getNbChildren() { return nbChildren; }
            
            public bool isLeafNode() { return isLeaf; }
            public bool hasWebsiteNode() { return hasWebsite; }
            public bool isExtinctNode() { return extinct; }
            public bool isClusterNode() { return isCluster; }
            
            public Confidence getConfidence() { return confidence; }
            public Phylesis getPhylesis() { return phylesis; }
            
            


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


    }
}
