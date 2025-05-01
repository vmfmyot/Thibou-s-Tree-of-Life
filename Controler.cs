using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static Tree_of_Life.Modele;

namespace Tree_of_Life
{
    internal class Controler
    {
        Modele modele;
        ZoneArbre arbre;
        ZoneMenu menu;

        public Controler(Modele modele, ZoneArbre zoneArbre, ZoneMenu zoneMenu)
        {
            this.modele = modele;
            this.arbre = zoneArbre;
            this.menu = zoneMenu;
        }

        //Classe pour représenter les boutons pour les nodes
        public class NodeButton : Button
        {
            private Modele.Node node;
            public static ZoneArbre arbre;
            public static ZoneMenu menu;

            public static void setZoneArbre(ZoneArbre a)
            {
                if (arbre == null)
                {
                    NodeButton.arbre = a;
                }
            }
            public static void setZoneMenu(ZoneMenu m)
            {
                if (menu == null)
                {
                    NodeButton.menu = m;
                }
            }

            public NodeButton(Modele.Node node, Point p)
            {
                this.node = node;
                this.Text = node.getName();
                this.Size = new Size(ZoneArbre.tailleNode, ZoneArbre.tailleNode);
                this.BackColor = node.getShade();
                this.ForeColor = Color.White;
                this.Font = new Font("Arial", 10, FontStyle.Bold);
                this.Location = p;
                this.Cursor = Cursors.Hand;
            }
            public Modele.Node getNode()
            {
                return node;
            }

            protected override void OnClick(EventArgs e)
            {
                menu.updateInfos(node);
                
            }
        }

        //Classe pour représenter les boutons pour les clusters
        public class ClusterButton : Button
        {
            private Modele.Node node;
            public static ZoneArbre arbre;
            public static ZoneMenu menu;

            public static void setZoneArbre(ZoneArbre a)
            {
                if (arbre == null)
                    ClusterButton.arbre = a;
            }
            public static void setZoneMenu(ZoneMenu m)
            {
                if (menu == null)
                    ClusterButton.menu = m;
            }

            public ClusterButton(Modele.Node node, Point p)
            {
                this.node = node;
                this.Text = node.getName();
                this.Size = new Size(ZoneArbre.tailleNode, ZoneArbre.tailleNode);
                this.BackColor = node.getShade();
                this.ForeColor = Color.White;
                this.Font = new Font("Arial", 10, FontStyle.Bold);
                this.Location = p;
                this.Cursor = Cursors.Hand;
            }


            GraphicsPath GetRoundPath(RectangleF Rect, int radius)
            {
                GraphicsPath GraphPath = new GraphicsPath();
                GraphPath.AddEllipse(Rect);
                return GraphPath;

            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
                using (GraphicsPath GraphPath = GetRoundPath(Rect, ZoneArbre.tailleNode))
                {
                    this.Region = new Region(GraphPath);
                    using (Pen pen = new Pen(Color.Orange, 1.75f))
                    {
                        pen.Alignment = PenAlignment.Inset;
                        e.Graphics.DrawPath(pen, GraphPath);
                    }
                }
            }

            public Modele.Node getNode()
            {
                return node;
            }

            protected override void OnClick(EventArgs e)
            {
                arbre.setRootNode(node);
                menu.updateInfos(node);
            }
        }




        /*
         * Label cliquable dans un chemin de parents de noeuds
         * allant jusqu'au noeud sélectionné
         */
        public class PathLabel : Label
        {
            public ZoneMenu menu;
            private ZoneArbre arbre;
            private Node node;


            public PathLabel(ZoneMenu menu, ZoneArbre arbre)
            {
                this.menu = menu;
                this.arbre = arbre;
                this.node = null;
                this.Text = "";
                this.Click += Label_Click;
                this.Cursor = Cursors.Hand;
            }

            public PathLabel(Node n, ZoneMenu m, ZoneArbre a)
            {
                this.menu = m; this.arbre = a;
                this.node = n;
                this.Text = n.getName();
                this.Click += Label_Click;
                this.Cursor = Cursors.Hand;
            }

            public void setNode(Node n) { this.node = n; }
            

