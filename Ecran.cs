using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ComponentModel.Design.ObjectSelectorEditor;

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
        public class ZoneArbre : ScrollableControl
        {
            private Modele modele;
            private bool repaint=true;
            private Dictionary<Modele.Node, Point> positions;
            private ArrayList displayed;
            private ArrayList links;
            
            //Constantes de taille
            public static int tailleNode = 125;
            public static int espaceNodeX = 175;
            public static int espaceNodeY = 250;


            public ZoneArbre(Modele modele) : base()
            {
                this.modele = modele;
                this.Location = new System.Drawing.Point(0, 0);
                this.Size = new System.Drawing.Size(1000, 800);
                this.BackColor = Color.White;
                positions = new Dictionary<Modele.Node, Point>();
                displayed = new ArrayList();
                links = new ArrayList();
                this.AutoScroll = true;
            }

            /*
             * Dessine un noeud
             * @param node : le noeud à dessiner
             * @param x : la position x du noeud
             * @param y : la position y du noeud
             * @param pevent : l'événement de peinture
             */
            public void drawNode(Modele.Node node, int x, int y, PaintEventArgs pevent)
            {
                Brush b = new SolidBrush(Color.Orange);

                if (node.isClusterNode())
                {
                    pevent.Graphics.FillEllipse(b, x, y, tailleNode, tailleNode);

                    //draw the node in the middle of the ZoneArbre
                    Label nodeLabel = new Label();
                    nodeLabel.Text = node.getName();
                    //make it BIG and VISIBLE
                    nodeLabel.Font = new Font("Arial", 10, FontStyle.Bold);
                    nodeLabel.ForeColor = Color.Black;
                    nodeLabel.BackColor = Color.Transparent;
                    nodeLabel.TextAlign = ContentAlignment.MiddleCenter;
                    //set the size of the label
                    nodeLabel.Size = new Size(100, 50);
                    //set the location of the label in the middle of the ZoneArbre
                    nodeLabel.Location = new Point(x+10, y + 20);
                    nodeLabel.AutoSize = true;
                    nodeLabel.Visible = true;
                    this.Controls.Add(nodeLabel);

                    Label nbCh = new Label();
                    nbCh.Text = "Nombre d'enfants : " + node.getNbChildren();
                    nbCh.Font = new Font("Arial", 10);
                    nbCh.ForeColor = Color.Black;
                    nbCh.BackColor = Color.Transparent;
                    nbCh.TextAlign = ContentAlignment.MiddleCenter;
                    nbCh.Size = new Size(tailleNode, 50);
                    nbCh.Location = new Point(x, y + 45);
                    nbCh.Visible = true;
                    this.Controls.Add(nbCh);
                } else
                {
                    pevent.Graphics.FillRectangle(b, x, y, tailleNode, tailleNode);
                    Label nodeLabel = new Label();
                    nodeLabel.Text = node.getName();
                    nodeLabel.Font = new Font("Arial", 10, FontStyle.Bold);
                    nodeLabel.ForeColor = Color.Black;
                    nodeLabel.BackColor = Color.Transparent;
                    nodeLabel.TextAlign = ContentAlignment.MiddleCenter;
                    nodeLabel.Size = new Size(100, 50);
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
                    extinctLabel.Font = new Font("Arial", 10);
                    extinctLabel.ForeColor = Color.Black;
                    extinctLabel.BackColor = Color.Transparent;
                    extinctLabel.TextAlign = ContentAlignment.MiddleCenter;
                    extinctLabel.Size = new Size(tailleNode, 50);
                    extinctLabel.Location = new Point(x, y+40);
                    extinctLabel.Visible = true;
                    this.Controls.Add(extinctLabel);
                }
            }

            /*
             * Dessine 1 lien entre deux noeuds
             * @param pevent : l'événement de peinture
             * @param p1 : le premier point
             * @param p2 : le deuxième point
             */
            public void drawLink(PaintEventArgs pevent, Point source, Point target)
            {
                Debug.Print("LINK : " + source.X + ", " + source.Y + " -> " + target.X + ", " + target.Y);
                
                Point p1 = new Point(source.X + tailleNode / 2, source.Y);
                Point p2 = new Point(p1.X, source.Y - (espaceNodeY-tailleNode)/2);
                Point p3 = new Point(target.X + tailleNode / 2, p2.Y);
                Point p4 = new Point(p3.X, target.Y + tailleNode);


                pevent.Graphics.DrawLine(Pens.Black, p1, p2);
                pevent.Graphics.DrawLine(Pens.Black, p2, p3);
                pevent.Graphics.DrawLine(Pens.Black, p3, p4);
            }
            

            /*
             * Calcule la position de chaque noeud de l'arbre
             * @param node : le noeud à partir duquel on calcule la position
             * @param p : la position du noeud
             */
            public void calculTree(Modele.Node node, Point p)
            {
                if(positions.ContainsKey(node))
                    positions[node] = p ;
                else
                    positions.Add(node, p);
                
                if( !node.isClusterNode() && (node.getChildren != null || node.getChildren().Count > 0) )
                {
                    Debug.Print("NOM : " + node.getName() + " is NOT a cluster !!");
                    int depart = p.X - (int)(node.getNbChildren() / 2) * espaceNodeX;
                    
                    foreach (Modele.Node child in node.getChildren())
                    {
                        Debug.Print("   CHILD : " + child.getName());
                        //calculate the position of the child
                        Point childPos = new Point(depart, p.Y - espaceNodeY);
                        
                        //Add the link to the link list
                        links.Add((positions[node], childPos));
                        
                        //recursively calculate the tree for the child
                        calculTree(child, childPos);
                        depart += espaceNodeX;
                    }

                } else Debug.Print("NOM : " + node.getName() + " is a cluster....");
            }


            /*
             * Redessine l'écran
             * @param pevent : l'événement de peinture
             */
            protected override void OnPaintBackground(PaintEventArgs pevent)
            {
                if (repaint)
                {
                    //on efface l'aire de dessin
                    pevent.Graphics.Clear(Color.White);

                    calculTree(this.modele.getRootNode(), new Point(400, 500));

                    foreach ((Point, Point) link in links)
                    {
                        drawLink(pevent, link.Item1, link.Item2);
                    }

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

            /*
             * Vérifie si la souris est sur un noeud
             * @param x : la position x de la souris
             * @param y : la position y de la souris
             */
            public  bool contient(Point node, int xx, int yy)
            {
                return (xx >= node.X && xx <= node.X + tailleNode && yy >= node.Y && yy <= node.Y + tailleNode);
            }


            /*
             * Clic sur :
             * - un noeud : on le sélectionne et affiche les infos
             * - un cluster : on l'ouvre et affiche les enfants (TODO : gérer fermeture cluster --------)
             * @param e : l'événement de la souris
             */
            protected override void OnMouseDown(MouseEventArgs e)
            {

                foreach ( (Modele.Node n, Point p) in positions)
                {
                    if (contient(p, e.X, e.Y))
                    {
                        positions = new Dictionary<Modele.Node, Point>();
                        links = new ArrayList();
                        
                        calculTree(n, p);

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
