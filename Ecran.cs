using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree_of_Life
{
    internal class Ecran
    {
        Modele modele;
        ZoneArbre arbre;

        public Ecran(Modele modele)
        {
            this.modele = modele;
        }


        
        //ECRAN DE L'ARBRE -------------------------------------------------------------------
        public class ZoneArbre : Control
        {
            private Modele modele;
            private bool repaint=true;
            private Dictionary<Modele.Node, Point> positions;
            private ArrayList displayed;

            public ZoneArbre(Modele modele) : base()
            {
                this.modele = modele;
                this.Location = new System.Drawing.Point(0, 0);
                this.Size = new System.Drawing.Size(1000, 800);
                this.BackColor = Color.White;
                positions = new Dictionary<Modele.Node, Point>();
                displayed = new ArrayList();
            }

            public void drawNode(Modele.Node node, int x, int y, PaintEventArgs pevent)
            {
                Brush b = new SolidBrush(Color.Orange);

                if (node.isClusterNode())
                {
                    pevent.Graphics.FillEllipse(b, x, y, 200, 200);
                } else
                {
                    pevent.Graphics.FillRectangle(b, x, y, 150, 150);
                    //draw the node in the middle of the ZoneArbre
                    Label nodeLabel = new Label();
                    nodeLabel.Text = node.getName();
                    //make it BIG and VISIBLE
                    nodeLabel.Font = new Font("Arial", 15, FontStyle.Bold);
                    nodeLabel.ForeColor = Color.Black;
                    nodeLabel.BackColor = Color.Transparent;
                    nodeLabel.TextAlign = ContentAlignment.MiddleCenter;
                    //set the size of the label
                    nodeLabel.Size = new Size(100, 50);
                    //set the location of the label in the middle of the ZoneArbre
                    nodeLabel.Location = new Point(x, y+5);
                    nodeLabel.AutoSize = true;
                    nodeLabel.Visible = true;
                    this.Controls.Add(nodeLabel);

                    //extinct or not
                    //draw the node in the middle of the ZoneArbre
                    Label extinctLabel = new Label();
                    if(node.isExtinctNode())
                    {
                        extinctLabel.Text = "Espèce éteinte";
                    }
                    else
                    {
                        extinctLabel.Text = "Espèce en vie";
                    }
                    //make it BIG and VISIBLE
                    extinctLabel.Font = new Font("Arial", 10);
                    extinctLabel.ForeColor = Color.Black;
                    extinctLabel.BackColor = Color.Transparent;
                    extinctLabel.TextAlign = ContentAlignment.MiddleCenter;
                    //set the size of the label
                    extinctLabel.Size = new Size(150, 50);
                    //set the location of the label in the middle of the ZoneArbre
                    extinctLabel.Location = new Point(x, y+40);
                    extinctLabel.Visible = true;
                    this.Controls.Add(extinctLabel);
                }

            }

            public void calculTree(Modele.Node node, Point p)
            {
                if(positions.ContainsKey(node))
                    positions[node] = p ;
                else
                    positions.Add(node, p);
                
                if( !node.isClusterNode() && (node.getChildren != null || node.getChildren().Count > 0) )
                {
                    Debug.Print("NOM : " + node.getName() + " is NOT a cluster !!");
                    int depart = p.X - (int)(node.getNbChildren() / 2) * 100;
                    
                    foreach (Modele.Node child in node.getChildren())
                    {
                        Debug.Print("   CHILD : " + child.getName());
                        //calculate the position of the child
                        Point childPos = new Point(depart, p.Y - 100);
                        //recursively calculate the tree for the child
                        calculTree(child, childPos);
                        depart += 100;
                    }

                } else Debug.Print("NOM : " + node.getName() + " is a cluster....");
            }

            protected override void OnPaintBackground(PaintEventArgs pevent)
            {
                if (repaint)
                {
                    //on efface l'aire de dessin
                    pevent.Graphics.Clear(Color.White);

                    calculTree(this.modele.getRootNode(), new Point(400, 500));
                    foreach (Modele.Node node in positions.Keys)
                    {
                        //draw the node
                        drawNode(node, positions[node].X, positions[node].Y, pevent);
                    }

                    //print every key value of positions
                    foreach (KeyValuePair<Modele.Node, Point> kvp in positions)
                    {
                        Debug.Print(kvp.Key.getName() + " : " + kvp.Value.X + ", " + kvp.Value.Y);
                    }

                }
            }
        }





        //ECRAN MENU -------------------------------------------------------------------
        public class ZoneMenu : Control
        {
            private Modele modele;
            public ZoneMenu(Modele modele) : base()
            {
                this.modele = modele;
                this.Location = new System.Drawing.Point(1000, 0);
                this.Size = new System.Drawing.Size(400, 800);
                this.BackColor = Color.LightGray;
            }
        }
    }
}
