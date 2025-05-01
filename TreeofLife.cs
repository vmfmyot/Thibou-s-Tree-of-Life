using static Tree_of_Life.Controler;
using System.Collections;
using System.Diagnostics;
using static Tree_of_Life.Modele;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Security.Policy;
using System.Drawing.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using System.Drawing;

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

            

            // Initialize the screen
            ZoneArbre arbre = new ZoneArbre(modele, this.Width*2/3, this.Height);
            this.Controls.Add(arbre);

            ZoneMenu menu = new ZoneMenu(modele, this.Width*2/3, this.Height, this.Width*1/3, arbre);
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
            public static Dictionary<Node, System.Windows.Forms.Button> buttons = new Dictionary<Node, System.Windows.Forms.Button>();
            public static ArrayList links;

            public static Modele.Node rootNode;
   
            private int topNodeX;
            private int topNodeY;

        //taille de la zone Arbre (stockée au cas où, à voir pour plus tard)
        public int zoneArbreWidth;
            public int zoneArbreHeight;

        //Constantes de taille
        public static int tailleNode = 70;
        public static int espaceNodeX = 10;
        public static int espaceNodeY = 20;

            public static bool init = true;



        public ZoneArbre(Modele modele, int w, int h) : base()
        {
            this.modele = modele;
            this.Location = new System.Drawing.Point(0, 0);
            //this.Size = new System.Drawing.Size(1000, 800);
            this.zoneArbreWidth = w; this.zoneArbreHeight = h;
            this.Size = new System.Drawing.Size(w, h);
            this.BackColor = Color.Beige;
            positions = new Dictionary<Modele.Node, Point>();
            links = new ArrayList();
            rootNode = modele.getRootNode();
            this.AutoScroll = true;


            calcTree(rootNode);
            foreach (Modele.Node node in positions.Keys)
            {
                createNode(node, positions[node].X, positions[node].Y);
            }
        }

       

        public void setRootNode(Modele.Node n)
        {
            rootNode= n;
            foreach ((Node node, Control c) in buttons)
            {
                Controls.Remove(c);
                c.Dispose();
            }
            buttons.Clear();
            positions.Clear();
            links.Clear();

            init = true;
            calcTree(rootNode);
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
                buttons.Add(node, cn);
                }
                else
                {
                    NodeButton nb = new NodeButton(node, new Point(x, y));
                    Controls.Add(nb);
                    buttons.Add(node, nb);
                }
            }

           


            

            private int maxX = 0; //sert pour l'affichage

            /* On va d'abord tt en haut de l'arbre
             * p sera la position du noeud en haut à gauche de l'arbre
             * on fixe les positions de tous les enfants
             * puis on prend le premier enfant et le dernier et on fait la moyenne des positiosn
             * on pourra par la suite faire une translation sur tous les noeuds de l'arbre pour le centrer
             */
            public bool calculPos(Modele.Node node, int y)
            {
                if (!positions.ContainsKey(node)) { // si le noeud n'est pas encore dans l'arbre
                    if ((node.isClusterNode() && node!=rootNode) || node.getChildren().Count == 0) //si le noeud est un cluster ou une leaf
                    {
                        maxX = maxX + tailleNode + espaceNodeX;
                        positions.Add(node, new Point (maxX, y));
                        return true;
                    }
                    else
                    {
                        ArrayList childPos = new ArrayList();
                        foreach (Modele.Node child in node.getChildren())
                        {
                            if (!positions.ContainsKey(child))
                            {
                                bool b = calculPos(child, y+espaceNodeY+tailleNode);
                                if (b)
                                {
                                    childPos.Add(positions[child]);
                                    links.Add(new Link(node, child));
                                }
                            }
                        }
                        // Tout ça parce qu'il y a des boucles
                        Point source = new Point((((Point)childPos[0]).X + ((Point)childPos[childPos.Count - 1]).X) / 2, y); 
                        positions.Add(node, source);

                        return true;
                    }
                }
            return false;
            }


        public void calcTree(Node root)
        {
            maxX = 0;
            calculPos(root,20);
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
                pevent.Graphics.Clear(Color.Beige);
                //on dessine les liens
                foreach (Link link in links)
                {
                    Point source = buttons[link.getSource()].Location;
                    Point target = buttons[link.getTarget()].Location;

                    Point p1 = new Point(source.X + tailleNode / 2, source.Y);
                    Point p2 = new Point(source.X + tailleNode / 2, source.Y + tailleNode +espaceNodeY/2);
                    Point p3 = new Point(target.X + tailleNode / 2, target.Y - espaceNodeY / 2);
                    Point p4 = new Point(target.X + tailleNode / 2, target.Y);


                    pevent.Graphics.DrawLine(Pens.Black, p1, p2);
                    pevent.Graphics.DrawLine(Pens.Black, p2, p3);
                    pevent.Graphics.DrawLine(Pens.Black, p3, p4);
                }

                init = false;
            }
            
            }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            init = true;
            Invalidate(); // Force le repaint pour redessiner les liens visibles
        }




















        /* Sous-classe : links (éléments graphiques)
         */
        public class Link 
        {
            
            /*************
             * ATTRIBUTS *
             *************/
            private Node source;
            private Node target;


            /****************
             * CONSTRUCTEUR *
             ****************/
            public Link(Node source, Node target)
            {
                this.source = source;
                this.target = target;
            }

            /**************
             * ACCESSEURS *
             **************/
            public Node getSource()
            {
                return source;
            }
            public Node getTarget()
            {
                return target;
            }

        }



        }












        //ECRAN MENU -------------------------------------------------------------------
        public class ZoneMenu : Control
        {
            private Modele modele;
            public ZoneArbre arbre;
            private PathLabel especeCliquée;
            public LinkLabel website;
            public Label extinct;
            public Label phylesis;
            public Label extinctNb;
            public Label totalNb;

            public Panel nodePath;


            public Modele.Node selectedNode;

            private SearchBox searchBox;


            //tailles stockées au cas où
            public int zoneMenuWidth;
            public int zoneMenuHeight;
            public int start;


        public ZoneMenu(Modele modele, int w, int h, int start, ZoneArbre a) : base()
            {
                this.modele = modele; this.arbre = a;
                this.zoneMenuWidth = w; this.zoneMenuHeight = h;
                this.start = start; // début en x


                //Création de la barre de recherche
                searchBox = new SearchBox(modele, this, new Point(start + 50, 105));
                searchBox.PlaceholderText = "Rechercher une espèce";
                searchBox.Size = new Size(2*zoneMenuWidth/5, 30);
                searchBox.setSiz(2 * zoneMenuWidth / 5, 30);
                this.Controls.Add(searchBox);
                


                //Nom de l'espèce lorsque l'on clique
                this.especeCliquée = new PathLabel(this, this.arbre);
                this.especeCliquée.BackColor = Color.FromArgb(255, 200, 255, 200);
                this.especeCliquée.Location = new System.Drawing.Point(start+50, 95+zoneMenuHeight/5);
                this.especeCliquée.Font = new Font("Arial", 12, FontStyle.Bold);
                this.especeCliquée.AutoSize = true;
                this.especeCliquée.Size = new System.Drawing.Size(200, 20);
                this.Controls.Add(this.especeCliquée);

                //Lien du site lorsque l'on clique sur une espèce
                this.website = new LinkLabel();
                this.website.BackColor = Color.FromArgb(255, 200, 255, 200);
                this.website.Location = new System.Drawing.Point(start + 50, 155 + zoneMenuHeight / 5);
                this.website.Font = new Font("Arial", 12);
                this.website.AutoSize = true;
                this.website.Size = new System.Drawing.Size(200, 20);
                this.Controls.Add(this.website);

                //plus tard : image venant du site TODO ----------------------------------------------------------------

                this.extinct = new Label();
                this.extinct.BackColor = Color.FromArgb(255, 200, 255, 200);
                this.extinct.Location = new System.Drawing.Point(start + 50, 185 + zoneMenuHeight / 5);
                this.extinct.Font = new Font("Arial", 12);
                this.extinct.AutoSize = true;
                this.extinct.Size = new System.Drawing.Size(200, 20);
                this.Controls.Add(this.extinct);

                this.phylesis = new Label();
            this.phylesis.BackColor = Color.FromArgb(255, 200, 255, 200);
                this.phylesis.Location = new System.Drawing.Point(start + 50, 215 + zoneMenuHeight / 5);
                this.phylesis.Font = new Font("Arial", 12);
                this.phylesis.AutoSize = true;
                this.phylesis.Size = new System.Drawing.Size(200, 20);
                this.Controls.Add(this.phylesis);

                this.nodePath = new Panel();
                this.nodePath.BackColor = Color.FromArgb(255, 200, 255, 200);
                this.nodePath.Location = new System.Drawing.Point(start + 50, 275 + zoneMenuHeight / 5);
                this.nodePath.Size = new System.Drawing.Size(2 * zoneMenuWidth / 5, 50);
                this.nodePath.AutoScroll = true;
                this.Controls.Add(nodePath);

                
                this.selectedNode = this.modele.getRootNode();

                this.Location = new System.Drawing.Point(start, 0);
                this.Size = new System.Drawing.Size(w, h);

                this.BackColor = Color.LightGreen;


                //Infos de Thibou
                Label l = new Label();
                l.Text = "Quelques chiffres : ";
                l.ForeColor = Color.White;
                l.BackColor = Color.DarkGreen;
                l.Size = new System.Drawing.Size(200, 30);
                l.Font = new Font("Comic Sans MS", 12, FontStyle.Bold);
                l.Location = new Point(start + 150, zoneMenuHeight / 5 + 380);
                Controls.Add(l);

                extinctNb= new Label();
                extinctNb.AutoSize = true;
                extinctNb.BackColor = Color.DarkGreen; 
                extinctNb.ForeColor = Color.White;
                extinctNb.Font = new Font("Comic Sans MS", 12);
                extinctNb.Location = new Point(start + 50, zoneMenuHeight / 5 + 420);
                Controls.Add(extinctNb);

                totalNb = new Label();
                totalNb.AutoSize = true;
                totalNb.BackColor = Color.DarkGreen;
                totalNb.ForeColor = Color.White;
                totalNb.Font = new Font("Comic Sans MS", 12);
                totalNb.Font = new Font("Comic Sans MS", 12);
                totalNb.Location = new Point(start + 50, zoneMenuHeight / 5 + 450);
                Controls.Add(totalNb);

                //label panneau
                ArrayList noms = ["Rattus group", "Strigiformes", "Corvoidea", "Giraffidae", "Cetacea"];
                ArrayList points = [new Point(start+65, zoneMenuHeight - 255), 
                                    new Point(start+65, zoneMenuHeight - 210), 
                                    new Point(start+110, zoneMenuHeight - 170), 
                                    new Point(start+75, zoneMenuHeight - 130),
                                    new Point(start+110, zoneMenuHeight - 90),];
                for (int i = 0; i < noms.Count; i++)
                {
                    PathLabel label = new PathLabel(modele.getSpeciesList()[(string)noms[i]], this, arbre);
                    label.AutoSize = true;
                    label.Text = (string) noms[i];
                    label.BackColor = Color.Transparent;
                    label.Font = new Font("Comic Sans MS", 9, FontStyle.Bold);
                    label.Location = (Point) points[i];
                    Controls.Add((label));
                }




        }

        public void setSelectedNode(Modele.Node n)
        {
            selectedNode = n;
            nodePath.Controls.Clear();
            printPath(start, nodePath);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.DrawLine(new Pen(Color.DarkGreen, 3), new PointF(start, 0), new Point(start, zoneMenuHeight));
            pevent.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255, 200, 255, 200)), new Rectangle(new Point(start+45, 90 + zoneMenuHeight / 5), new Size(2 * zoneMenuWidth / 5 +10 , 245)));
            
            Image i = Image.FromFile("../../../Resources/latest.png");
            pevent.Graphics.DrawImage(i, new Point(zoneMenuWidth - 310, zoneMenuHeight - 300));


            Image sign = Image.FromFile("../../../Resources/sign.png");
            pevent.Graphics.DrawImage(sign, new Point(start, zoneMenuHeight - 300));


            pevent.Graphics.FillRectangle(new SolidBrush(Color.Brown), new Rectangle(new Point(start + 40, zoneMenuHeight / 5 + 375), new Size(2 * zoneMenuWidth / 5 + 15, 115)));
            pevent.Graphics.FillRectangle(new SolidBrush(Color.DarkGreen), new Rectangle(new Point(start + 45, zoneMenuHeight / 5 + 380), new Size(2 * zoneMenuWidth / 5 + 5, 105)));



        }

        public void printPath(int start, Panel pan)
        {
            int initial = 0;
            if (selectedNode != null)
            {
                ArrayList path = new ArrayList();
                path.Add(selectedNode);
                //revPath.Reverse();

                Node n = selectedNode;
                while (n != modele.getRootNode())
                {
                    path.Add(n.getParentNode());
                    n = n.getParentNode();
                }

                path.Reverse();
               

                foreach (Node node in path)
                {
                    Label dash = new Label();
                    dash.Location = new System.Drawing.Point(initial, 0);
                    dash.Font = new Font("Arial", 12);
                    dash.AutoSize = true;
                    dash.Size = new System.Drawing.Size(200, 20);
                    dash.Text = "/";
                    pan.Controls.Add(dash);

                    initial += 15;

                    PathLabel l = new PathLabel(node, this, this.arbre);
                    l.Location = new System.Drawing.Point(initial, 0);
                    l.Font = new Font("Arial", 12);
                    l.AutoSize = true;
                    l.Size = new System.Drawing.Size(200, 20);

                    pan.Controls.Add(l);
                    initial += l.Size.Width;
                }

            }
        }


        private void openLink(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = ((Label)sender).Text,
                    UseShellExecute = true // Required for modern .NET versions
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open the link: {ex.Message}");
            }
        }

        /*
         * Met à jour les infos du noeud cliqué dans ZoneMenu
         * @param Node node
         */
        public void updateInfos(Node node)
        {
            setSelectedNode(node);
            extinctNb.Text ="Nombre d'enfants éteints : "+ node.getNbExtinctChildren();
            totalNb.Text = "Nombre d'enfants au total : "+ node.getNbChildren();


            //mise à jour du pathLabel du nom de l'espèce
            this.especeCliquée.Text = node.getName();
            this.especeCliquée.setNode(node);

            nodePath.Controls.Clear();
            printPath(start, nodePath);

            if (node.hasWebsiteNode())
            {
                website.Text = "http://tolweb.org/" + node.getName() + "/" + node.getId();
                website.ActiveLinkColor = Color.Blue;
                website.ForeColor = Color.Blue;
                website.Click += openLink;
            }
            else
            {
                website.Text = "Pas de site internet disponible";
                website.ActiveLinkColor = Color.Black;
                website.ForeColor = Color.Black;
            }

            if (node.isExtinctNode())
            {
                extinct.Text = "Espèce éteinte";
                extinct.ForeColor = Color.Red;
            }
            else
            {
                extinct.Text = "Espèce en vie";
                extinct.ForeColor = Color.Green;
            }

            phylesis.Text = "Phylesis : " + node.getPhylesis();


        }


        } //FIN ZONE MENU ------------------------------------------------------------------------------------
    



    }
