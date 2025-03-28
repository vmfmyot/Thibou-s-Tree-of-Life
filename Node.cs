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

        //Confidence in the placement of the node (in the tree)
        enum Confidence { CONFIDENT, NOTCONFIDENT, UNKNOWN };

        //Phylesis of the node
        enum Phylesis { MONOPHYLETIC, UNCERTAIN, NOTMONOPHYLETIC };

        Confidence confidence;
        Phylesis phylesis;

        Node parentNode;
        ArrayList children; //list of children NODES (not ids)


        /****************
        *    METHODS    *
        ****************/
    }
}