            private void Label_Click(object sender, EventArgs e)
            {
                menu.updateInfos(node);
                arbre.setRootNode(node);
            }
        }


        /*
         * Barre de recherche
         * permettant de rechercher des espèces dans l'arbre
         */
        public class SearchBox : TextBox
        {
            Modele modele;
            ArrayList resultLabels;

            private Point loc; //localisation
            private Size siz; //taille

            ZoneMenu menu;

            ArrayList filters;

            public SearchBox(Modele m, ZoneMenu me, Point p) {
                modele = m;
                menu = me;

                this.Location = p; this.loc = p;

                this.TextChanged += search;
                resultLabels = new ArrayList();
                for (int i = 0; i < 5; i++)
                {
                    resultLabels.Add(new Label());
                }

                filters = new ArrayList();

                CheckBox checkBoxHasLink = new CheckBox();
                checkBoxHasLink.Text = "site web";
                checkBoxHasLink.Click += setFilter;
                checkBoxHasLink.Location = new Point(loc.X, 35);
                checkBoxHasLink.Size = new Size(200, 30);
                me.Controls.Add(checkBoxHasLink);

                CheckBox checkBoxLeaf = new CheckBox();
                checkBoxLeaf.Text = "feuille";
                checkBoxLeaf.Click += setFilter;
                checkBoxLeaf.Location = new Point(loc.X+250, 35);
                checkBoxLeaf.Size = new Size(200, 30);
                me.Controls.Add(checkBoxLeaf);

                CheckBox checkBoxDead = new CheckBox();
                checkBoxDead.Text = "espèces éteintes";
                checkBoxDead.Click += setFilter;
                checkBoxDead.Location = new Point(loc.X, 65);
                checkBoxDead.Size = new Size(200, 30);
                me.Controls.Add(checkBoxDead);

                CheckBox checkBoxAlive = new CheckBox();
                checkBoxAlive.Text = "espèces en vie";
                checkBoxAlive.Click += setFilter;
                checkBoxAlive.Location = new Point(loc.X + 250, 65);
                checkBoxAlive.Size = new Size(200, 30);
                me.Controls.Add(checkBoxAlive);


            }

            private void setFilter(object sender, EventArgs e) {
                CheckBox checkBox = ((CheckBox)sender);
                if (filters.Contains(checkBox.Text)) filters.Remove(checkBox.Text);
                else filters.Add(checkBox.Text);
            }



            public void search(object sender, EventArgs e) 
            {
                if( resultLabels.Count > 0 || resultLabels.Count != null) {
                    foreach (Label l in resultLabels)
                    {
                        this.menu.Controls.Remove(l);
                    }
                }
                
                resultLabels.Clear();
                ArrayList top5 = getTop5(this.Text);
                int y = loc.Y + 30;

                for (int i=0; i<top5.Count; i++)
                {
                    
                    PathLabel l = new PathLabel((Node)top5[i], menu, menu.arbre);
                    l.Location = new Point(loc.X, y);
                    l.Font = new Font("Segoe UI", 9);
                    l.Text = ((Node)top5[i]).getName();
                    l.Size = siz;
                    l.BackColor = Color.White;

                    resultLabels.Add(l);
                    this.menu.Controls.Add(l);
                    y += 25;
                }
            }

            public bool applyFilters(Node n)
            {
                foreach (string s in filters)
                {
                    switch (s)
                    {
                        case "site web": if (! n.hasWebsiteNode()) return false; break;
                        case "feuille": if (! n.isLeafNode()) return false; break; ;
                        case "espèces éteintes": if (!n.isExtinctNode()) return false; break; ;
                        case "espèces en vie": if (n.isExtinctNode()) return false; break; ;
                    }
                }
                return true;
            }
        

            public ArrayList getTop5(string text)
            {
                ArrayList res = new ArrayList();
                foreach((String s, Node n) in modele.getSpeciesList())
                {
                    if (s.Contains(text) && applyFilters(n))
                    {
                        res.Add(n);
                        if (res.Count >= 5) return res;
                    }
                }
                return res;
            }

            public void setLoc(int x, int y) {
                loc.X = x; loc.Y = y;
            }

            public void setSiz(int l, int r)
            {
                siz = new Size(l, r);
            }
        }
    }     
}
