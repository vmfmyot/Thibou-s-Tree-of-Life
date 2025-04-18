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

        public Controler(Modele modele, ZoneArbre zoneArbre)
        {
            this.modele = modele;
            this.arbre = zoneArbre;
        }

        //Classe pour représenter les boutons pour les nodes
        public class NodeButton : Button
        {
            private Modele.Node node;
            public static ZoneArbre arbre;

            public static void setZoneArbre(ZoneArbre a)
            {
                NodeButton.arbre = a;
            }
            public NodeButton(Modele.Node node, Point p)
            {
                this.node = node;
                this.Text = node.getName();
                this.Size = new Size(ZoneArbre.tailleNode, ZoneArbre.tailleNode);
                this.BackColor = Color.Orange;
                this.ForeColor = Color.Black;
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
                Debug.Print("CLICK : " + node.getName() + " at " + this.Location.X + ", " + this.Location.Y);
            }
        }

        //Classe pour représenter les boutons pour les clusters
        public class ClusterButton : Button
        {
            private Modele.Node node;
            public static ZoneArbre arbre;

            public static void setZoneArbre(ZoneArbre a)
            {
                NodeButton.arbre = a;
            }
            public ClusterButton(Modele.Node node, Point p)
            {
                this.node = node;
                this.Text = node.getName();
                this.Size = new Size(ZoneArbre.tailleNode, ZoneArbre.tailleNode);
                this.BackColor = Color.Orange;
                this.ForeColor = Color.Black;
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
                ZoneArbre.setRootNode(node);
            }

        }



    }     
}
