using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree_of_Life
{
    internal class Ecran
    {
        Modele modele;

        public Ecran(Modele modele)
        {
            this.modele = modele;
        }


        public class ZoneArbre : Control
        {
            public ZoneArbre() : base()
            {
                this.Location = new System.Drawing.Point(0, 0);
                this.Size = new System.Drawing.Size(1000, 800);
                this.BackColor = Color.White;
            }
        }

        public class ZoneMenu : Control
        {
            public ZoneMenu() : base()
            {
                this.Location = new System.Drawing.Point(1200, 0);
                this.Size = new System.Drawing.Size(400, 800);
                this.BackColor = Color.Gray;
            }
        }
    }
}
