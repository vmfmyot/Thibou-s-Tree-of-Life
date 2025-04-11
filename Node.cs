using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree_of_Life
{
    internal class Node
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

        Confidence  confidence;
        Phylesis phylesis;

        Node? parentNode;
        ArrayList? children; //list of children NODES (not ids)


        /****************
        *    METHODS    *
        ****************/

        public Node(String[] list){
            id = Int32.Parse(list[0]);
            name= list[1];
            nbChildren= Int32.Parse(list[2]);
            isLeaf= stringToBool(list[3]);
            hasWebsite= stringToBool(list[4]);
            extinct= stringToBool(list[5]);
            confidence = stringToConfidence(list[6]);
            phylesis = stringToPhylesis(list[7]);

            children=new ArrayList();

            isCluster= nbChildren > 5;
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
}
