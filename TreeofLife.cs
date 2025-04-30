using static Tree_of_Life.Controler;
using System.Collections;
using System.Diagnostics;
using static Tree_of_Life.Modele;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Security.Policy;

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

            //this.StartPosition = FormStartPosition.Manual;
            //this.Bounds = new Rectangle(1, 1, Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y+10) ;
            //this.FormBorderStyle = FormBorderStyle.None;

            // Initialize the screen
            ZoneArbre arbre = new ZoneArbre(modele, this.Width*2/3, this.Height);
            this.Controls.Add(arbre);
            // Set the form's properties
            //this.Text = "Tree of Life";
            ZoneMenu menu = new ZoneMenu(modele, this.Width*2/3, this.Height, this.Width*1/3);
            this.Controls.Add(menu);
            NodeButton.setZoneArbre(arbre);
            NodeButton.setZoneMenu(menu);
            ClusterButton.setZoneArbre(arbre);
            ClusterButton.setZoneMenu(menu);


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
   
            private int topNodeX;
            private int topNodeY;

        //taille de la zone Arbre (stockée au cas où, à voir pour plus tard)
        public int zoneArbreWidth;
            public int zoneArbreHeight;

        //Constantes de taille
        public static int tailleNode = 125;
            public static int espaceNodeX = 175;
            public static int espaceNodeY = 250;

            public static bool init = true;

            public static ArrayList buttons = new ArrayList();


        public ZoneArbre(Modele modele, int w, int h) : base()
        {
            this.modele = modele;
            this.Location = new System.Drawing.Point(0, 0);
            //this.Size = new System.Drawing.Size(1000, 800);
            this.zoneArbreWidth = w; this.zoneArbreHeight = h;
            this.Size = new System.Drawing.Size(w, h);
            this.BackColor = Color.White;
            positions = new Dictionary<Modele.Node, Point>();
            links = new ArrayList();
            rootNode = modele.getRootNode();
            this.AutoScroll = true;
            
            calculTree(rootNode, new Point(400, 1000));
            translateTree(0-topNodeX+20, 0-topNodeY+20);
            foreach (Modele.Node node in positions.Keys)
            {
                createNode(node, positions[node].X, positions[node].Y);
            }
            //calculTree2(rootNode, new Point(0, 1000));
        }

       

        public void setRootNode(Modele.Node n)
        {
            rootNode= n;
            foreach (Control c in buttons)
            {
                Controls.Remove(c);
                c.Dispose();
            }
            foreach (Link l in links)
            {
                Controls.Remove(l);
                l.Dispose();
            }
            buttons.Clear();
            positions.Clear();
            links.Clear();
            init = true;
            calculTree(rootNode, new Point(400, 1000));
            translateTree(0 - topNodeX + 20, 0 - topNodeY + 20);

            foreach (Modele.Node node in positions.Keys)
            {
                createNode(node, positions[node].X, positions[node].Y);
            }
        }

        /*
         * Dessine un noeud
         * @param node : le noeud à dessiner
         * @param x : la position x du noeud
         * @param y : la position y du noeud
         * @param pevent : l'événement de peinture
         */
        public void createNode(Modele.Node node, int x, int y)
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

            if (p.X < topNodeX) topNodeX = p.X; 
            if (p.Y < topNodeY) topNodeY = p.Y;

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

                    Link l = new Link(p, childPos);
                    links.Add(l);
                    Controls.Add(l);

                    //recursively calculate the tree for the child
                    calculTree(child, childPos);
                        depart += espaceNodeX;
                    }

                }
                //else Debug.Print("NOM : " + node.getName() + " is a cluster....");
            }

            /* On va d'abord tt en haut de l'arbre
             * p sera la position du noeud en haut à gauche de l'arbre
             * on fixe les positions de tous les enfants
             * puis on prend le premier enfant et le dernier et on fait la moyenne des positiosn
             * on pourra par la suite faire une translation sur tous les noeuds de l'arbre pour le centrer
             */
            public void calculTree2(Modele.Node node, Point p)
            {
                if (!positions.ContainsKey(node)) { // si le noeud n'est pas encore dans l'arbre
                    if (node.isClusterNode() || node.getChildren().Count == 0) //si le noeud est un cluster ou une leaf
                    {
                        positions.Add(node, p);
                    }
                    else {
                        int x = p.X;
                        foreach (Modele.Node child in node.getChildren())
                        {
                            if (!positions.ContainsKey(child))
                            {
                                calculTree2(child, new Point(x, p.Y - espaceNodeY));
                                x = positions[child].X + espaceNodeX;
                            }
                        }
                        positions.Add(node, new Point((x+p.X-espaceNodeX) / 2, p.Y));

                    }
                }
            }


            public void translateTree(int x, int y)
            {
                //translate all the nodes
                foreach (Modele.Node node in positions.Keys)
                {
                    Point p = positions[node];
                    p.X += x;
                    p.Y += y;
                    positions[node] = p;
                }
                //translate all the links
                foreach (Link link in links)
                {
                    link.translate(x, y);

                //link.Location = new Point(source.X, source.Y);
            }
            }

            /*
             * Redessine l'écran
             * @param pevent : l'événement de peinture
             */
            protected override void OnPaint(PaintEventArgs pevent)
            {
            if (init)
            {
                //on efface l'aire de dessin
                pevent.Graphics.Clear(Color.White);
                //on dessine les liens
                foreach (Link link in links)
                {
                    pevent.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
                    Point source = link.getSource();
                    Point target = link.getTarget();

                    Point p1 = new Point(source.X + tailleNode / 2, source.Y);
                    Point p2 = new Point(p1.X, source.Y - (espaceNodeY - tailleNode) / 2);
                    Point p3 = new Point(target.X + tailleNode / 2, p2.Y);
                    Point p4 = new Point(p3.X, target.Y + tailleNode);

                    pevent.Graphics.DrawLine(Pens.Black, p1, p2);
                    pevent.Graphics.DrawLine(Pens.Black, p2, p3);
                    pevent.Graphics.DrawLine(Pens.Black, p3, p4);
                }

                init = false;
            }
            
            }

        protected override void OnScroll(ScrollEventArgs se)
        {
            init=true; //on redessine l'écran
        }

        //protected override void OnPaintBackground(PaintEventArgs pevent)
        //{
        //    //base.OnPaintBackground(e);
        //    //on efface l'aire de dessin
        //    pevent.Graphics.Clear(Color.White);
        //    foreach ((Point, Point) link in links)
        //    {
        //        drawLink(pevent, link.Item1, link.Item2);
        //    }
        //}



        /* Sous-classe : links (éléments graphiques)
         */
        public class Link : Control
        {
            
            /*************
             * ATTRIBUTS *
             *************/
            private Point source;
            private Point target;


            /****************
             * CONSTRUCTEUR *
             ****************/
            public Link(Point source, Point target)
            {
                this.source = source;
                this.target = target;
            }

            /**************
             * ACCESSEURS *
             **************/
            public Point getSource()
            {
                return source;
            }
            public Point getTarget()
            {
                return target;
            }

            protected override void OnPaint(PaintEventArgs pevent)
            {
                base.OnPaint(pevent);

                Point p1 = new Point(source.X + tailleNode / 2, source.Y);
                Point p2 = new Point(p1.X, source.Y - (espaceNodeY - tailleNode) / 2);
                Point p3 = new Point(target.X + tailleNode / 2, p2.Y);
                Point p4 = new Point(p3.X, target.Y + tailleNode);

                pevent.Graphics.DrawLine(Pens.Black, p1, p2);
                pevent.Graphics.DrawLine(Pens.Black, p2, p3);
                pevent.Graphics.DrawLine(Pens.Black, p3, p4);
            }

            public void translate(int x, int y)
            {
                source.X += x;
                source.Y += y;
                target.X += x;
                target.Y += y;
            }

        }



        }





        //ECRAN MENU -------------------------------------------------------------------
        public class ZoneMenu : Control
        {
            private Modele modele;
            public Label especeCliquée;
            public LinkLabel website;
            public Button image;
            public Label extinct;
            public Label phylesis;

            public Panel nodePath;


        private Modele.Node selectedNode;


        //tailles stockées au cas où
        public int zoneMenuWidth;
        public int zoneMenuHeight;
        public int start;

        public ZoneMenu(Modele modele, int w, int h, int start) : base()
            {
                this.modele = modele;
                this.zoneMenuWidth = w; this.zoneMenuHeight = h;
                this.start = start;

                this.especeCliquée = new Label();
                this.especeCliquée.Location = new System.Drawing.Point(start+zoneMenuWidth/5, 25+zoneMenuHeight/5);
                this.especeCliquée.Font = new Font("Arial", 12, FontStyle.Bold);
                this.especeCliquée.AutoSize = true;
                this.especeCliquée.Size = new System.Drawing.Size(200, 20);
                this.Controls.Add(this.especeCliquée);

                this.website = new LinkLabel();
                this.website.Location = new System.Drawing.Point(start + 100, 60 + zoneMenuHeight / 5);
                this.website.Font = new Font("Arial", 12);
                this.website.AutoSize = true;
                this.website.Size = new System.Drawing.Size(200, 20);
                this.Controls.Add(this.website);

                //plus tard : image venant du site TODO ----------------------------------------------------------------

                this.extinct = new Label();
                this.extinct.Location = new System.Drawing.Point(start + 100, 120 + zoneMenuHeight / 5);
                this.extinct.Font = new Font("Arial", 12);
                this.extinct.AutoSize = true;
                this.extinct.Size = new System.Drawing.Size(200, 20);
                this.Controls.Add(this.extinct);

                this.phylesis = new Label();
                this.phylesis.Location = new System.Drawing.Point(start + 100, 180 + zoneMenuHeight / 5);
                this.phylesis.Font = new Font("Arial", 12);
                this.phylesis.AutoSize = true;
                this.phylesis.Size = new System.Drawing.Size(200, 20);
                this.Controls.Add(this.phylesis);

                this.nodePath = new Panel();
                this.nodePath.Location = new System.Drawing.Point(start + 100, 250 + zoneMenuHeight / 5);
                this.nodePath.Size = new System.Drawing.Size(200, 200);
                this.nodePath.AutoSize = true;
                this.Controls.Add(nodePath);

            this.selectedNode = this.modele.getRootNode();


            //this.StartPosition = FormStartPosition.Manual;
            //this.FormBorderStyle = FormBorderStyle.None;
            //this.Location = new System.Drawing.Point(1000, 0);
            this.Location = new System.Drawing.Point(start, 0);
                //this.Size = new System.Drawing.Size(400, 800);
                this.Size = new System.Drawing.Size(w, h);



            this.BackColor = Color.LightGray;
            }

        public void setSelectedNode(Modele.Node n)
        {
            selectedNode = n;
        }

        public void printPath(int start, Panel pan)
        {
            int initial = start + 100;
            if (selectedNode != null)
            {
                foreach (string s in selectedNode.getPath())
                {
                    Label l = new Label();
                    l.Location = new System.Drawing.Point(initial, 250 + zoneMenuHeight / 5);
                    l.Font = new Font("Arial", 12);
                    l.AutoSize = true;
                    l.Size = new System.Drawing.Size(200, 20);
                    l.Text = "/"+s;
                    pan.Controls.Add(l);
                    initial += 50;
                }
            }
        }
    }
    



    }
