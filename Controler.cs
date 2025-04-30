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
                menu.setSelectedNode(node);
                menu.especeCliquée.Text = node.getName();
                menu.nodePath.Controls.Clear();
                menu.printPath(menu.start, menu.nodePath);



                if (node.hasWebsiteNode())
                {
                    menu.website.Text = "http://tolweb.org/$"+node.getName()+"/$"+node.getId();
                    menu.website.ActiveLinkColor = Color.Blue;
                    menu.website.ForeColor = Color.Blue;
                }
                else
                {
                    menu.website.Text = "Pas de site internet disponible";
                    menu.website.ActiveLinkColor = Color.Black;
                    menu.website.ForeColor = Color.Black;
                }
                
                if(node.isExtinctNode())
                {
                    menu.extinct.Text = "Espèce éteinte";
                    menu.extinct.ForeColor = Color.Red;
                }
                else
                {
                    menu.extinct.Text = "Espèce en vie";
                    menu.extinct.ForeColor = Color.Green;
                }

                menu.phylesis.Text = "Phylesis : " + node.getPhylesis();
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
                menu.setSelectedNode(node);
                arbre.setRootNode(node);

                menu.especeCliquée.Text = node.getName();

                menu.printPath(menu.start, menu.nodePath);

                if (node.hasWebsiteNode())
                {
                    menu.website.Text = "http://tolweb.org/$" + node.getName() + "/$" + node.getId();
                    menu.website.ActiveLinkColor = Color.Blue;
                    menu.website.ForeColor = Color.Blue;
                }
                else
                {
                    menu.website.Text = "Pas de site internet disponible";
                    menu.website.ActiveLinkColor = Color.Black;
                    menu.website.ForeColor = Color.Black;
                }

                if (node.isExtinctNode())
                {
                    menu.extinct.Text = "Espèce éteinte";
                    menu.extinct.ForeColor = Color.Red;
                }
                else
                {
                    menu.extinct.Text = "Espèce en vie";
                    menu.extinct.ForeColor = Color.Green;
                }
                menu.phylesis.Text = "Phylesis : " + node.getPhylesis();
            }
        }
    }     
}
