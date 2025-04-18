using static Tree_of_Life.Controler;
using System.Collections;
using System.Diagnostics;
using static Tree_of_Life.Modele;

namespace Tree_of_Life
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Initialize the model
            Modele modele = new Modele();
            // Get the root node
            Modele.Node rootNode = modele.getRootNode();
            // Display the root node's name
            //MessageBox.Show("Root Node: " + rootNode.getName());

            // Initialize the screen
            ZoneArbre arbre = new ZoneArbre(modele);
            this.Controls.Add(arbre);
            // Set the form's properties
            //this.Text = "Tree of Life";
            ZoneMenu menu = new ZoneMenu(modele);
            this.Controls.Add(menu);
            NodeButton.setZoneArbre(arbre);
            ClusterButton.setZoneArbre(arbre);


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
        



        //ECRAN DE L'ARBRE -------------------------------------------------------------------
        public class ZoneArbre : ScrollableControl
        {
            private Modele modele;
            public static Dictionary<Modele.Node, Point> positions;
            public static ArrayList links;

            public static Modele.Node rootNode;

            //Constantes de taille
            public static int tailleNode = 125;
            public static int espaceNodeX = 175;
            public static int espaceNodeY = 250;

            public static bool init = true;

            public static ArrayList buttons = new ArrayList();


        public ZoneArbre(Modele modele) : base()
        {
            this.modele = modele;
            this.Location = new System.Drawing.Point(0, 0);
            this.Size = new System.Drawing.Size(1000, 800);
            this.BackColor = Color.White;
            positions = new Dictionary<Modele.Node, Point>();
            links = new ArrayList();
            rootNode = modele.getRootNode();
            this.AutoScroll = true;
        }

        public static void setRootNode(Modele.Node node)
        {
            rootNode= node;
            buttons.Clear();
            foreach (Control c in buttons)
            {
                buttons.Remove(c);
                //Controls.Remove(c);
                c.Dispose();
            }
            positions.Clear();
            links.Clear();
            init = true;
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
                    ClusterButton cn = new ClusterButton(node, new Point(x, y));
                    Controls.Add(cn);
                    buttons.Add(cn);

                }
                else
                {
                        NodeButton nb = new NodeButton(node, new Point(x, y));
                        Controls.Add(nb);
                        buttons.Add(nb);
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
                //Debug.Print("LINK : " + source.X + ", " + source.Y + " -> " + target.X + ", " + target.Y);

                Point p1 = new Point(source.X + tailleNode / 2, source.Y);
                Point p2 = new Point(p1.X, source.Y - (espaceNodeY - tailleNode) / 2);
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
                if (positions.ContainsKey(node))
                    positions[node] = p;
                else
                    positions.Add(node, p);

                if ( (node.getChildren().Count > 0 && !node.isClusterNode()) || node == rootNode)
                {
                    //Debug.Print("NOM : " + node.getName() + " is NOT a cluster !!");
                    int depart = p.X - (node.getChildren().Count - 1) * espaceNodeX / 2;

                    foreach (Modele.Node child in node.getChildren())
                    {
                        //Debug.Print("   CHILD : " + child.getName());
                        //calculate the position of the child
                        Point childPos = new Point(depart, p.Y - espaceNodeY);

                        //Add the link to the link list
                        links.Add((positions[node], childPos));

                        //recursively calculate the tree for the child
                        calculTree(child, childPos);
                        depart += espaceNodeX;
                    }

                }
                //else Debug.Print("NOM : " + node.getName() + " is a cluster....");
            }

        


            /*
             * Redessine l'écran
             * @param pevent : l'événement de peinture
             */
            protected override void OnPaintBackground(PaintEventArgs pevent)
            {
            if (init)
            {
                //on efface l'aire de dessin
                pevent.Graphics.Clear(Color.White);

                calculTree(rootNode, new Point(400, 500));

                foreach (Modele.Node node in positions.Keys)
                {
                    //draw the node
                    drawNode(node, positions[node].X, positions[node].Y, pevent);
                }

                foreach ((Point, Point) link in links)
                {
                    drawLink(pevent, link.Item1, link.Item2);
                }

                init = false;
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
