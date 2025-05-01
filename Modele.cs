using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree_of_Life
{
    public class Modele
    {

        private Dictionary<int, Node> nodes = new Dictionary<int, Node>(); //id -> node
        private Dictionary<string, Node> speciesList = new Dictionary<string, Node>();
        public Dictionary<string, Node> getSpeciesList() { return this.speciesList; }


        /******************
         *  CONSTRUCTEUR  *
         ******************/
        public Modele()
        {
            var links = new StreamReader(File.OpenRead("../../../data/treeoflife_links.csv"));
            var nodes = new StreamReader(File.OpenRead("../../../data/treeoflife_nodes.csv"));

            nodes.ReadLine();
            //Création des nodes
            while (!nodes.EndOfStream)
            {
                string line = nodes.ReadLine();
                string[] values = line.Split(',');

                //Dans le csv il ya des nodes dont le nom contient une virgule qui est également le séparateur du csv
                //Cela créé des problèmes donc on a décidé d'ignorer ces noeuds la
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
                string[] values = line.Split(',');

                int parentId = Int32.Parse(values[0]);
                int childId = Int32.Parse(values[1]);

                if (this.nodes.ContainsKey(parentId) && this.nodes.ContainsKey(childId))
                {
                    this.nodes[childId].setParentNode(this.nodes[parentId]);
                    this.nodes[parentId].addChild(this.nodes[childId]);
                }
            }

            speciesList = new Dictionary<string, Node>();

            foreach (Node n in this.nodes.Values)
            {
                //n.setPath(n);
                n.countChildren(n);
                n.setShade();
                if (!speciesList.ContainsKey(n.getName()))
                speciesList.Add(n.getName(), n);
            }

            
        }

        /****************
         *  GETTERS     *
         ****************/
        public Node getRootNode()
        {  return this.nodes[1]; } //donne la racine de l'arbre
           

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
            int nbChildren=-1;
            int nbExtinctChildren = -1;
            bool isLeaf;
            bool hasWebsite;
            bool extinct;
            bool isCluster;

            private Color shade;
            private ArrayList path; //path to the root node with the IDs

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
                isLeaf = stringToBool(list[3]);
                hasWebsite = stringToBool(list[4]);
                extinct = stringToBool(list[5]);
                confidence = stringToConfidence(list[6]);
                phylesis = stringToPhylesis(list[7]);

                children = new ArrayList();
                path = new ArrayList();

                isCluster = Int32.Parse(list[2]) > 4;

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
            public void setNbChildren(int n) {  nbChildren = n; }

            public int getNbExtinctChildren() {  return nbExtinctChildren; }
            public void setNbExtinctChildren(int n) { nbExtinctChildren = n; }

            
            public bool isLeafNode() { return isLeaf; }
            public bool hasWebsiteNode() { return hasWebsite; }
            public bool isExtinctNode() { return extinct; }
            public bool isClusterNode() { return isCluster; }
            public Phylesis getPhylesis() { return phylesis; }
            public Color getShade() { return shade; }


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


            public void countChildren(Node n)
            {
                if(n.getNbChildren() == -1 )
                {
                    int count = 0;
                    int countExtinct = 0;
                    foreach( Node child in n.getChildren())
                    {
                        countChildren(child);
                        count += child.getNbChildren()+1;
                        if (child.isExtinctNode()) {countExtinct += child.getNbExtinctChildren() + 1; }
                        else { countExtinct += child.getNbExtinctChildren(); }
                    }
                    n.setNbChildren(count);
                    n.setNbExtinctChildren(countExtinct);
                }
            }


            public void setShade()
            {
                if (isCluster)
                {
                    int green = Convert.ToInt32(200 * (1-Decimal.Divide(nbExtinctChildren, nbChildren)));
                    shade = Color.FromArgb(200, 0, green, 0);
                }
                else
                {
                    if (extinct)
                    {
                        shade= Color.FromArgb(255, 0, 0, 0); //black
                    }
                    else
                        shade = Color.FromArgb(255, 0, 200, 0); //green
                }
            }
        }


    }
}
